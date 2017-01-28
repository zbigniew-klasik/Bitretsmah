using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitretsmah.Core.Exceptions
{
    public class UnknownTargetException : BitretsmahException
    {
        public UnknownTargetException(string name)
            : base($"Target with name '{name}' is not unknown.")
        {
        }
    }
}