namespace Be.Vlaanderen.Basisregisters.Aws.Lambda.Tests
{
    using Amazon.Lambda.SQSEvents;
    using Amazon.Lambda.TestUtilities;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
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
                x => {
                    receivedMessageGroupId = x;
                    return Task.CompletedTask;
                },
                x =>
                {
                    receivedMessage = x;
                    return Task.CompletedTask;
                });
            await function.Handler(JObject.FromObject(sqsEvent), context);

            Assert.Contains(serializedMessage, logger.Buffer.ToString());
            Assert.Equal(messageGroupId, receivedMessageGroupId);
            Assert.NotNull(receivedMessage);
            Assert.IsType<MockMessage>(receivedMessage);
            Assert.Equal(Guid.Parse("020c172d-eceb-42ac-b9e4-ab7f3be23999"), ((MockMessage)receivedMessage!).Id);
            Assert.Equal("Mock", ((MockMessage)receivedMessage!).Name);
        }

        [Fact]
        public async Task TestGracefulShutdown()
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
                    },
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
                Logger = logger,
                RemainingTime = TimeSpan.FromSeconds(2)
            };

            var function = new TestFunction(
                x => Task.CompletedTask,
                async x =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(2));
                },
                () => new LambdaOptions
                {
                    GracefulShutdownSeconds = 1
                }
                );
            try
            {
                await function.Handler(JObject.FromObject(sqsEvent), context);

                throw new Exception("Handler should be cancelled");
            }
            catch (OperationCanceledException)
            {
                Assert.True(true);
            }
        }
    }

    public class MockMessage
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
    }
}
