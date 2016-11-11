using Bitretsmah.Data.System;
using FluentAssertions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Bitretsmah.Tests.Integration.Data.System
{
    // TODO:
    [TestFixture]
    public class LocalFilesServiceShould
    {
        [Test]
        public async Task Test()
        {
            var dir = @"D:\Temp";

            var service = new LocalFilesService();
            var node = service.GetNodeStructure(dir);

            node.Should().NotBeNull();
        }
    }
}