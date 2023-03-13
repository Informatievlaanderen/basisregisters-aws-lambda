namespace Be.Vlaanderen.Basisregisters.Aws.Lambda.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class TestMessageHandler : IMessageHandler
    {
        public Func<string, Task> MessageGroupIdAction { get; set; }
        public Func<object?, Task> MessageDataAction { get; }

        public TestMessageHandler(
            Func<string, Task> messageGroupIdAction,
            Func<object?, Task> messageDataAction)
        {
            MessageGroupIdAction = messageGroupIdAction;
            MessageDataAction = messageDataAction;
        }

        public async Task HandleMessage(object? messageData, MessageMetadata messageMetadata, CancellationToken cancellationToken)
        {
            await Task.Yield();
            var message = messageMetadata.Message;
            var messageGroupId = message?.Attributes["MessageGroupId"];
            await MessageGroupIdAction.Invoke(messageGroupId ?? string.Empty);
            await MessageDataAction.Invoke(messageData);
        }
    }
}
