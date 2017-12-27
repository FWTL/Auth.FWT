using System.Net;
using System.Web.Http;
using Auth.FWT.API.Models;
using Swashbuckle.Swagger.Annotations;

namespace Auth.FWT.API.Controllers.Login
{
    public class LoginController : ApiController
    {
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ValidationResultModel))]
        public bool Login(string phoneNumber, string code)
        {
            return false;
        }
    }
}