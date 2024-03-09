namespace Be.Vlaanderen.Basisregisters.Aws.Lambda
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.Lambda.Core;
    using Amazon.Lambda.SQSEvents;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public abstract class FunctionBase
    {
        private readonly IEnumerable<Assembly> _messageAssemblies;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        protected IServiceProvider ServiceProvider { get; }
        protected IConfigureService ConfigureService { get; }

        /// <param name="messageAssemblies">The assemblies in which message contracts reside.</param>
        /// <param name="jsonSerializerSettings">The newtonsoft JsonSerializer settings.</param>
        protected FunctionBase(
            IEnumerable<Assembly> messageAssemblies,
            JsonSerializerSettings? jsonSerializerSettings = null)
        {
            _messageAssemblies = messageAssemblies;
            _cancellationTokenSource = new CancellationTokenSource();

            var services = new ServiceCollection();
            ServiceProvider = ConfigureFunctionServices(services);

            _jsonSerializerSettings = jsonSerializerSettings ?? new JsonSerializerSettings();

            ConfigureService = ServiceProvider.GetRequiredService<IConfigureService>();
        }

        protected abstract IServiceProvider ConfigureServices(IServiceCollection services);

        protected virtual LambdaOptions LoadOptions()
        {
            var options = new LambdaOptions();
            ConfigureService.Configuration.Bind(options);
            return options;
        }

        public async Task Handler(JObject @event, ILambdaContext context)
        {
            var options = LoadOptions();
            if (options.GracefulShutdownSeconds > 0)
            {
                if (options.GracefulShutdownSeconds >= context.RemainingTime.TotalSeconds)
                {
                    throw new InvalidOperationException(
                        $"Configured {nameof(options.GracefulShutdownSeconds)} must be smaller than maximum Lambda execution time. It's currently configured to start at {options.GracefulShutdownSeconds} seconds before termination, while there's only {context.RemainingTime.TotalMinutes} seconds left.");
                }

                var gracefulShutdownTimeSpan =
                    TimeSpan.FromSeconds(context.RemainingTime.TotalSeconds - options.GracefulShutdownSeconds);
                _cancellationTokenSource.CancelAfter(gracefulShutdownTimeSpan);
            }

            context.Logger.LogInformation($"Receiving event of type {@event.GetType().FullName}.");

            if (@event.ContainsKey("Records") || @event.ContainsKey("records"))
            {
                var sqsEvent = @event.ToObject<SQSEvent>();
                if (sqsEvent is not null)
                {
                    foreach (var record in sqsEvent.Records)
                    {
                        _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                        await ProcessMessage(record, context);
                    }
                }
            }
            else if (@event.ContainsKey("Ping") || @event.ContainsKey("ping"))
            {
                var pingMessage = @event.ToObject<PingEvent>();
                if (pingMessage is not null)
                {
                    context.Logger.LogDebug($"Ping: {pingMessage} received.");
                }
            }
            else
            {
                throw new ArgumentException($"Unsupported JObject type: {@event.Type}");
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

            var sqsJsonMessage = JsonConvert.DeserializeObject<SqsJsonMessage>(record.Body, _jsonSerializerSettings);
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
            var messageData = sqsJsonMessage.Map(_messageAssemblies, _jsonSerializerSettings) ??
                              throw new ArgumentException("SQS message data is null.");

            var messageHandler = ServiceProvider.GetRequiredService<IMessageHandler>();
            await messageHandler.HandleMessage(messageData, messageMetadata, _cancellationTokenSource.Token);
        }
    }
}
