﻿using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using CG.Web.MegaApiClient;
using System;
using System.Collections.Generic;
using System.IO;
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

        public async Task<ICollection<RemoteFile>> GetFilesList()
        {
            await EnsureInitialized();
            var query =
                from fileNode in (await _megaApiClient.GetNodesAsync(_rootNode))
                where fileNode.Type == NodeType.File
                select new RemoteFile
                {
                    Id = new RemoteId { StoreId = StoreId, NodeId = fileNode.Id },
                    Name = fileNode.Name,
                    Size = fileNode.Size
                };

            return query.ToList();
        }

        public async Task<RemoteId> UploadFile(Stream stream, string remoteFileName, IProgress<double> progress)
        {
            await EnsureInitialized();
            var node = await _megaApiClient.UploadAsync(stream, remoteFileName, _rootNode, progress);
            await UpdateQuota();
            return new RemoteId(StoreId, node.Id);
        }

        public async Task<Stream> DownloadFile(RemoteId remoteId, IProgress<double> progress)
        {
            await EnsureInitialized();
            if (remoteId.StoreId != StoreId) throw new ArgumentException(); // todo
            var node = (await _megaApiClient.GetNodesAsync(_rootNode)).SingleOrDefault(x => x.Id == remoteId.NodeId);
            if (node == null) throw new Exception("invalid node"); // todo
            var stream = await _megaApiClient.DownloadAsync(node, progress);
            return stream;
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