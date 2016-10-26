using LiteDB;

namespace Bitretsmah.Data.LiteDB
{
    internal static class DbFactory
    {
        public static LiteDatabase Create()
        {
            return new LiteDatabase(@"bitretsmah.db");
        }
    }
}