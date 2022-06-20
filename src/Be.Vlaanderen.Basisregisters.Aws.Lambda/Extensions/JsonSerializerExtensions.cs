namespace Be.Vlaanderen.Basisregisters.Aws.Lambda.Extensions
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;
    using JsonSerializer = Amazon.Lambda.Serialization.Json.JsonSerializer;

    public static class JsonSerializerExtensions
    {
        public static string Serialize(
            this JsonSerializer jsonSerializer,
            object value)
        {
            var newtonSoftJsonSerializer = new Newtonsoft.Json.JsonSerializer();
            var stringWriter = new StringWriter(new StringBuilder(256), CultureInfo.InvariantCulture);
            using (var jsonTextWriter = new JsonTextWriter(stringWriter))
            {
                jsonTextWriter.Formatting = newtonSoftJsonSerializer.Formatting;
                newtonSoftJsonSerializer.Serialize(jsonTextWriter, value, value.GetType());
            }

            return stringWriter.ToString();
        }

        public static T? Deserialize<T>(this JsonSerializer serializer, string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            var stream = StringToStream(message);

            return serializer.Deserialize<T>(stream);
        }

        private static Stream StringToStream(string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            return new MemoryStream(bytes);
        }
    }
}
