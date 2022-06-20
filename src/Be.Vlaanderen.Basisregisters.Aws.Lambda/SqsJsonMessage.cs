namespace Be.Vlaanderen.Basisregisters.Aws.Lambda
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using Extensions;
    using Newtonsoft.Json;
    using JsonSerializer = Amazon.Lambda.Serialization.Json.JsonSerializer;

    public class SqsJsonMessage
    {
        public string Type { get; set; }
        public string Data { get; set; }

        public SqsJsonMessage()
            : this(string.Empty, string.Empty)
        { }

        public SqsJsonMessage(string type, string data)
        {
            Type = type;
            Data = data;
        }

        public object? Map()
        {
            var assembly = GetAssemblyNameContainingType(Type);
            var type = assembly?.GetType(Type);

            return JsonConvert.DeserializeObject(Data, type!);
        }

        public static SqsJsonMessage Create<T>([DisallowNull] T message, JsonSerializer serializer)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            string data = serializer.Serialize(message);
            return new SqsJsonMessage(message.GetType().FullName!, data);
        }

        private static Assembly? GetAssemblyNameContainingType(string typeName) => AppDomain.CurrentDomain.GetAssemblies()
            .Select(x => new
            {
                Assembly = x,
                Type = x.GetType(typeName, false, true)
            })
            .Where(x => x.Type != null)
            .Select(x => x.Assembly)
            .FirstOrDefault();
    }
}
