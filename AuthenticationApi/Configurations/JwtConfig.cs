﻿namespace AuthenticationApi.Configurations
{
    public class JwtConfig
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpiryMinutes { get; set; }
        public int RefreshTokenExpiryDays { get; set; }
    }
}
