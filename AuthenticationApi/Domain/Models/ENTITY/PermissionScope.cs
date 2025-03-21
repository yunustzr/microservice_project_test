using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationApi.Domain.Models.ENTITY;

public class PermissionScope
{ 
    [Key] // Birincil anahtar tanımı
    public int Id { get; set; }

    [ForeignKey("Permission")]
    public int PermissionId { get; set; }
    public Permission Permission { get; set; }

    [ForeignKey("Scope")]
    public int ScopeId { get; set; }
    public Scope Scope { get; set; }
}
