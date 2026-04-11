namespace Application.Abstractions
{
    public interface IEventBus
    {
        Task PublishAsync<T>(T message, CancellationToken cancellationToken);
    }
}
