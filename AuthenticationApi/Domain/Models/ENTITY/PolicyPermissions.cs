using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationApi.Domain.Models.ENTITY;
public class PolicyPermissions
{
    [Key]
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }

    public string UpdatedBy { get; set; }
    public int IsActive { get; set; }
    public string CreatedBy { get; set; }
    
    [ForeignKey("Policy")]
    public int PolicyId { get; set; }
    public Policy Policy { get; set; } = null!;

    [ForeignKey("Permission")]
    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
  
}