using Auth.FWT.Core.Services.Telegram;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Auth.FWT.Infrastructure.Json
{
    public class TelegramMediaConverter : JsonConverter
    {
        private static Dictionary<string, Type> _types = new Dictionary<string, Type>()
        {
            { typeof(PhotoInfo).FullName, typeof(PhotoInfo) },
            { typeof(MediaInfo).FullName, typeof(MediaInfo) },
        };

        static TelegramMediaConverter()
        {
        }

        public override bool CanWrite => false;

        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            return objectType.FullName == typeof(ITelegramMedia).FullName;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new InvalidOperationException("Use default serialization.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.Null)
            {
                var jsonObject = JObject.Load(reader);
                var typeName = jsonObject["_"].Value<string>();
                Type type = _types[typeName];
                return jsonObject.ToObject(type);
            }

            return null;
        }
    }
}