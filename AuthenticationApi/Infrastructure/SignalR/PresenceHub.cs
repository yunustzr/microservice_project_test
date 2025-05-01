using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Infrastructure.Repositories;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace AuthenticationApi.Infrastructure.SignalR
{
    public class PresenceHub : Hub
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserConnectionRepository _connectionRepository;

        public PresenceHub(
            IUserRepository userRepository,
            IUserConnectionRepository connectionRepository)
        {
            _userRepository = userRepository;
            _connectionRepository = connectionRepository;
        }

        private async Task BroadcastOnlineCountAsync()
        {
            // Aktif bağlantıları al
            var connections = await _connectionRepository.GetAllActiveConnectionsAsync();
            // Kullanıcı ID'lerini ayıkla, tekrarlananları kaldır ve say
            var activeUsers = connections
                .Select(c => c.UserId)
                .Distinct()
                .Count();

            await Clients.All.SendAsync("OnlineCount", activeUsers);
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserIdFromContext();
            var connectionId = Context.ConnectionId;

            // Kullanıcıyı online olarak işaretle
            await UpdateUserOnlineStatus(userId, true);

            // Connection kaydı ekle
            await _connectionRepository.AddAsync(new UserConnection
            {
                ConnectionId   = connectionId,
                UserId         = userId,
                ConnectedAt    = DateTime.UtcNow
            });

            // Tüm client'lara bildir
            await Clients.All.SendAsync("UserOnline", userId);
            await BroadcastOnlineCountAsync();
            await base.OnConnectedAsync();
        }



        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = GetUserIdFromContext();
            var connectionId = Context.ConnectionId;

            // Connection kaydını güncelle
            var connection = await _connectionRepository.GetByConnectionIdAsync(connectionId);
            if (connection != null)
            {
                connection.DisconnectedAt = DateTime.UtcNow;
                await _connectionRepository.UpdateAsync(connection);
            }

            // Aktif başka bağlantı yoksa offline yap
            if (!await _connectionRepository.HasActiveConnectionsAsync(userId))
            {
                await UpdateUserOnlineStatus(userId, false);
                await Clients.All.SendAsync("UserOffline", userId);
            }
            await BroadcastOnlineCountAsync();

            await base.OnDisconnectedAsync(exception);
        }

        private Guid GetUserIdFromContext()
            => Guid.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

        private async Task UpdateUserOnlineStatus(Guid userId, bool isOnline)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.IsOnline = isOnline;
                user.LastConnectedAt    = isOnline ? DateTime.UtcNow : user.LastConnectedAt;
                user.LastDisconnectedAt = !isOnline ? DateTime.UtcNow : user.LastDisconnectedAt;
                await _userRepository.UpdateAsync(user);
            }
        }
    }
}