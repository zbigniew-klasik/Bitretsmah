using Bitretsmah.Core.Interfaces;
using System;

namespace Bitretsmah.Data.System
{
    public class DateTimeService : IDateTimeService
    {
        public DateTimeOffset Now => DateTimeOffset.Now;
    }
}