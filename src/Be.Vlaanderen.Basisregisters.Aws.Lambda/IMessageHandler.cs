namespace Be.Vlaanderen.Basisregisters.Aws.Lambda
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IMessageHandler
    {
        Task HandleMessage(object? messageData, MessageMetadata messageMetadata, CancellationToken cancellationToken);
    }
}
