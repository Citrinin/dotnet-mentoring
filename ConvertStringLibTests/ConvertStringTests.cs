using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConvertStringLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertStringLib.Tests
{
    [TestClass()]
    public class ConvertStringTests
    {
        [TestMethod()]
        public void ConvertToIntTest()
        {
            var converter = new ConvertString();
            var result = converter.ConvertToInt("-");
            Assert.AreEqual(0, result);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConvertToIntTestInputNull()
        {
            var converter = new ConvertString();
            var result = converter.ConvertToInt(null);
        }
    }
}