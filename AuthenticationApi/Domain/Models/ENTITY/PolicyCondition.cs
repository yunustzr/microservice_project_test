using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationApi.Domain.Models.ENTITY;

public class PolicyCondition
{
    [Key] // Birincil anahtar
    public int Id { get; set; }

    [ForeignKey("Policy")]
    public int PolicyId { get; set; }
    public Policy Policy { get; set; }

    [ForeignKey("Condition")]
    public int ConditionId { get; set; }
    public Condition Condition { get; set; }
}