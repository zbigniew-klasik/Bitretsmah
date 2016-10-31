using System.Net;
using System.Threading.Tasks;

namespace Bitretsmah.Core.Interfaces
{
    public interface ICredentialVerifier
    {
        Task<bool> Verify(NetworkCredential credential);
    }
}