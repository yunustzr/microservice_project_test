using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AuthenticationApi.Domain.Models.ENTITY;


public class IPRestrictions
{
    [Key]
    public int Id { get; set; }
    public string IPAddress { get; set; }
    public string Subnet { get; set; }
    public bool  IsAllowed { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    [ForeignKey("Resource")]
    public int ResourceId { get; set; }
    public Resource Resource { get; set; }
    
}
