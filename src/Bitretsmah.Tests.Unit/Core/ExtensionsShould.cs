using Bitretsmah.Core;
using FluentAssertions;
using NUnit.Framework;
using System;
using static Bitretsmah.Tests.Unit.Core.NodesTestHelper;

namespace Bitretsmah.Tests.Unit.Core
{
    [TestFixture]
    public class ExtensionsShould
    {
        [Test]
        public void DeepCopySimpleObject()
        {
            var file = CreateFile("foo.txt");
            var fileCopy = file.DeepCopy();

            fileCopy.Should().NotBe(file);
            fileCopy.ShouldBeEquivalentTo(file);
            fileCopy.ShouldSerializeSameAs(file);
        }

        [Test]
        public void DeepCopyComplicatedObject()
        {
            var rootDirectory = CreateDirectory("root");
            for (var i = 0; i < 10; i++)
            {
                var directory = CreateDirectory("Dir " + i);
                rootDirectory.InnerNodes.Add(directory);

                for (var j = 0; j < 10; j++)
                {
                    var file = CreateFile("file" + (i * j), Guid.NewGuid().ToString());
                    directory.InnerNodes.Add(file);
                }

                for (var j = 0; j < 10; j++)
                {
                    var innerDirectory = CreateDirectory("Dir " + i * j);
                    directory.InnerNodes.Add(innerDirectory);

                    for (var k = 0; k < 10; k++)
                    {
                        var file = CreateFile("file" + (i * j ^ k), Guid.NewGuid().ToString());
                        innerDirectory.InnerNodes.Add(file);
                    }
                }
            }

            var rootDirectoryCopy = rootDirectory.DeepCopy();
            rootDirectoryCopy.ShouldBeEquivalentTo(rootDirectory);
            rootDirectoryCopy.ShouldSerializeSameAs(rootDirectory);
        }
    }
}