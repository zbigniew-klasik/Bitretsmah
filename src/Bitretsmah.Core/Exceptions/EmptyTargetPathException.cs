using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitretsmah.Core.Exceptions
{
    public class EmptyTargetPathException : BitretsmahException
    {
        public EmptyTargetPathException()
            : base($"Target path cannot be empty.")
        {
        }
    }
}
