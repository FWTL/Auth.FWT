using System;
using System.Threading.Tasks;
using System.Web.Http;
using Auth.FWT.API.Controllers.Job.Index;
using Auth.FWT.Core.Providers;
using Auth.FWT.CQRS;

namespace Auth.FWT.API.Controllers.Job
{
    public class JobController : ApiController
    {
        private ICommandDispatcher _commandDispatcher;
        private IQueryDispatcher _queryDispatcher;
        private IUserProvider _userProvider;

        public JobController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher, IUserProvider userProvider)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _userProvider = userProvider;
        }

        [Authorize]
        [HttpPost]
        [Route("api/job/index")]
        public async Task Index(Guid jobId)
        {
            await _commandDispatcher.Dispatch(new IndexMessages.Command()
            {
                JobId = jobId
            });
        }
    }
}