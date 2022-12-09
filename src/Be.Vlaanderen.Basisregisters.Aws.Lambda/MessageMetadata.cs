namespace Be.Vlaanderen.Basisregisters.Aws.Lambda
{
    using Amazon.Lambda.Core;
    using Amazon.Lambda.SQSEvents;

    public class MessageMetadata
    {
        public SQSEvent.SQSMessage? Message { get; set; }
        public string? MessageGroupId { get; set; }
        public ILambdaLogger? Logger { get; set; }
    }
}
