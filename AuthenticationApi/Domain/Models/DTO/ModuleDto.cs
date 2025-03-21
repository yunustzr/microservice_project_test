namespace AuthenticationApi.Domain.Models.DTO;
public class ModuleDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
    public byte OrderIndex { get; set; }
    public List<PageDto> Pages { get; set; } = new();
}
