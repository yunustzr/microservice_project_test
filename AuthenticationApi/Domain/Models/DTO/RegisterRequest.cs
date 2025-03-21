using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Domain.Models.DTO
{
    public class RegisterRequest : LoginRequest
    {
        public string Username { get; set; }
        public string LastName { get; set; }
        public string VerificationCode { get; set; }        
        public bool  AcceptPrivacyPolicy { get; set; }
        public bool AcceptTerms { get; set; }

        
    }
}
