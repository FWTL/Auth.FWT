using Auth.FWT.Core.CQRS;

namespace QueueReceiver
{
    public class Job
    {
        private IEventDispatcher _eventDispatcher;

        public Job(IEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }
    }
}