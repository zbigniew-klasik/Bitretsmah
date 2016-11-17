using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using CG.Web.MegaApiClient;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Bitretsmah.Data.Mega
{
    public class MegaStore : IRemoteFileStore
    {
        private readonly NetworkCredential _credential;
        private readonly IMegaApiClient _megaApiClient;
        private readonly Quota _quota;
        private INode _rootNode;
        private bool _isInitialized;

        public MegaStore(NetworkCredential credential)
        {
            _credential = credential;
            _megaApiClient = new MegaApiClient();
            _quota = new Quota();
        }

        public string StoreId => _credential.UserName;

        public async Task<Quota> GetQuota()
        {
            await EnsureInitialized();
            return _quota;
        }

        public async Task<RemoteId> UploadFile(string localFilePath, IProgress<double> progress)
        {
            await EnsureInitialized();
            var node = await _megaApiClient.UploadFileAsync(localFilePath, _rootNode, progress);
            await UpdateQuota();
            return new RemoteId(StoreId, node.Id);
        }

        public async Task DownloadFile(RemoteId remoteId, string localFilePath, IProgress<double> progress)
        {
            await EnsureInitialized();
            if (remoteId.StoreId != StoreId) throw new ArgumentException(); // todo
            var node = (await _megaApiClient.GetNodesAsync(_rootNode)).SingleOrDefault(x => x.Id == remoteId.NodeId);
            if (node == null) throw new Exception("invalid node"); // todo
            await _megaApiClient.DownloadFileAsync(node, localFilePath, progress);
        }

        private async Task EnsureInitialized()
        {
            if (_isInitialized) return;
            await _megaApiClient.LoginAsync(_credential.UserName, _credential.Password);
            await UpdateQuota();
            _rootNode = (await _megaApiClient.GetNodesAsync()).Single(n => n.Type == NodeType.Root);
            _isInitialized = true;
        }

        private async Task UpdateQuota()
        {
            var information = await _megaApiClient.GetAccountInformationAsync();
            _quota.Total = information.TotalQuota;
            _quota.Used = information.UsedQuota;
        }

        public void Dispose()
        {
            if (_isInitialized) _megaApiClient.LogoutAsync();
        }
    }
}