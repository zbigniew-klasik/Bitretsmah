using System.Threading.Tasks;
using Bitretsmah.Core.Interfaces;

namespace Bitretsmah.Core
{
    public interface IRemoteFileWarehouseFactory
    {
        Task<IRemoteFileWarehouse> Create();
    }

    public class RemoteFileWarehouseFactory : IRemoteFileWarehouseFactory
    {
        private readonly IRemoteFileStoreFactory _remoteFileStoreFactory;

        public RemoteFileWarehouseFactory(IRemoteFileStoreFactory remoteFileStoreFactory)
        {
            _remoteFileStoreFactory = remoteFileStoreFactory;
        }

        public async Task<IRemoteFileWarehouse> Create()
        {
            var warehouse = new RemoteFileWarehouse(_remoteFileStoreFactory);
            await warehouse.LoadStores();
            return warehouse;
        }
    }
}