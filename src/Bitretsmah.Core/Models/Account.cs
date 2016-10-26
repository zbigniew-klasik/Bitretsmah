using System.Net;

namespace Bitretsmah.Core.Models
{
    public class Account
    {
        public NetworkCredential Credential { get; set; }
        public Quota Quota { get; set; }
    }
}