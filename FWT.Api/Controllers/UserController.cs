using FWT.Api.Controllers.User;
using FWT.Core.CQRS;
using FWT.Core.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FWT.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private ICommandDispatcher _commandDispatcher;
        private IQueryDispatcher _queryDispatcher;

        public UserController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        [HttpGet]
        [Route("/Me")]
        public async Task<GetMe.Result> GetMe(string phone)
        {
            string hashedPhoneId = HashHelper.GetHash(phone);
            return await _queryDispatcher.DispatchAsync<GetMe.Query, GetMe.Result>(new GetMe.Query() {
                UserHashId = hashedPhoneId
            });
        }
    }
}