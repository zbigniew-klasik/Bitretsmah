using Bitretsmah.Core.Exceptions;
using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bitretsmah.Core
{
    public interface IRemoteFileWarehouse
    {
        StoreSelectionMethod StoreSelectionMethod { get; set; }

        Task LoadStores();

        Task<RemoteId> UploadFile(string localFilePath, IProgress<double> progress);

        Task DownloadFile(RemoteId remoteFileId, string localFilePath, IProgress<double> progress);
    }

    public class RemoteFileWarehouse : IRemoteFileWarehouse
    {
        private readonly IRemoteFileStoreFactory _remoteFileStoreFactory;
        protected readonly List<IRemoteFileStore> _remoteFileStores;

        public RemoteFileWarehouse(IRemoteFileStoreFactory remoteFileStoreFactory)
        {
            _remoteFileStoreFactory = remoteFileStoreFactory;
            _remoteFileStores = new List<IRemoteFileStore>();
        }

        public StoreSelectionMethod StoreSelectionMethod { get; set; }

        public async Task LoadStores()
        {
            var allStores = await _remoteFileStoreFactory.GetAll();
            var newStores = allStores.Where(x => !_remoteFileStores.Any(y => y.StoreId.Equals(x.StoreId)));
            _remoteFileStores.AddRange(newStores);
        }

        public async Task<RemoteId> UploadFile(string localFilePath, IProgress<double> progress)
        {
            var store = GetUploadStore();
            return await store.UploadFile(localFilePath, progress);
        }

        public async Task DownloadFile(RemoteId remoteFileId, string localFilePath, IProgress<double> progress)
        {
            var store = GetDownloadStore(remoteFileId.StoreId);
            await store.DownloadFile(remoteFileId, localFilePath, progress);
        }

        private IRemoteFileStore GetDownloadStore(string storeId)
        {
            var store = _remoteFileStores.SingleOrDefault(x => x.StoreId.Equals(storeId));
            if (store == null) throw new UnknownStoreException($"Unknown store '{storeId}'.");
            return store;
        }

        private IRemoteFileStore GetUploadStore()
        {
            if (!_remoteFileStores.Any()) throw new InvalidOperationException("The store list is empty.");

            switch (StoreSelectionMethod)
            {
                case StoreSelectionMethod.WithLessFreeSpace:
                    return _remoteFileStores.OrderBy(x => x.Quota.Free).First();

                case StoreSelectionMethod.WithMoreFreeSpace:
                    return _remoteFileStores.OrderBy(x => x.Quota.Free).Last();

                default:
                    throw new NotImplementedException($"The store selection method: '{StoreSelectionMethod}' is not implemnted.");
            }
        }
    }
}