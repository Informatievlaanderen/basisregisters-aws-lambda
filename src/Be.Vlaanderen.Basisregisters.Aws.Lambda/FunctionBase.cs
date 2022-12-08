namespace Be.Vlaanderen.Basisregisters.Aws.Lambda
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.Lambda.Core;
    using Amazon.Lambda.Serialization.Json;
    using Amazon.Lambda.SQSEvents;
    using Extensions;
    using Microsoft.Extensions.DependencyInjection;

    public abstract class FunctionBase
    {
        private readonly IEnumerable<Assembly> _messageAssemblies;
        private readonly CancellationTokenSource _cancellationTokenSource;

        protected IServiceProvider ServiceProvider { get; }

        public IConfigureService ConfigureService { get; set; }

        /// <param name="messageAssemblies">The assemblies in which message contracts reside.</param>
        protected FunctionBase(IEnumerable<Assembly> messageAssemblies)
        {
            _messageAssemblies = messageAssemblies;
            _cancellationTokenSource = new CancellationTokenSource();

            var services = new ServiceCollection();
            ServiceProvider = ConfigureFunctionServices(services);

            ConfigureService = ServiceProvider.GetRequiredService<IConfigureService>();
        }

        protected abstract IServiceProvider ConfigureServices(IServiceCollection services);

        public async Task Handler(SQSEvent sqsEvent, ILambdaContext context)
        {
            foreach (var record in sqsEvent.Records)
            {
                await ProcessMessage(record, context);
            }
        }

        private IServiceProvider ConfigureFunctionServices(IServiceCollection services)
        {
            services.AddLogging();
            
            services.AddTransient<IEnvironmentService, EnvironmentService>();
            services.AddTransient<IConfigureService, ConfigureService>();

            return ConfigureServices(services);
        }

        private async Task ProcessMessage(SQSEvent.SQSMessage record, ILambdaContext context)
        {
            var logger = context.Logger;
            logger.LogDebug($"Process message: {record.Body}");

            var serializer = new JsonSerializer();
            var sqsJsonMessage = serializer.Deserialize<SqsJsonMessage>(record.Body);
            if (sqsJsonMessage is not null)
            {
                var groupId = record.Attributes["MessageGroupId"];
                await ProcessSqsJsonMessage(sqsJsonMessage, new MessageMetadata
                {
                    Message = record,
                    MessageGroupId = groupId,
                    Logger = context.Logger
                });
            }

            logger.LogDebug($"Processed message: {record.Body}");
        }

        private async Task ProcessSqsJsonMessage(SqsJsonMessage sqsJsonMessage, MessageMetadata messageMetadata)
        {
            var messageData = sqsJsonMessage.Map(_messageAssemblies) ?? throw new ArgumentException("SQS message data is null.");

            var messageHandler = ServiceProvider.GetRequiredService<IMessageHandler>();
            await messageHandler.HandleMessage(messageData, messageMetadata, _cancellationTokenSource.Token);
        }
    }
}
