namespace Be.Vlaanderen.Basisregisters.Aws.Lambda
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using Newtonsoft.Json;

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

        public object? Map(IEnumerable<Assembly> messageAssemblies, JsonSerializerSettings jsonSerializerSettings)
        {
            var assembly = GetAssemblyNameContainingType(messageAssemblies, Type);
            var type = assembly?.GetType(Type);

            return JsonConvert.DeserializeObject(Data, type!, jsonSerializerSettings);
        }

        public static SqsJsonMessage Create<T>([DisallowNull] T message, JsonSerializerSettings jsonSerializerSettings)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (jsonSerializerSettings == null)
                throw new ArgumentNullException(nameof(jsonSerializerSettings));

            var data = JsonConvert.SerializeObject(message, jsonSerializerSettings);
            return new SqsJsonMessage(message.GetType().FullName!, data);
        }

        private static Assembly? GetAssemblyNameContainingType(IEnumerable<Assembly> messageAssemblies, string typeName)
            => messageAssemblies
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
