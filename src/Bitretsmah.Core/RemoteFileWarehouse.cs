using Bitretsmah.Core.Exceptions;
using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bitretsmah.Core
{
    public interface IRemoteFileWarehouse : IDisposable
    {
        StoreSelectionMethod StoreSelectionMethod { get; set; }

        Task LoadStores();

        Task<RemoteId> UploadFile(Stream stream, string remoteFileName, IProgress<double> progress);

        Task<Stream> DownloadFile(RemoteId remoteId, IProgress<double> progress);
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

        public async Task<RemoteId> UploadFile(Stream stream, string remoteFileName, IProgress<double> progress)
        {
            var store = await GetUploadStore();
            return await store.UploadFile(stream, remoteFileName, progress);
        }

        public async Task<Stream> DownloadFile(RemoteId remoteId, IProgress<double> progress)
        {
            var store = GetDownloadStore(remoteId.StoreId);
            return await store.DownloadFile(remoteId, progress);
        }

        private IRemoteFileStore GetDownloadStore(string storeId)
        {
            var store = _remoteFileStores.SingleOrDefault(x => x.StoreId.Equals(storeId));
            if (store == null) throw new UnknownStoreException($"Unknown store '{storeId}'.");
            return store;
        }

        private async Task<IRemoteFileStore> GetUploadStore()
        {
            if (!_remoteFileStores.Any()) throw new InvalidOperationException("The store list is empty.");

            var storesQuota = _remoteFileStores.Select(x => new { Store = x, QuotaTask = x.GetQuota() }).ToList();
            await Task.WhenAll(storesQuota.Select(x => x.QuotaTask));

            switch (StoreSelectionMethod)
            {
                case StoreSelectionMethod.WithLessFreeSpace:
                    return storesQuota.OrderBy(x => x.QuotaTask.Result.Free).First().Store;

                case StoreSelectionMethod.WithMoreFreeSpace:
                    return storesQuota.OrderBy(x => x.QuotaTask.Result.Free).Last().Store;

                default:
                    throw new NotImplementedException($"The store selection method: '{StoreSelectionMethod}' is not implemnted.");
            }
        }

        public void Dispose()
        {
            _remoteFileStores.ForEach(x => x.Dispose());
        }
    }
}