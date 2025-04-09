namespace Be.Vlaanderen.Basisregisters.Aws.Lambda
{
    public sealed class PingEvent
    {
        public required string Ping { get; set; }

        public override string ToString() => Ping;
    }
}
