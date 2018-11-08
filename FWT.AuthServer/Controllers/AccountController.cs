using FWT.AuthServer.Controllers.Account;
using FWT.Core.CQRS;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FWT.AuthServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private ICommandDispatcher _commandDispatcher;
        private IQueryDispatcher _queryDispatcher;

        public AccountController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        [HttpPost]
        [Route("Sendcode")]
        public async Task<string> SendCode(string phoneNumber)
        {
            return await _queryDispatcher.DispatchAsync<SendCode.Query, string>(new SendCode.Query(phoneNumber));
        }

        [HttpPost]
        [Route("SignIn")]
        public async Task<string> SignIn(string phoneNumber, string sentCode, string code)
        {
            return await _queryDispatcher.DispatchAsync<SignIn.Query, string>(new SignIn.Query(phoneNumber, sentCode, code));
        }
    }
}