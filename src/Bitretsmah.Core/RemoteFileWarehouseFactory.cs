using System;
using Bitretsmah.Core.Interfaces;

namespace Bitretsmah.Core
{
    public interface IRemoteFileWarehouseFactory
    {
        IRemoteFileWarehouse Create();
    }

    public class RemoteFileWarehouseFactory : IRemoteFileWarehouseFactory
    {
        private readonly IRemoteFileStoreFactory _remoteFileStoreFactory;

        public RemoteFileWarehouseFactory(IRemoteFileStoreFactory remoteFileStoreFactory)
        {
            _remoteFileStoreFactory = remoteFileStoreFactory;
        }

        public IRemoteFileWarehouse Create()
        {
            return new RemoteFileWarehouse(_remoteFileStoreFactory);
        }
    }
}