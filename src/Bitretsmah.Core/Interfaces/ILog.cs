using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitretsmah.Core.Interfaces
{
    public interface ILog
    {
        void Info(string message);
        void Warn();
        void Error();
    }
}
