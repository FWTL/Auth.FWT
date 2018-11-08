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
        public async Task SendCode(string phoneNumber)
        {
            await _commandDispatcher.DispatchAsync(new SendCode.Command(phoneNumber));
        }
    }
}