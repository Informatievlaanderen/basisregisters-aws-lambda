using System.Collections.Generic;
using System.Reflection;

namespace Be.Vlaanderen.Basisregisters.Aws.Lambda.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;

    public class TestFunction : FunctionBase
    {
        private readonly Func<LambdaOptions>? _buildLambdaOptions;
        public Func<string, Task> MessageGroupIdAction { get; set; }
        public Func<object?, Task> MessageDataAction { get; set; }

        public TestFunction(Func<string, Task> messageGroupIdAction, Func<object?, Task> messageDataAction, Func<LambdaOptions>? buildLambdaOptions = null)
            : base(new List<Assembly> { typeof(MockMessage).Assembly })
        {
            _buildLambdaOptions = buildLambdaOptions;
            MessageGroupIdAction = messageGroupIdAction;
            MessageDataAction = messageDataAction;
        }

        protected override LambdaOptions LoadOptions()
        {
            if (_buildLambdaOptions is not null)
            {
                return _buildLambdaOptions();
            }

            return base.LoadOptions();
        }

        protected override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IMessageHandler>(x => new TestMessageHandler(MessageGroupIdAction, MessageDataAction));

            return services.BuildServiceProvider();
        }
    }
}
