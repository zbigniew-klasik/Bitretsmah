using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Bitretsmah.Data.Mega
{
    public class MegaStore : IRemoteFileStore
    {
        private NetworkCredential _credentia;

        public MegaStore(NetworkCredential credential)
        {
            _credentia = credential;
        }

        public string StoreId => _credentia.UserName;

        public Quota Quota { get; private set; }

        public Task<RemoteId> UploadFile(string localFilePath, IProgress<double> progress)
        {
            throw new NotImplementedException();
        }

        public Task DownloadFile(RemoteId remoteId, string localFilePath, IProgress<double> progress)
        {
            throw new NotImplementedException();
        }
    }
}