using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConvertStringLib.Tests
{
    [TestClass()]
    public class ConvertStringTests
    {

        [TestMethod()]
        public void ConvertToIntTest_CorrectInput()
        {
            var converter = new ConvertString();
            var result = converter.ConvertToInt("500");
            Assert.AreEqual(500, result);
        }

        [TestMethod()]
        public void ConvertToIntTest_CorrectInputBelowZero()
        {
            var converter = new ConvertString();
            var result = converter.ConvertToInt("-500");
            Assert.AreEqual(-500, result);
        }

        [TestMethod()]
        public void ConvertToIntTest_CorrectInputWithSign()
        {
            var converter = new ConvertString();
            var result = converter.ConvertToInt("+500");
            Assert.AreEqual(500, result);
        }

        [TestMethod()]
        public void ConvertToIntTest_CorrectInputIntMax()
        {
            var converter = new ConvertString();
            var result = converter.ConvertToInt(int.MaxValue.ToString());
            Assert.AreEqual(int.MaxValue, result);
        }

        [TestMethod()]
        public void ConvertToIntTest_CorrectInputIntMin()
        {
            var converter = new ConvertString();
            var result = converter.ConvertToInt(int.MinValue.ToString());
            Assert.AreEqual(int.MinValue, result);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertToIntTest_ContainsLetter()
        {
            var converter = new ConvertString();
            converter.ConvertToInt("+0d");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConvertToIntTest_InputNull()
        {
            var converter = new ConvertString();
            converter.ConvertToInt(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertToIntTest_InputTooLong()
        {
            var converter = new ConvertString();
            converter.ConvertToInt("12345678912");
        }
    }
}