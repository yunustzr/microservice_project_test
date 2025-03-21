namespace AuthenticationApi.Domain.Models.DTO;

public class PageDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string RoutePath { get; set; }
    public List<IPRestrictionDto> IPRestrictions { get; set; } = new();
}