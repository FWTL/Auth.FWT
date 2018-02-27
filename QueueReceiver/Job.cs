using Microsoft.Azure.WebJobs;
using StackExchange.Redis;

namespace QueueReceiver
{
    public class Job
    {
        private IDatabase _redis;

        public Job(IDatabase redis)
        {
            _redis = redis;
        }

        public static void ProcessQueueMessage([QueueTrigger("xyz")] string value)
        {
            
        }
    }
}