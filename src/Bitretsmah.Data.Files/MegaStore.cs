using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using System.Net;

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
    }
}