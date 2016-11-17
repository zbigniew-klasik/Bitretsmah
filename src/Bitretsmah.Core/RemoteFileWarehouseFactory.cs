using System;

namespace Bitretsmah.Core
{
    public interface IRemoteFileWarehouseFactory
    {
        IRemoteFileWarehouse Create();
    }

    public class RemoteFileWarehouseFactory : IRemoteFileWarehouseFactory
    {
        public IRemoteFileWarehouse Create()
        {
            throw new NotImplementedException();
        }
    }
}