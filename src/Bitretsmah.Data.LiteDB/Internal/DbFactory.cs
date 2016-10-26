namespace Bitretsmah.Data.LiteDB.Internal
{
    internal static class DbFactory
    {
        public static Db Create()
        {
            return new Db(@"bitretsmah.db");
        }
    }
}