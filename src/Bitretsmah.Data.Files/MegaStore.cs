using System.Net;
using Bitretsmah.Core.Interfaces;

namespace Bitretsmah.Data.Mega
{
    public class MegaStore : IRemoteFileStore
    {
        private NetworkCredential _credentia;

        public MegaStore(NetworkCredential credential)
        {
            _credentia = credential;
        }

        public string UserName => _credentia.UserName;
    }
}