using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bitretsmah.Core.Interfaces
{
    public interface IRemoteFileStoreFactory
    {
        Task<IList<IRemoteFileStore>> GetAll();
    }
}