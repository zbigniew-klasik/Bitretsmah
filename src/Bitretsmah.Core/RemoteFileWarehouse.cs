using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bitretsmah.Core
{
    public class RemoteFileWarehouse
    {
        private readonly IAccountService _accountService;
        private readonly IRemoteFileStoreFactory _remoteFileStoreFactory;
        private readonly IList<IRemoteFileStore> _remoteFileStores;

        public RemoteFileWarehouse(IAccountService accountService, IRemoteFileStoreFactory remoteFileStoreFactory)
        {
            _accountService = accountService;
            _remoteFileStoreFactory = remoteFileStoreFactory;
            _remoteFileStores = new List<IRemoteFileStore>();
        }

        public StoreSelectionMethod StoreSelectionMethod { get; set; }

        public async Task Initialize()
        {
        }

        public async Task<RemoteId> UploadFile(string sourceFilePath)
        {
            var store = GetUploadStore();
            throw new NotImplementedException();
        }

        public async Task DownloadFile(RemoteId sourceId, string destinationFilePath)
        {
            var store = GetDownloadStore(sourceId.StoreId);
            throw new NotImplementedException();
        }

        private IRemoteFileStore GetDownloadStore(string storeId)
        {
            return null;
        }

        private IRemoteFileStore GetUploadStore()
        {
            switch (StoreSelectionMethod)
            {
                case StoreSelectionMethod.WithLessFreeSpace:
                    return null;

                case StoreSelectionMethod.WithMoreFreeSpace:
                    return null;

                case StoreSelectionMethod.Random:
                    return null;

                default:
                    return null;
            }
        }
    }
}