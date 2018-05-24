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
            { @"c:\myfile.txt", new MockFileData("My favorite file") },
            { @"c:\test\jQuery.js", new MockFileData("some js") },
            { @"c:\test\1.txt", new MockFileData("123") },
            { @"c:\test\image.gif", new MockFileData(new byte[] { 0x01, 0x02, 0x03, 0x04 }) },
            { @"c:\test\myfolder\coolstory.txt", new MockFileData("Cool story") }
        });

        [TestMethod()]
        public void GetDirectoryContentTestCorrectData1()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem);
            var result = fileSystemVisitor.GetDirectoryContent(@"c:\");
            Assert.AreEqual(7, result.Count());
        }

        [TestMethod()]
        public void GetDirectoryContentTestCorrectData2()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem);
            var result = fileSystemVisitor.GetDirectoryContent(@"c:\test\");
            Assert.AreEqual(5, result.Count());
        }

        [TestMethod()]
        public void GetDirectoryContentTestCorrectDataWithFilter1()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem, s => s.EndsWith(".txt"));
            var result = fileSystemVisitor.GetDirectoryContent(@"c:\test\");
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod()]
        public void GetDirectoryContentTestCorrectDataWithFilter2()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem, s => s.Contains("test"));
            var result = fileSystemVisitor.GetDirectoryContent(@"c:\");
            Assert.AreEqual(6, result.Count());
        }

        [TestMethod()]
        public void GetDirectoryContentTestCorrectDataWithFilterIsNll()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem, null);
            var result = fileSystemVisitor.GetDirectoryContent(@"c:\");
            Assert.AreEqual(7, result.Count());
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void GetDirectoryContentTestIncorrectInputData2()
        {
            var fileSystemVisitor = new FileSystemVisitor(_mockFileSystem);
            var result = fileSystemVisitor.GetDirectoryContent(@"c").Count();
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

    }
}