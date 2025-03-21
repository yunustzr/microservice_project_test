
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationApi.Domain.Models.ENTITY;

public class UserRoles
{
    [Key]
    public int Id { get; set; }
    public DateTime AssignedAt { get; set; }

    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    [ForeignKey("Role")]
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    // Yeni eklenen alan: Kullanıcıya rol verilirken ilişkilendirilen evrak referansı
    public int? DocumentReferenceId { get; set; }
    [ForeignKey("DocumentReferenceId")]
    public DocumentReference? DocumentReference { get; set; }
}
