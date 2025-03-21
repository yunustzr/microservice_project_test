using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationApi.Domain.Models.ENTITY;

public class RolePolicy
{
    private const string V = "Policy";


    [Key]
    public int Id { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UpdatedBy { get; set; }
    public int IsActive { get; set; }
    

    [ForeignKey("Policy")]
    public int PolicyId { get; set; }
    public Policy Policy { get; set; } = null!;

    [ForeignKey("Role")]
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}