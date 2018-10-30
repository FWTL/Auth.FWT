using System.Threading.Tasks;

namespace FWT.Core.Services.KeyVault
{
    public interface IAppKeyVault
    {
        Task<string> DecryptAsync(string value);

        string Encrypt(string value);
    }
}
