using HashidsNet;

namespace FWT.Core.Services.Hash
{
    public interface IShortenService
    {
        Hashids Hash<TModel>();
    }
}
