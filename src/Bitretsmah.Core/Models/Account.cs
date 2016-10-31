using System.Net;

namespace Bitretsmah.Core.Models
{
    public class Account
    {
        public Account()
        {
            Credential = new NetworkCredential();
            Quota = new Quota();
        }

        public NetworkCredential Credential { get; set; }
        public Quota Quota { get; set; }
    }
}