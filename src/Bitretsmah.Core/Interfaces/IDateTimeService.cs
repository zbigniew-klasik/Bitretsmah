using System;

namespace Bitretsmah.Core.Interfaces
{
    public interface IDateTimeService
    {
        DateTimeOffset Now { get; }
    }
}