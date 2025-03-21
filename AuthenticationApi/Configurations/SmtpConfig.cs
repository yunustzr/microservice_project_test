namespace AuthenticationApi.Configurations;

public class SmtpConfig
{
    public string Host  { get; set; }
    public int Port { get; set; }
    
    public string User { get; set; }
    public string Pass { get; set; }
    public string From { get; set; }
        
}
