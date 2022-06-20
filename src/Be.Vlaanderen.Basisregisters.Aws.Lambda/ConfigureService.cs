namespace Be.Vlaanderen.Basisregisters.Aws.Lambda
{
    using System.IO;
    using Microsoft.Extensions.Configuration;

    public class ConfigureService : IConfigureService
    {
        public IEnvironmentService EnvironmentService { get; }

        public ConfigureService(IEnvironmentService environmentService)
        {
            EnvironmentService = environmentService;
        }

        public IConfiguration Configuration => new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{EnvironmentService.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }
}
