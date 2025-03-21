namespace AuthenticationApi.Domain.Models.DTO
{

    public class EmailVerifyRequest
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string TokenType { get; set; }

    }
}