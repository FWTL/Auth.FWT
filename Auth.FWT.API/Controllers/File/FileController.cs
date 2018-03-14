using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Auth.FWT.API.SwaggerExtensions;
using Auth.FWT.Core.Helpers;
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
        [SwaggerFileResponse(HttpStatusCode.OK)]
        public async Task<HttpResponseMessage> File(long accessHash, long id, int version, int size)
        {
            var result = await _queryDispatcher.Dispatch<GetFile.Query, byte[]>(new GetFile.Query()
            {
                AccessHash = accessHash,
                Id = id,
                Version = version,
                Size = size
            });

            return ResponseHelper.FileResult(Guid.NewGuid().ToString("n"), result);
        }

        [Authorize]
        [HttpGet]
        [Route("api/photo")]
        [SwaggerFileResponse(HttpStatusCode.OK)]
        public async Task<HttpResponseMessage> Photo(int localId, long secret, long volumeId)
        {
            var result = await _queryDispatcher.Dispatch<GetPhoto.Query, byte[]>(new GetPhoto.Query()
            {
                LocalId = localId,
                Secret = secret,
                VolumeId = volumeId,
            });

            return ResponseHelper.FileResult(Guid.NewGuid().ToString("n"), result);
        }
    }
}