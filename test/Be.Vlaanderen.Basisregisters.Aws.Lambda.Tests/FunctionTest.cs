namespace Be.Vlaanderen.Basisregisters.Aws.Lambda.Tests
{
    using Amazon.Lambda.SQSEvents;
    using Amazon.Lambda.TestUtilities;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class FunctionTest
    {
        [Fact]
        public async Task TestSqsLambdaFunction()
        {


            const string messageGroupId = nameof(TestSqsLambdaFunction);
            const string serializedMessage =
                "{\"Type\":\"Be.Vlaanderen.Basisregisters.Aws.Lambda.Tests.MockMessage\",\"Data\":\"{\\\"Id\\\":\\\"020c172d-eceb-42ac-b9e4-ab7f3be23999\\\",\\\"Name\\\":\\\"Mock\\\"}\"}";

            var sqsEvent = new SQSEvent
            {
                Records = new List<SQSEvent.SQSMessage>
                {
                    new SQSEvent.SQSMessage
                    {
                        Attributes = new Dictionary<string, string>
                        {
                            ["MessageGroupId"] = messageGroupId
                        },
                        Body = serializedMessage
                    }
                }
            };

            var logger = new TestLambdaLogger();
            var context = new TestLambdaContext
            {
                Logger = logger
            };

            var receivedMessageGroupId = "";
            object? receivedMessage = null;
            var function = new TestFunction(
                x => { receivedMessageGroupId = x; },
                x => { receivedMessage = x; });
            await function.Handler(sqsEvent, context);

            Assert.Contains(serializedMessage, logger.Buffer.ToString());
            Assert.Equal(receivedMessageGroupId, messageGroupId);
            Assert.NotNull(receivedMessage);
            Assert.IsType<MockMessage>(receivedMessage);
            Assert.Equal(Guid.Parse("020c172d-eceb-42ac-b9e4-ab7f3be23999"), ((MockMessage)receivedMessage!).Id);
            Assert.Equal("Mock", ((MockMessage)receivedMessage!).Name);
        }
    }

    public class MockMessage
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
