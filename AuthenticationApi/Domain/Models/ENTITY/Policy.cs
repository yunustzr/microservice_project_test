using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Domain.Models.ENTITY;
public class Policy
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int IsActive { get; set; }
    public string Functionality { get; set; }
    public string IsPublic { get; set; }
    public List<PolicyPermissions> PolicyPermissions { get; set; } = new List<PolicyPermissions>();
}