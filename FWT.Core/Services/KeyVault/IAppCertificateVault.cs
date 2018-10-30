using System.Threading.Tasks;

namespace FWT.Core.Services.KeyVault
{
    public interface IAppCertificateVault
    {
        Task CreateSelfSignedAsync();
    }
}
