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
        private readonly NetworkCredential _credentia;
        private readonly IMegaApiClient _megaApiClient;
        private readonly INode _rootNode;

        public MegaStore(NetworkCredential credential)
        {
            _credentia = credential;
            _megaApiClient = new MegaApiClient();
            _megaApiClient.Login(credential.UserName, credential.Password);
            _rootNode = _megaApiClient.GetNodes().Single(n => n.Type == NodeType.Root); // TODO async
        }

        public string StoreId => _credentia.UserName;

        public Quota Quota { get; private set; }

        public async Task<RemoteId> UploadFile(string localFilePath, IProgress<double> progress)
        {
            var node = await _megaApiClient.UploadFileAsync(localFilePath, _rootNode, progress);
            // TODO update quota
            return new RemoteId(StoreId, node.Id);
        }

        public async Task DownloadFile(RemoteId remoteId, string localFilePath, IProgress<double> progress)
        {
            if (remoteId.StoreId != StoreId) throw new ArgumentException(); // todo
            var node = (await _megaApiClient.GetNodesAsync(_rootNode)).SingleOrDefault(x => x.Id == remoteId.NodeId);
            if (node == null) throw new Exception("invalid node"); // todo
            await _megaApiClient.DownloadFileAsync(node, localFilePath, progress);
        }
    }
}