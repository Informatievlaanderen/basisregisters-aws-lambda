namespace Be.Vlaanderen.Basisregisters.Aws.Lambda
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
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

        public object? Map(IEnumerable<Assembly> messageAssemblies)
        {
            var serializer = Newtonsoft.Json.JsonSerializer.CreateDefault();
            serializer.CheckAdditionalContent = true;
            return Map(messageAssemblies, serializer);
        }

        public object? Map(IEnumerable<Assembly> messageAssemblies, Newtonsoft.Json.JsonSerializer serializer)
        {
            ArgumentNullException.ThrowIfNull(serializer);

            var assembly = GetAssemblyNameContainingType(messageAssemblies, Type);
            var type = assembly?.GetType(Type);

            using (var streamReader = new StringReader(Data))
            using (var reader = new JsonTextReader(streamReader))
            {
                return serializer.Deserialize(reader, type);
            }
        }

        public static SqsJsonMessage Create<T>([DisallowNull] T message, JsonSerializer serializer)
        {
            ArgumentNullException.ThrowIfNull(message);
            ArgumentNullException.ThrowIfNull(serializer);

            var data = serializer.Serialize(message);
            return new SqsJsonMessage(message.GetType().FullName!, data);
        }

        public static SqsJsonMessage Create<T>([DisallowNull] T message, Newtonsoft.Json.JsonSerializer serializer)
        {
            ArgumentNullException.ThrowIfNull(message);
            ArgumentNullException.ThrowIfNull(serializer);

            var data = serializer.Serialize(message);
            return new SqsJsonMessage(message.GetType().FullName!, data);
        }

        private static Assembly? GetAssemblyNameContainingType(IEnumerable<Assembly> messageAssemblies, string typeName)
        {
            ArgumentNullException.ThrowIfNull(messageAssemblies);

            return messageAssemblies
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
}
