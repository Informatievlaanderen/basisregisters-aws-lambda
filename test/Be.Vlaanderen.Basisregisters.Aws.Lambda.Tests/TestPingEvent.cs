namespace Be.Vlaanderen.Basisregisters.Aws.Lambda.Tests
{
    using System.Threading.Tasks;
    using Amazon.Lambda.TestUtilities;
    using Newtonsoft.Json.Linq;
    using Xunit;

    public class TestPingEvent
    {
        [Fact]
        public async Task TestPingEventLogged()
        {
            var function = new TestFunction(x => Task.CompletedTask, x => Task.CompletedTask);

            var lambdaContext = new TestLambdaContext();
            await function.Handler(JObject.Parse("{\"Ping\":\"pong\"}"), lambdaContext);

            Assert.Contains("pong", ((TestLambdaLogger)lambdaContext.Logger).Buffer.ToString());
        }
    }
}
