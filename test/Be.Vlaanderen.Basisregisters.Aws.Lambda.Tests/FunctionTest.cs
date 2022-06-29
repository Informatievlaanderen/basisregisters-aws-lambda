namespace Be.Vlaanderen.Basisregisters.Aws.Lambda.Tests
{
    using Amazon.Lambda.SQSEvents;
    using Amazon.Lambda.TestUtilities;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class FunctionTest
    {
        [Fact]
        public async Task TestSqsLambdaFunction()
        {
            const string messageGroupId = nameof(TestSqsLambdaFunction);
            const string serializedMessage = "{\"Type\":\"BuildingRegistry.Api.BackOffice.Abstractions.Building.Requests.SqsPlanBuildingRequest\",\"Data\":\"{\\\"GeometriePolygoon\\\":\\\"\\\"}\"}";

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
            var function = new TestFunction(x => { receivedMessageGroupId = x; });
            await function.FunctionHandler(sqsEvent, context);

            Assert.Contains(serializedMessage, logger.Buffer.ToString());
            Assert.Equal(receivedMessageGroupId, messageGroupId);
        }
    }
}
