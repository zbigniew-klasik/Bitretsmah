using Bitretsmah.Core;
using Bitretsmah.Core.Models;
using FluentAssertions;
using NUnit.Framework;
using System;
using static Bitretsmah.Tests.Unit.Core.NodesTestHelper;

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
            var initialFile = CreateFile("foo.txt");
            var finalFile = CreateFile("foo.txt");
            var result = _finder.Find(initialFile, finalFile);
            result.State.Should().Be(NodeState.None);
        }

        [Test]
        public void FindModifedForDifferentFileSize()
        {
            var initialFile = CreateFile("foo.txt");
            var finalFile = CreateFile("foo.txt");
            initialFile.Size = 5555;
            var result = _finder.Find(initialFile, finalFile);
            result.State.Should().Be(NodeState.Modified);
        }

        [Test]
        public void FindModifedForDifferentFileCreationTime()
        {
            var initialFile = CreateFile("foo.txt");
            var finalFile = CreateFile("foo.txt");
            initialFile.CreationTime = new DateTimeOffset(2016, 12, 12, 1, 1, 1, new TimeSpan(0));
            var result = _finder.Find(initialFile, finalFile);
            result.State.Should().Be(NodeState.Modified);
        }

        [Test]
        public void FindModifedForDifferentFileModificationTime()
        {
            var initialFile = CreateFile("foo.txt");
            var finalFile = CreateFile("foo.txt");
            initialFile.ModificationTime = new DateTimeOffset(2016, 12, 12, 1, 1, 1, new TimeSpan(0));
            var result = _finder.Find(initialFile, finalFile);
            result.State.Should().Be(NodeState.Modified);
        }

        [Test]
        public void FindNoChangeForNullFileHashes()
        {
            var initialFile = CreateFile("foo.txt");
            var finalFile = CreateFile("foo.txt");
            initialFile.Hash = null;
            finalFile.Hash = null;
            var result = _finder.Find(initialFile, finalFile);
            result.State.Should().Be(NodeState.None);
        }

        [Test]
        public void FindNoChangeForFirstNullFileHash()
        {
            var initialFile = CreateFile("foo.txt");
            var finalFile = CreateFile("foo.txt");
            initialFile.Hash = null;
            var result = _finder.Find(initialFile, finalFile);
            result.State.Should().Be(NodeState.None);
        }

        [Test]
        public void FindNoChangeForSecondNullFileHash()
        {
            var initialFile = CreateFile("foo.txt");
            var finalFile = CreateFile("foo.txt");
            finalFile.Hash = null;
            var result = _finder.Find(initialFile, finalFile);
            result.State.Should().Be(NodeState.None);
        }

        [Test]
        public void FindModifedForDifferentFileHashes()
        {
            var initialFile = CreateFile("foo.txt");
            var finalFile = CreateFile("foo.txt");
            finalFile.Hash = "different";
            var result = _finder.Find(initialFile, finalFile);
            result.State.Should().Be(NodeState.Modified);
        }

        #endregion SINGLE FILE

        #region SINGLE DIRECTORY

        [Test]
        public void FindNoChangesInDirectory()
        {
            var initialDirectory = CreateDirectory("D", CreateFile("F"));
            var finalDirectory = CreateDirectory("D", CreateFile("F"));
            var result = (Directory)_finder.Find(initialDirectory, finalDirectory);
            result.State.Should().Be(NodeState.None);
            result.InnerNodes.Should().BeEmpty();
        }

        [Test]
        public void FindCreatedFileInDirectory()
        {
            var initialDirectory = CreateDirectory("D", CreateFile("F1"));
            var finalDirectory = CreateDirectory("D", CreateFile("F1"), CreateFile("F2"));
            var result = (Directory)_finder.Find(initialDirectory, finalDirectory);
            result.State.Should().Be(NodeState.Modified);
            result.InnerNodes.Count.Should().Be(1);
            result.InnerNodes[0].State.Should().Be(NodeState.Created);
            result.InnerNodes[0].Name.Should().Be("F2");
        }

        [Test]
        public void FindModifiedFileInDirectory()
        {
            var initialDirectory = CreateDirectory("D", CreateFile("F1"), CreateFile("F2", "hash"));
            var finalDirectory = CreateDirectory("D", CreateFile("F1"), CreateFile("F2", "different_hash"));
            var result = (Directory)_finder.Find(initialDirectory, finalDirectory);
            result.State.Should().Be(NodeState.Modified);
            result.InnerNodes.Count.Should().Be(1);
            result.InnerNodes[0].State.Should().Be(NodeState.Modified);
            result.InnerNodes[0].Name.Should().Be("F2");
        }

        [Test]
        public void FindDeletedFileInDirectory()
        {
            var initialDirectory = CreateDirectory("D", CreateFile("F1"), CreateFile("F2"));
            var finalDirectory = CreateDirectory("D", CreateFile("F1"));
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
                CreateDirectory("D_0",
                    CreateFile("F_1"));

            var finalNode =
                CreateDirectory("D_0",
                    CreateFile("F_1"),
                    CreateDirectory("D_1", // has been created
                        CreateFile("F_2"),
                        CreateDirectory("D_2",
                            CreateFile("F_3"))));

            var expectedResult =
                CreateDirectory("D_0", NodeState.Modified,
                    CreateDirectory("D_1", NodeState.Created,
                        CreateFile("F_2", NodeState.Created),
                        CreateDirectory("D_2", NodeState.Created,
                            CreateFile("F_3", NodeState.Created))));

            var actualResult = (Directory)_finder.Find(initialNode, finalNode);
            actualResult.ShouldBeEquivalentTo(expectedResult);
            actualResult.ShouldSerializeSameAs(expectedResult);
        }

        [Test]
        public void FindDeletedDirectoryAndMarkInnerNodes()
        {
            var initialNode =
                CreateDirectory("D_0",
                    CreateFile("F_1"),
                    CreateDirectory("D_1", // will be deleted
                        CreateFile("F_2"),
                        CreateDirectory("D_2",
                            CreateFile("F_3"))));

            var finalNode =
                CreateDirectory("D_0",
                    CreateFile("F_1"));

            var expectedResult =
                CreateDirectory("D_0", NodeState.Modified,
                    CreateDirectory("D_1", NodeState.Deleted,
                        CreateFile("F_2", NodeState.Deleted),
                        CreateDirectory("D_2", NodeState.Deleted,
                            CreateFile("F_3", NodeState.Deleted))));

            var actualResult = (Directory)_finder.Find(initialNode, finalNode);
            actualResult.ShouldBeEquivalentTo(expectedResult);
            actualResult.ShouldSerializeSameAs(expectedResult);
        }

        [Test]
        public void FindAllModifiedNodes()
        {
            var initialNode =
                CreateDirectory("root",
                    CreateFile("F_0"),
                    CreateFile("F_1"),                          // will be deleted
                    CreateDirectory("D_2",
                        CreateFile("F_2_0"),
                        CreateFile("F_2_1"),
                        CreateDirectory("D_2_2",
                            CreateFile("F_2_2_0"),
                            CreateFile("F_2_2_1")),             // will be deleted
                        CreateDirectory("D_2_3",                // will be deleted
                            CreateFile("F_2_3_0"),
                            CreateFile("F_2_3_1"))),
                    CreateDirectory("D_3",                      // will be deleted
                        CreateFile("F_3_0"),
                        CreateFile("F_3_1"),
                        CreateDirectory("D_3_2",
                            CreateFile("F_3_2_0"),
                            CreateFile("F_3_2_1")),
                        CreateDirectory("D_3_3",
                            CreateFile("F_3_3_0"),
                            CreateFile("F_3_3_1"))));

            var finalNode =
                CreateDirectory("root",
                    CreateFile("F_0"),
                    CreateDirectory("D_2",
                        CreateFile("F_2_0"),
                        CreateFile("F_2_1", "different_hash_1"),
                        CreateDirectory("D_2_2",
                            CreateFile("F_2_2_0", "different_hash_2")),
                            CreateDirectory("D_2_2_2",          // has been created
                                CreateFile("F_2_2_2_0"),
                                CreateDirectory("F_2_2_2_1",
                                    CreateFile("F_2_2_2_1_0")))));

            var expectedResult =
                CreateDirectory("root", NodeState.Modified,
                    CreateFile("F_1", NodeState.Deleted),
                    CreateDirectory("D_2", NodeState.Modified,
                        CreateFile("F_2_1", NodeState.Modified, "different_hash_1"),
                        CreateDirectory("D_2_2", NodeState.Modified,
                            CreateFile("F_2_2_0", NodeState.Modified, "different_hash_2"),
                            CreateFile("F_2_2_1", NodeState.Deleted)),
                            CreateDirectory("D_2_2_2", NodeState.Created,
                                CreateFile("F_2_2_2_0", NodeState.Created),
                                CreateDirectory("F_2_2_2_1", NodeState.Created,
                                    CreateFile("F_2_2_2_1_0", NodeState.Created))),
                        CreateDirectory("D_2_3", NodeState.Deleted,
                            CreateFile("F_2_3_0", NodeState.Deleted),
                            CreateFile("F_2_3_1", NodeState.Deleted))),
                    CreateDirectory("D_3", NodeState.Deleted,
                        CreateFile("F_3_0", NodeState.Deleted),
                        CreateFile("F_3_1", NodeState.Deleted),
                        CreateDirectory("D_3_2", NodeState.Deleted,
                            CreateFile("F_3_2_0", NodeState.Deleted),
                            CreateFile("F_3_2_1", NodeState.Deleted)),
                        CreateDirectory("D_3_3", NodeState.Deleted,
                            CreateFile("F_3_3_0", NodeState.Deleted),
                            CreateFile("F_3_3_1", NodeState.Deleted))));

            var actualResult = (Directory)_finder.Find(initialNode, finalNode);
            actualResult.ShouldBeEquivalentTo(expectedResult);
            actualResult.ShouldSerializeSameAs(expectedResult);
        }

        #endregion NESTED DIRECTORIES

        #region NULL INITIAL NODE

        [Test]
        public void ForNullInitialNodeReturnsFinalNodeWithCreatedState()
        {
            Node initialNode = null;

            var finalNode =
                CreateDirectory("D_0",
                    CreateFile("F_1"),
                    CreateDirectory("D_1",
                        CreateFile("F_2"),
                        CreateDirectory("D_2",
                            CreateFile("F_3"))));

            var expectedResult =
                CreateDirectory("D_0", NodeState.Created,
                    CreateFile("F_1", NodeState.Created),
                    CreateDirectory("D_1", NodeState.Created,
                        CreateFile("F_2", NodeState.Created),
                        CreateDirectory("D_2", NodeState.Created,
                            CreateFile("F_3", NodeState.Created))));

            var actualResult = (Directory)_finder.Find(initialNode, finalNode);
            actualResult.ShouldBeEquivalentTo(expectedResult);
            actualResult.ShouldSerializeSameAs(expectedResult);
        }

        #endregion
    }
}