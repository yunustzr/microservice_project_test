using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationApi.Domain.Models.ENTITY;

public class PolicyVersion
{
   [Key]
    public int Id { get; set; }
   
    public string Version { get; set; } // "1.0.2"
    public DateTime EffectiveDate { get; set; }
    public string Changes { get; set; } // JSON diff
    
    [ForeignKey("Policy")]
    public int PolicyId { get; set; }
    public Policy Policy { get; set; } = null!;

}
