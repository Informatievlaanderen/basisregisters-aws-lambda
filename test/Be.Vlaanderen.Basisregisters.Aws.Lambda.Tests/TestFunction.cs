namespace Be.Vlaanderen.Basisregisters.Aws.Lambda.Tests
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    public class TestFunction : FunctionBase
    {
        public Action<string> MessageGroupIdAction { get; set; }
        public Action<object?> MessageDataAction { get; set; }

        public TestFunction(Action<string> messageGroupIdAction, Action<object?> messageDataAction)
        {
            MessageGroupIdAction = messageGroupIdAction;
            MessageDataAction = messageDataAction;
        }

        protected override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IMessageHandler>(x => new TestMessageHandler(MessageGroupIdAction, MessageDataAction));

            return services.BuildServiceProvider();
        }
    }
}
