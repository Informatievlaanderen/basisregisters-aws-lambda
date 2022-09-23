namespace Be.Vlaanderen.Basisregisters.Aws.Lambda
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.Lambda.Core;
    using Amazon.Lambda.Serialization.Json;
    using Amazon.Lambda.SQSEvents;
    using Extensions;
    using Microsoft.Extensions.DependencyInjection;

    public abstract class FunctionBase
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly IServiceProvider _serviceProvider;

        public IConfigureService ConfigureService { get; set; }

        protected FunctionBase()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            var services = new ServiceCollection();
            _serviceProvider = ConfigureFunctionServices(services);

            ConfigureService = _serviceProvider.GetRequiredService<IConfigureService>();
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
            var messageData = sqsJsonMessage.Map() ?? throw new ArgumentException("SQS message data is null.");

            var messageHandler = _serviceProvider.GetRequiredService<IMessageHandler>();
            await messageHandler.HandleMessage(messageData, messageMetadata, _cancellationTokenSource.Token);
        }
    }
}
