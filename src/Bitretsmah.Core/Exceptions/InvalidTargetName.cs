using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitretsmah.Core.Exceptions
{
    public class InvalidTargetName : BitretsmahException
    {
        public InvalidTargetName(string targetName)
        {
            TargetName = targetName;
        }

        public string TargetName { get; private set; }
    }
}