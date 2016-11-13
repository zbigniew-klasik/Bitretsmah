using Bitretsmah.Core;
using Bitretsmah.Core.Models;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace Bitretsmah.Tests.Unit.Core
{
    [TestFixture]
    public class ExtensionsShould
    {
        [Test]
        public void DeepCopySimpleObject()
        {
            var file = new File("foo.txt", 123, "hash", DateTimeOffset.Now, DateTimeOffset.Now);
            var fileCopy = file.DeepCopy();

            fileCopy.Should().NotBe(file);
            fileCopy.ShouldBeEquivalentTo(file);
            fileCopy.ToJson().Should().Be(file.ToJson());
        }

        [Test]
        public void DeepCopyComplicatedObject()
        {
            var rootDirectory = new Directory("root");
            for (var i = 0; i < 10; i++)
            {
                var directory = new Directory("Dir " + i);
                rootDirectory.InnerNodes.Add(directory);

                for (var j = 0; j < 10; j++)
                {
                    var file = new File("file" + (i * j), 123, Guid.NewGuid().ToString(), DateTimeOffset.Now, DateTimeOffset.Now);
                    directory.InnerNodes.Add(file);
                }

                for (var j = 0; j < 10; j++)
                {
                    var innerDirectory = new Directory("Dir " + i * j);
                    directory.InnerNodes.Add(innerDirectory);

                    for (var k = 0; k < 10; k++)
                    {
                        var file = new File("file" + (i * j ^ k), 321, Guid.NewGuid().ToString(), DateTimeOffset.Now, DateTimeOffset.Now);
                        innerDirectory.InnerNodes.Add(file);
                    }
                }
            }

            var rootDirectoryCopy = rootDirectory.DeepCopy();
            rootDirectoryCopy.ShouldBeEquivalentTo(rootDirectory);
            rootDirectoryCopy.ToJson().Should().Be(rootDirectory.ToJson());
        }
    }
}