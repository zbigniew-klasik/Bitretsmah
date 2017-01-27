using System;

namespace Bitretsmah.Core.Interfaces
{
    public interface ILogger
    {
        void Info(string message);
        void Info(string message, params object[] args);
        void Warn(string message);
        void Warn(string message, params object[] args);
        void Warn(Exception exception);
        void Warn(Exception exception, string message);
        void Warn(Exception exception, string message, params object[] args);
        void Error(string message);
        void Error(string message, params object[] args);
        void Error(Exception exception);
        void Error(Exception exception, string message);
        void Error(Exception exception, string message, params object[] args);
    }
}
