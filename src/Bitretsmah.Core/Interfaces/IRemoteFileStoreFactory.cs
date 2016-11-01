using System.Collections.Generic;

namespace Bitretsmah.Core.Interfaces
{
    public interface IRemoteFileStoreFactory
    {
        IList<IRemoteFileStore> GetAll();
    }
}