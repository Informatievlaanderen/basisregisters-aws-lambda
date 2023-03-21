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
    using Microsoft.Extensions.Configuration;
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

        protected virtual LambdaOptions LoadOptions()
        {
            var options = new LambdaOptions();
            ConfigureService.Configuration.Bind(options);
            return options;
        }
        
        protected virtual SqsJsonMessage? DeserializeSqsMessage(SQSEvent.SQSMessage record)
        {
            var serializer = new JsonSerializer();
            return serializer.Deserialize<SqsJsonMessage>(record.Body);
        }

        public async Task Handler(SQSEvent sqsEvent, ILambdaContext context)
        {
            var options = LoadOptions();
            if (options.GracefulShutdownSeconds > 0)
            {
                if (options.GracefulShutdownSeconds >= context.RemainingTime.TotalSeconds)
                {
                    throw new InvalidOperationException($"Configured {nameof(options.GracefulShutdownSeconds)} must be smaller than maximum Lambda execution time. It's currently configured to start at {options.GracefulShutdownSeconds} seconds before termination, while there's only {context.RemainingTime.TotalMinutes} seconds left.");
                }

                var gracefulShutdownTimeSpan = TimeSpan.FromSeconds(context.RemainingTime.TotalSeconds - options.GracefulShutdownSeconds);
                _cancellationTokenSource.CancelAfter(gracefulShutdownTimeSpan);
            }
            
            foreach (var record in sqsEvent.Records)
            {
                _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                await ProcessMessage(record, context);
            }
        }

        private IServiceProvider ConfigureFunctionServices(IServiceCollection services)
        {
            services.AddLogging();
            
            services.AddSingleton<IEnvironmentService, EnvironmentService>();
            services.AddSingleton<IConfigureService, ConfigureService>();

            return ConfigureServices(services);
        }

        private async Task ProcessMessage(SQSEvent.SQSMessage record, ILambdaContext context)
        {
            var logger = context.Logger;
            logger.LogDebug($"Process message: {record.Body}");
            
            var sqsJsonMessage = DeserializeSqsMessage(record);
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
