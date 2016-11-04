using Bitretsmah.Core.Models;

namespace Bitretsmah.Core.Interfaces
{
    public interface IRemoteFileStore
    {
        string StoreId { get; }

        Quota Quota { get; }
    }
}