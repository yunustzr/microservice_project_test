namespace AuthenticationApi.Domain.Models.ENTITY;

public class Attributes
{
    public int Id { get; set; }
    public EntityType EntityType { get; set; } // Enum: User, Resource
    public Guid EntityId { get; set; } // User.Id veya Resource.Id
    public string Key { get; set; } // "Department", "SecurityLevel"
    public string Value { get; set; } // "Finance", "5"
}


public enum EntityType
{
    User,
    Resource
}