using System.Net;

namespace Bitretsmah.Core.Models
{
    public class Account
    {
        public Account()
            : this(new NetworkCredential(), new Quota())
        {
        }

        public Account(NetworkCredential credential, Quota quota)
        {
            Credential = credential;
            Quota = quota;
        }

        public int Id { get; set; }

        public NetworkCredential Credential { get; set; }

        public Quota Quota { get; set; }
    }
}