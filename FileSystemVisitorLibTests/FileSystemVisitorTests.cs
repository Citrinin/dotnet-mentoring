using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace FileSystemVisitorLib.Tests
{
    [TestClass()]
    public class FileSystemVisitorTests
    {
        private MockFileSystem _mockFileSystem;

        [TestInitialize]
        public void Setup() => _mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { @"c:\test\myfolder\coolstory.txt", new MockFileData("Cool story") },
            { @"c:\test\1.txt", new MockFileData("123") },
            { @"c:\test\image.gif", new MockFileData(new byte[] { 0x01, 0x02, 0x03, 0x04 }) },
            { @"c:\test\jQuery.js", new MockFileData("some js") },
            { @"c:\myfile.txt", new MockFileData("My favorite file") },
        });

        [TestMethod()]
        public void GetDirectoryContentTest_CorrectPath1()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem);
            var result = fileSystemVisitor.GetDirectoryContent(@"c:\");
            Assert.AreEqual(7, result.Count());
        }

        [TestMethod()]
        public void GetDirectoryContentTest_CorrectPath2()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem);
            var result = fileSystemVisitor.GetDirectoryContent(@"c:\test\");
            Assert.AreEqual(5, result.Count());
        }

        [TestMethod()]
        public void GetDirectoryContentTest_CorrectPath_CorrectFilter1()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem, s => s.EndsWith(".txt"));
            var result = fileSystemVisitor.GetDirectoryContent(@"c:\test\");
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod()]
        public void GetDirectoryContentTest_CorrectPath_CorrectFilter2()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem, s => s.Contains("test"));
            var result = fileSystemVisitor.GetDirectoryContent(@"c:\");
            Assert.AreEqual(6, result.Count());
        }

        [TestMethod()]
        public void GetDirectoryContentTest_CorrectPath_FilterIsNll()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem, null);
            var result = fileSystemVisitor.GetDirectoryContent(@"c:\");
            Assert.AreEqual(7, result.Count());
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void GetDirectoryContentTest_IncorrectPath()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem);
            var result = fileSystemVisitor.GetDirectoryContent("c").Count();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetDirectoryContentTest_PathNull()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem);
            var result = fileSystemVisitor.GetDirectoryContent(null).Count();
        }

        [TestMethod()]
        public void StartEventTest()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem);
            var eventHandled = false;
            fileSystemVisitor.Start += (sender, args) => eventHandled = true;
            var result = fileSystemVisitor.GetDirectoryContent(@"c:\").Count();

            Assert.AreEqual(7, result);
            Assert.AreEqual(true, eventHandled);

        }

        [TestMethod()]
        public void FinishEventTest()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem);
            var eventHandled = false;
            fileSystemVisitor.Finish += (sender, args) => eventHandled = true;
            var result = fileSystemVisitor.GetDirectoryContent(@"c:\").Count();

            Assert.AreEqual(7, result);
            Assert.AreEqual(true, eventHandled);
        }

        [TestMethod()]
        public void FileFindedEventTest_ExcludeFromResult()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem);
            fileSystemVisitor.FileFinded += (sender, args) =>
            {
                if (args.FullName.EndsWith(".txt"))
                {
                    args.ExcludeItemFromResult = true;
                }
            };
            var result = fileSystemVisitor.GetDirectoryContent(@"c:\").Count();
            Assert.AreEqual(4, result);
        }

        [TestMethod()]
        public void FileFindedEventTest_StopSearch()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem);
            fileSystemVisitor.FileFinded += (sender, args) =>
            {
                if (args.FullName.EndsWith("1.txt"))
                {
                    args.CancelSearching = true;
                }
            };
            var result = fileSystemVisitor.GetDirectoryContent(@"c:\").ToList();
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(@"c:\test\1.txt", result.Last());
        }

        [TestMethod()]
        public void DirectoryFindedEventTest_ExcludeFromResult()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem);
            fileSystemVisitor.DirectoryFinded += (sender, args) =>
            {
                if (args.FullName.Contains("test"))
                {
                    args.ExcludeItemFromResult = true;
                }
            };
            var result = fileSystemVisitor.GetDirectoryContent(@"c:\").ToList();
            Assert.AreEqual(5, result.Count);
        }

        [TestMethod()]
        public void DirectoryFindedEventTest_StopSearch()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem);
            fileSystemVisitor.DirectoryFinded += (sender, args) =>
            {
                if (args.FullName.Contains("folder"))
                {
                    args.CancelSearching = true;
                }
            };
            var result = fileSystemVisitor.GetDirectoryContent(@"c:\").ToList();
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(@"c:\test\myfolder", result.Last());
        }

        [TestMethod()]
        public void FileFindedEventTest_ExcludeFromResultWithFilter()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem, s => s.Contains("test"));
            fileSystemVisitor.FileFinded += (sender, args) =>
            {
                if (args.FullName.EndsWith(".txt"))
                {
                    args.ExcludeItemFromResult = true;
                }
            };
            var result = fileSystemVisitor.GetDirectoryContent(@"c:\").Count();
            Assert.AreEqual(4, result);
        }

        [TestMethod()]
        public void FileFindedEventTest_StopSearchWithFilter()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem, s => s.Contains("test"));
            fileSystemVisitor.FileFinded += (sender, args) =>
            {
                if (args.FullName.Contains("story"))
                {
                    args.CancelSearching = true;
                }
            };
            var result = fileSystemVisitor.GetDirectoryContent(@"c:\").ToList();
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(@"c:\test\myfolder\coolstory.txt", result.Last());
        }

        [TestMethod()]
        public void DirectoryFindedEventTest_ExcludeFromResultWithFilter()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem, s => s.Contains("test"));
            fileSystemVisitor.DirectoryFinded += (sender, args) =>
            {
                if (args.FullName.Contains("test"))
                {
                    args.ExcludeItemFromResult = true;
                }
            };
            var result = fileSystemVisitor.GetDirectoryContent(@"c:\").ToList();
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod()]
        public void DirectoryFindedEventTest_StopSearchWithFilter()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem, s => !s.Contains("test"));
            fileSystemVisitor.DirectoryFinded += (sender, args) =>
            {
                if (args.FullName.Contains("test"))
                {
                    args.CancelSearching = true;
                }
            };
            var result = fileSystemVisitor.GetDirectoryContent(@"c:\").ToList();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(@"c:\test", result.Last());
        }
        //----

        [TestMethod()]
        public void FilteredFileFindedEventTest_ExcludeFromResultWithFilter()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem, s => s.Contains("test"));
            fileSystemVisitor.FilteredFileFinded += (sender, args) =>
            {
                if (args.FullName.EndsWith(".gif") || args.FullName.EndsWith(".js"))
                {
                    args.ExcludeItemFromResult = true;
                }
            };
            var result = fileSystemVisitor.GetDirectoryContent(@"c:\").Count();
            Assert.AreEqual(4, result);
        }

        [TestMethod()]
        public void FilteredFileFindedEventTest_StopSearchWithFilter()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem, s => s.Contains("myfolder"));
            fileSystemVisitor.FilteredFileFinded += (sender, args) =>
            {
                if (args.FullName.Contains("story"))
                {
                    args.CancelSearching = true;
                }
            };
            var result = fileSystemVisitor.GetDirectoryContent(@"c:\").ToList();
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(@"c:\test\myfolder\coolstory.txt", result.Last());
        }


        [TestMethod()]
        public void FilteredDirectoryFindedEventTest_ExcludeFromResultWithFilter()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem, s => s.Length >= 15);
            fileSystemVisitor.FilteredDirectoryFinded += (sender, args) =>
            {
                if (args.FullName.Contains("folder"))
                {
                    args.ExcludeItemFromResult = true;
                }
            };
            var result = fileSystemVisitor.GetDirectoryContent(@"c:\").ToList();
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod()]
        public void FilteredDirectoryFindedEventTest_StopSearchWithFilter()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem, s => s.Contains("test"));
            fileSystemVisitor.FilteredDirectoryFinded += (sender, args) =>
            {
                if (args.FullName.Contains("folder"))
                {
                    args.CancelSearching = true;
                }
            };
            var result = fileSystemVisitor.GetDirectoryContent(@"c:\").ToList();
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(@"c:\test\myfolder", result.Last());
        }

    }
}