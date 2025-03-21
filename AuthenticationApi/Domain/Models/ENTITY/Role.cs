using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Domain.Models.ENTITY;
public class Role
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsDefault  { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? DeletedToken { get; set; }
    public List<RolePolicy> RolePolicies { get; set; } = new List<RolePolicy>();
    public List<UserRoles> UserRole {get;set;} = new List<UserRoles>();
}