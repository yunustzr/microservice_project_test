using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Domain.Models.ENTITY;

public class Modules
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
    public byte  OrderIndex  { get; set; }
    public string CreatedBy  { get; set; }
    public DateTime CreatedAt  { get; set; }
    public string UpdatedBy  { get; set; }
    public DateTime UpdatedAt  { get; set; }
    
    
    
}
