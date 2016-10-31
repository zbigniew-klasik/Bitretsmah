using Bitretsmah.Core.Interfaces;
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
    }
}