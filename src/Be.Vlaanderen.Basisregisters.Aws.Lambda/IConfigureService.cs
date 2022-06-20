namespace Be.Vlaanderen.Basisregisters.Aws.Lambda
{
    using Microsoft.Extensions.Configuration;

    public interface IConfigureService
    {
        IConfiguration Configuration { get; }
    }
}
