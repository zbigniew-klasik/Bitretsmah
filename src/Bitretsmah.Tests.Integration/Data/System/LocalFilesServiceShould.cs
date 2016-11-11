using NUnit.Framework;
using System.IO;

namespace Bitretsmah.Tests.Integration.Data.System
{
    [TestFixture]
    public class LocalFilesServiceShould
    {
        [Test]
        public void Test()
        {
            var file = new FileInfo(@"D:\Temp\Lucifer.S02E01.HDTV.x264-LOL[ettv]\lucifer.201.hdtv-lol[ettv].srt");
            var dir = new DirectoryInfo(@"D:\Temp\Lucifer.S02E01.HDTV.x264-LOL[ettv]\lucifer.201.hdtv-lol[ettv].srt");
        }
    }
}