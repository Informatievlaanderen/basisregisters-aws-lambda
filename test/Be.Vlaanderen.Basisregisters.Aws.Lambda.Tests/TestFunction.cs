namespace Be.Vlaanderen.Basisregisters.Aws.Lambda.Tests
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    public class TestFunction : FunctionBase
    {
        public Action<string> MessageGroupIdAction { get; set; }

        public TestFunction(Action<string> messageGroupIdAction)
        {
            MessageGroupIdAction = messageGroupIdAction;
        }

        protected override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IMessageHandler>(x => new TestMessageHandler(MessageGroupIdAction));

            return services.BuildServiceProvider();
        }
    }
}
