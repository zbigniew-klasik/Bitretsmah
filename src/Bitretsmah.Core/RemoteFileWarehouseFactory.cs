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
            // TODO: make it async
            // TODO: unit tests load stores

            var warehouse = new RemoteFileWarehouse(_remoteFileStoreFactory);
            warehouse.LoadStores().Wait();
            return warehouse;
        }
    }
}