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

        protected readonly IServiceProvider ServiceProvider;

        public IConfigureService ConfigureService { get; set; }

        protected FunctionBase()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            var services = new ServiceCollection();
            ServiceProvider = ConfigureFunctionServices(services);

            ConfigureService = ServiceProvider.GetRequiredService<IConfigureService>();
        }

        private IServiceProvider ConfigureFunctionServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddTransient<IEnvironmentService, EnvironmentService>();
            services.AddTransient<IConfigureService, ConfigureService>();

            return ConfigureServices(services);
        }

        protected abstract IServiceProvider ConfigureServices(IServiceCollection services);

        public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
        {
            foreach (var record in sqsEvent.Records)
            {
                await ProcessMessage(record, context);
            }
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

            var messageHandler = ServiceProvider.GetRequiredService<IMessageHandler>();
            await messageHandler.HandleMessage(messageData, messageMetadata, _cancellationTokenSource.Token);
        }
    }
}
