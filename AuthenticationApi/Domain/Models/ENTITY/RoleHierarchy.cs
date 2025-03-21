using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationApi.Domain.Models.ENTITY;

public class RoleHierarchy
{
    [Key]
    public int Id { get; set; }
    public int ParentRoleId { get; set; }
    public int ChildRoleId { get; set; }
    
    [ForeignKey("ParentRoleId")]
    public Role ParentRole { get; set; }
    
    [ForeignKey("ChildRoleId")]
    public Role ChildRole { get; set; }
}
