
namespace AuthenticationApi.Domain.Models.ENTITY;

public class Condition
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ConditionType Type { get; set; } // Enum: Time, IP, Location, etc.
    public string Expression { get; set; } // JSON veya DSL formatında
}


public enum ConditionType
{
    TimeBased,
    IPRange,
    Geographic,
    Custom
}