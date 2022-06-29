namespace Be.Vlaanderen.Basisregisters.Aws.Lambda
{
    using Amazon.Lambda.SQSEvents;

    public class MessageMetadata
    {
        public SQSEvent.SQSMessage? Message { get; set; }
        public string? MessageGroupId { get; set; }
    }
}
