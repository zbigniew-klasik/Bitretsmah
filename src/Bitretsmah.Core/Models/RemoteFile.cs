using System.Text.RegularExpressions;

namespace Bitretsmah.Core.Models
{
    public class RemoteFile
    {
        public RemoteId Id { get; set; }
        public string Name { get; set; }
        public decimal Size { get; set; }

        public string Hash
        {
            get
            {
                if(string.IsNullOrWhiteSpace(Name)) return null;
                var match = Regex.Match(Name, @"(?<=^\[)[0-9A-F]{40}(?=\]_.*$)",RegexOptions.Singleline | RegexOptions.CultureInvariant);
                return match.Success ? match.Value : null;
            }
        }
    }
}