using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationApi.Domain.Models.ENTITY;
public class Resource
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public string UpdatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Boolean IsPublic  { get; set; }

    [ForeignKey("Modules")]
    public int? ModuleId { get; set; }
    public Modules? Module { get; set; }
    
    [ForeignKey("Pages")]
    public int? PageId { get; set; }
    public Pages? Page { get; set; }

    public ICollection<IPRestrictions> IPRestrictions { get; set; } = new List<IPRestrictions>();

    
}