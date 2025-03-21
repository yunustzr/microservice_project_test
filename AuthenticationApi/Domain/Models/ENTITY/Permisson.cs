using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationApi.Domain.Models.ENTITY;
public class Permission
{
  [Key]
  public int Id { get; set; }
  public string Name { get; set; }
  public string Description { get; set; }
  public string CreatedBy { get; set; }
  public DateTime? CreatedAt { get; set; }
  public string UpdatedBy { get; set; }
  public DateTime? UpdatedAt { get; set; }


  [ForeignKey("Operation")]
  public int OperationId { get; set; }
  public Operation Operation { get; set; }

  [ForeignKey("Resource")]
  public int ResourceId { get; set; }
  public Resource Resource { get; set; }


}