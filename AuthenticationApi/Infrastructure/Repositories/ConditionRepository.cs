using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Infrastructure;


public interface IConditionRepository : IRepository<Condition>
{
}

public class ConditionRepository : Repository<Condition>, IConditionRepository
{
    public ConditionRepository(AppDbContext context) : base(context) { }
}
