using Bitretsmah.Core.Interfaces;
using System.Net;
using System.Threading.Tasks;

namespace Bitretsmah.Data.Mega
{
    public class MegaCredentialVerifier : ICredentialVerifier
    {
        public Task<bool> Verify(NetworkCredential credential)
        {
            return null;
        }
    }
}