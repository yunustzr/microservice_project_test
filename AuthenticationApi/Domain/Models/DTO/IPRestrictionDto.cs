namespace AuthenticationApi.Domain.Models.DTO;

public class IPRestrictionDto
{
    public string IPAddress { get; set; }
    public string Subnet { get; set; }
    public bool IsAllowed { get; set; }
}
