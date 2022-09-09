namespace Be.Vlaanderen.Basisregisters.Aws.Lambda.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class TestMessageHandler : IMessageHandler
    {
        public Action<string> MessageGroupIdAction { get; set; }
        public Action<object?> MessageDataAction { get; }

        public TestMessageHandler(
            Action<string> messageGroupIdAction,
            Action<object?> messageDataAction)
        {
            MessageGroupIdAction = messageGroupIdAction;
            MessageDataAction = messageDataAction;
        }

        public async Task HandleMessage(object? messageData, MessageMetadata messageMetadata, CancellationToken cancellationToken)
        {
            await Task.Yield();
            var message = messageMetadata.Message;
            var messageGroupId = message?.Attributes["MessageGroupId"];
            MessageGroupIdAction.Invoke(messageGroupId ?? string.Empty);
            MessageDataAction.Invoke(messageData);
        }
    }
}
