using Newtonsoft.Json;

namespace Auth.FWT.Core.Services.ServiceBus
{
    public class KeyValuePair<TValue>
    {
        public KeyValuePair()
        {
        }

        public KeyValuePair(string key, TValue value)
        {
            Key = key;
            Value = JsonConvert.SerializeObject(value);
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}