using Bitretsmah.Core.Interfaces;
using CG.Web.MegaApiClient;
using System.Net;
using System.Threading.Tasks;

namespace Bitretsmah.Data.Mega
{
    public class MegaCredentialVerifier : ICredentialVerifier
    {
        public async Task<bool> Verify(NetworkCredential credential)
        {
            try
            {
                var client = new MegaApiClient();
                await client.LoginAsync(credential.UserName, credential.Password);
                await client.LogoutAsync();
                return true;
            }
            catch (ApiException)
            {
                return false;
            }
        }
    }
}