namespace Be.Vlaanderen.Basisregisters.Aws.Lambda.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class TestMessageHandler : IMessageHandler
    {
        public Action<string> MessageGroupIdAction { get; set; }

        public TestMessageHandler(Action<string> messageGroupIdAction)
        {
            MessageGroupIdAction = messageGroupIdAction;
        }
        
        public async Task HandleMessage(object? messageData, IMessageMetadata messageMetadata, CancellationToken cancellationToken)
        {
            await Task.Yield();
            var message = messageMetadata.Message;
            var messageGroupId = message?.Attributes["MessageGroupId"];
            MessageGroupIdAction.Invoke(messageGroupId);
        }
    }
}
