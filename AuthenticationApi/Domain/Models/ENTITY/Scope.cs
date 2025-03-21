using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationApi.Domain.Models.ENTITY;

public class Scope
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Filter { get; set; } 
    
    [ForeignKey("Resource")]
    public int ResourceId { get; set; }
    public Resource Resource { get; set; }
}
