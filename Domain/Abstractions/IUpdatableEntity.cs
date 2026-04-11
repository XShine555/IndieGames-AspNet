namespace Domain.Contracts
{
    public interface IUpdatableEntity
    {
        DateTime UpdatedAt { get; set; }
    }
}
