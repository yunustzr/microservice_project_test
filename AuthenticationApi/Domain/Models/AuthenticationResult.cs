namespace AuthenticationApi.Domain.Models
{
    public class AuthenticationResult
    {
        public bool IsAuthenticated { get; set; }
        public string? Message { get; set; }
        public User? User { get; set; }

        public static AuthenticationResult Success(User user)
        {
            return new AuthenticationResult { IsAuthenticated = true, User = user };
        }

        public static AuthenticationResult Failure(string message)
        {
            return new AuthenticationResult { IsAuthenticated = false, Message = message };
        }
    }
}
