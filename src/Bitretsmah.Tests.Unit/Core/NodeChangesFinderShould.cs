using Bitretsmah.Core;
using Bitretsmah.Core.Models;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Bitretsmah.Tests.Unit.Core
{
    [TestFixture]
    public class NodeChangesFinderShould
    {
        private readonly INodeChangesFinder _finder = new NodeChangesFinder();

        #region SINGLE FILE

        [Test]
        public void FindNoChangeForSameFiles()
        {
            var initialFile = CreateF("foo.txt");
            var finalFile = CreateF("foo.txt");
            var result = _finder.Find(initialFile, finalFile);
            result.State.Should().Be(NodeState.None);
        }

        [Test]
        public void FindModifedForDifferentFileSize()
        {
            var initialFile = CreateF("foo.txt");
            var finalFile = CreateF("foo.txt");
            initialFile.Size = 5555;
            var result = _finder.Find(initialFile, finalFile);
            result.State.Should().Be(NodeState.Modified);
        }

        [Test]
        public void FindModifedForDifferentFileCreationTime()
        {
            var initialFile = CreateF("foo.txt");
            var finalFile = CreateF("foo.txt");
            initialFile.CreationTime = new DateTimeOffset(2016, 12, 12, 1, 1, 1, new TimeSpan(0));
            var result = _finder.Find(initialFile, finalFile);
            result.State.Should().Be(NodeState.Modified);
        }

        [Test]
        public void FindModifedForDifferentFileModificationTime()
        {
            var initialFile = CreateF("foo.txt");
            var finalFile = CreateF("foo.txt");
            initialFile.ModificationTime = new DateTimeOffset(2016, 12, 12, 1, 1, 1, new TimeSpan(0));
            var result = _finder.Find(initialFile, finalFile);
            result.State.Should().Be(NodeState.Modified);
        }

        [Test]
        public void FindNoChangeForNullFileHashes()
        {
            var initialFile = CreateF("foo.txt");
            var finalFile = CreateF("foo.txt");
            initialFile.Hash = null;
            finalFile.Hash = null;
            var result = _finder.Find(initialFile, finalFile);
            result.State.Should().Be(NodeState.None);
        }

        [Test]
        public void FindNoChangeForFirstNullFileHash()
        {
            var initialFile = CreateF("foo.txt");
            var finalFile = CreateF("foo.txt");
            initialFile.Hash = null;
            var result = _finder.Find(initialFile, finalFile);
            result.State.Should().Be(NodeState.None);
        }

        [Test]
        public void FindNoChangeForSecondNullFileHash()
        {
            var initialFile = CreateF("foo.txt");
            var finalFile = CreateF("foo.txt");
            finalFile.Hash = null;
            var result = _finder.Find(initialFile, finalFile);
            result.State.Should().Be(NodeState.None);
        }

        [Test]
        public void FindModifedForDifferentFileHashes()
        {
            var initialFile = CreateF("foo.txt");
            var finalFile = CreateF("foo.txt");
            finalFile.Hash = "different";
            var result = _finder.Find(initialFile, finalFile);
            result.State.Should().Be(NodeState.Modified);
        }

        #endregion SINGLE FILE

        #region SINGLE DIRECTORY

        [Test]
        public void FindNoChangesInDirectory()
        {
            var initialDirectory = CreateD("D", CreateF("F"));
            var finalDirectory = CreateD("D", CreateF("F"));
            var result = (Directory)_finder.Find(initialDirectory, finalDirectory);
            result.State.Should().Be(NodeState.None);
            result.InnerNodes.Should().BeEmpty();
        }

        [Test]
        public void FindCreatedFileInDirectory()
        {
            var initialDirectory = CreateD("D", CreateF("F1"));
            var finalDirectory = CreateD("D", CreateF("F1"), CreateF("F2"));
            var result = (Directory)_finder.Find(initialDirectory, finalDirectory);
            result.State.Should().Be(NodeState.Modified);
            result.InnerNodes.Count.Should().Be(1);
            result.InnerNodes[0].State.Should().Be(NodeState.Created);
            result.InnerNodes[0].Name.Should().Be("F2");
        }

        [Test]
        public void FindModifiedFileInDirectory()
        {
            var initialDirectory = CreateD("D", CreateF("F1"), CreateF("F2", NodeState.None, "hash"));
            var finalDirectory = CreateD("D", CreateF("F1"), CreateF("F2", NodeState.None, "different_hash"));
            var result = (Directory)_finder.Find(initialDirectory, finalDirectory);
            result.State.Should().Be(NodeState.Modified);
            result.InnerNodes.Count.Should().Be(1);
            result.InnerNodes[0].State.Should().Be(NodeState.Modified);
            result.InnerNodes[0].Name.Should().Be("F2");
        }

        [Test]
        public void FindDeletedFileInDirectory()
        {
            var initialDirectory = CreateD("D", CreateF("F1"), CreateF("F2"));
            var finalDirectory = CreateD("D", CreateF("F1"));
            var result = (Directory)_finder.Find(initialDirectory, finalDirectory);
            result.State.Should().Be(NodeState.Modified);
            result.InnerNodes.Count.Should().Be(1);
            result.InnerNodes[0].State.Should().Be(NodeState.Deleted);
            result.InnerNodes[0].Name.Should().Be("F2");
        }

        #endregion SINGLE DIRECTORY

        #region NESTED DIRECTORIES

        [Test]
        public void FindCreatedDirectoryAndMarkInnerNodes()
        {
            var initialNode =
                CreateD("D_0",
                    CreateF("F_1"));

            var finalNode =
                CreateD("D_0",
                    CreateF("F_1"),
                    CreateD("D_1", // has been created
                        CreateF("F_2"),
                        CreateD("D_2",
                            CreateF("F_3"))));

            var expectedResult =
                CreateD("D_0", NodeState.Modified,
                    CreateD("D_1", NodeState.Created,
                        CreateF("F_2", NodeState.Created),
                        CreateD("D_2", NodeState.Created,
                            CreateF("F_3", NodeState.Created))));

            var actualResult = (Directory)_finder.Find(initialNode, finalNode);
            actualResult.ShouldBeEquivalentTo(expectedResult);
            actualResult.ToJson().Should().Be(expectedResult.ToJson());
        }

        [Test]
        public void FindDeletedDirectoryAndMarkInnerNodes()
        {
            var initialNode =
                CreateD("D_0",
                    CreateF("F_1"),
                    CreateD("D_1", // will be deleted
                        CreateF("F_2"),
                        CreateD("D_2",
                            CreateF("F_3"))));

            var finalNode =
                CreateD("D_0",
                    CreateF("F_1"));

            var expectedResult =
                CreateD("D_0", NodeState.Modified,
                    CreateD("D_1", NodeState.Deleted,
                        CreateF("F_2", NodeState.Deleted),
                        CreateD("D_2", NodeState.Deleted,
                            CreateF("F_3", NodeState.Deleted))));

            var actualResult = (Directory)_finder.Find(initialNode, finalNode);
            actualResult.ShouldBeEquivalentTo(expectedResult);
            actualResult.ToJson().Should().Be(expectedResult.ToJson());
        }

        [Test]
        public void FindAllModifiedNodes()
        {
            var initialNode =
                CreateD("root",
                    CreateF("F_0"),
                    CreateF("F_1"),                 // will be deleted
                    CreateD("D_2",
                        CreateF("F_2_0"),
                        CreateF("F_2_1"),
                        CreateD("D_2_2",
                            CreateF("F_2_2_0"),
                            CreateF("F_2_2_1")),    // will be deleted
                        CreateD("D_2_3",            // will be deleted
                            CreateF("F_2_3_0"),
                            CreateF("F_2_3_1"))),
                    CreateD("D_3",                  // will be deleted
                        CreateF("F_3_0"),
                        CreateF("F_3_1"),
                        CreateD("D_3_2",
                            CreateF("F_3_2_0"),
                            CreateF("F_3_2_1")),
                        CreateD("D_3_3",
                            CreateF("F_3_3_0"),
                            CreateF("F_3_3_1"))));

            var finalNode =
                CreateD("root",
                    CreateF("F_0"),
                    CreateD("D_2",
                        CreateF("F_2_0"),
                        CreateF("F_2_1", NodeState.None, "different_hash_1"),
                        CreateD("D_2_2",
                            CreateF("F_2_2_0", NodeState.None, "different_hash_2")),
                            CreateD("D_2_2_2",      // has been created
                                CreateF("F_2_2_2_0"),
                                CreateD("F_2_2_2_1",
                                    CreateF("F_2_2_2_1_0")))));

            var expectedResult =
                CreateD("root", NodeState.Modified,
                    CreateF("F_1", NodeState.Deleted),
                    CreateD("D_2", NodeState.Modified,
                        CreateF("F_2_1", NodeState.Modified, "different_hash_1"),
                        CreateD("D_2_2", NodeState.Modified,
                            CreateF("F_2_2_0", NodeState.Modified, "different_hash_2"),
                            CreateF("F_2_2_1", NodeState.Deleted)),
                            CreateD("D_2_2_2", NodeState.Created,
                                CreateF("F_2_2_2_0", NodeState.Created),
                                CreateD("F_2_2_2_1", NodeState.Created,
                                    CreateF("F_2_2_2_1_0", NodeState.Created))),
                        CreateD("D_2_3", NodeState.Deleted,
                            CreateF("F_2_3_0", NodeState.Deleted),
                            CreateF("F_2_3_1", NodeState.Deleted))),
                    CreateD("D_3", NodeState.Deleted,
                        CreateF("F_3_0", NodeState.Deleted),
                        CreateF("F_3_1", NodeState.Deleted),
                        CreateD("D_3_2", NodeState.Deleted,
                            CreateF("F_3_2_0", NodeState.Deleted),
                            CreateF("F_3_2_1", NodeState.Deleted)),
                        CreateD("D_3_3", NodeState.Deleted,
                            CreateF("F_3_3_0", NodeState.Deleted),
                            CreateF("F_3_3_1", NodeState.Deleted))));

            var actualResult = (Directory)_finder.Find(initialNode, finalNode);
            actualResult.ShouldBeEquivalentTo(expectedResult);
            actualResult.ToJson().Should().Be(expectedResult.ToJson());
        }

        #endregion NESTED DIRECTORIES

        #region HELP METHODS

        private DateTimeOffset newDate(int seconds)
        {
            return new DateTimeOffset(2016, 11, 12, 19, 34, seconds, new TimeSpan(0));
        }

        private File CreateF(string name, NodeState state = NodeState.None, string hash = "hash")
        {
            return new File
            {
                Name = name,
                State = state,
                Hash = hash,
                Size = 1234,
                CreationTime = new DateTimeOffset(2016, 10, 11, 18, 33, 14, new TimeSpan(0)),
                ModificationTime = new DateTimeOffset(2016, 11, 12, 19, 34, 15, new TimeSpan(0)),
                AbsolutePath = @"C:\Temp\" + name + ".txt"
            };
        }

        private Directory CreateD(string name, NodeState state, params Node[] nodes)
        {
            return new Directory
            {
                Name = name,
                State = state,
                InnerNodes = new List<Node>(nodes)
            };
        }

        private Directory CreateD(string name, params Node[] nodes)
        {
            return CreateD(name, NodeState.None, nodes);
        }

        #endregion HELP METHODS
    }
}