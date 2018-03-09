using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Auth.FWT.Core.Providers;
using Auth.FWT.CQRS;

namespace Auth.FWT.API.Controllers.File
{
    public class FileController : ApiController
    {
        private ICommandDispatcher _commandDispatcher;
        private IQueryDispatcher _queryDispatcher;
        private IUserProvider _userProvider;

        public FileController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher, IUserProvider userProvider)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _userProvider = userProvider;
        }

        [Authorize]
        [HttpGet]
        [Route("api/file")]
        public async Task<HttpResponseMessage> File(long accessHash, long id, int version, int size)
        {
            var result = await _queryDispatcher.Dispatch<GetFile.Query, GetFile.Result>(new GetFile.Query()
            {
                AccessHash = accessHash,
                Id = id,
                Version = version,
                Size = size
            });

            HttpResponseMessage xyz = new HttpResponseMessage(HttpStatusCode.OK);
            xyz.Content = new StringContent(System.Text.Encoding.Default.GetString(result.File.Bytes));
            xyz.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            xyz.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            xyz.Content.Headers.ContentDisposition.FileName = "test.jpg";
            return xyz;
        }
    }
}