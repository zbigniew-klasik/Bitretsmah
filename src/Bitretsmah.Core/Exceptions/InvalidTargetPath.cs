using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitretsmah.Core.Exceptions
{
    public class InvalidTargetPath : BitretsmahException
    {
        public InvalidTargetPath(string path)
        {
            TargetPath = path;
        }

        public string TargetPath { get; private set; }
    }
}