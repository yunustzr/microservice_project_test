using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationApi.Domain.Models.ENTITY;

public class Pages
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string RoutePath  { get; set; }
    public int OrderIndex  { get; set; }
    public string CreatedBy  { get; set; }
    public DateTime CreatedAt  { get; set; }
    public string UpdatedBy  { get; set; }
    public DateTime UpdatedAt  { get; set; }
    
    [ForeignKey("Module")]
    public int ModulesId { get; set; }
    public Modules Modules { get; set; }
        
}
