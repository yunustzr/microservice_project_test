namespace AuthenticationApi.Domain.Models
{
    public class RefreshToken
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public DateTime Created { get; set; }

    }
}
