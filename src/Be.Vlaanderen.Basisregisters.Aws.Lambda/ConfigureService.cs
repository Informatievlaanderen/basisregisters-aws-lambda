namespace Be.Vlaanderen.Basisregisters.Aws.Lambda
{
    using Microsoft.Extensions.Configuration;

    public class ConfigureService : IConfigureService
    {
        public IEnvironmentService EnvironmentService { get; }
        public IConfiguration Configuration { get; }

        public ConfigureService(IEnvironmentService environmentService, IConfiguration configuration)
        {
            Configuration = configuration;
            EnvironmentService = environmentService;
        }
    }
}
