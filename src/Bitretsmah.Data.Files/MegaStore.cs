using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using CG.Web.MegaApiClient;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Bitretsmah.Data.Mega
{
    public class MegaStore : IRemoteFileStore
    {
        private readonly NetworkCredential _credentia;
        private readonly IMegaApiClient _megaApiClient;

        public MegaStore(NetworkCredential credential)
        {
            _credentia = credential;
            _megaApiClient = new MegaApiClient();
        }

        public string StoreId => _credentia.UserName;

        public Quota Quota { get; private set; }

        public async Task<RemoteId> UploadFile(string localFilePath, IProgress<double> progress)
        {
            INode parentNode = null;
            INode node = await _megaApiClient.UploadFileAsync(localFilePath, parentNode, progress);

            // todo update quota
            throw new NotImplementedException();
        }

        public async Task DownloadFile(RemoteId remoteId, string localFilePath, IProgress<double> progress)
        {
            INode node = null;

            await _megaApiClient.DownloadFileAsync(node, localFilePath, progress);

            throw new NotImplementedException();
        }
    }
}