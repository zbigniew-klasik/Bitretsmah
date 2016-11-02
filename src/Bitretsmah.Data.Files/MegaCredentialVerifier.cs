using System.Net;
using System.Threading.Tasks;
using Bitretsmah.Core.Interfaces;

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