using OpenTl.ClientApi;
using System.Threading.Tasks;

namespace FWT.Core.Services.Telegram
{
    public interface ITelegramService
    {
        Task<IClientApi> Build(string hash);
    }
}