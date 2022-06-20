namespace Be.Vlaanderen.Basisregisters.Aws.Lambda
{
    using System;

    public class EnvironmentService : IEnvironmentService
    {
        public EnvironmentService()
        {
            EnvironmentName = Environment.GetEnvironmentVariable(EnvironmentVariables.AspnetCoreEnvironment) ?? Environments.Production;
        }

        public string EnvironmentName { get; set; }
    }
}
