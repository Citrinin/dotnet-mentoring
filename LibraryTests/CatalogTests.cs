using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Xml;

namespace Library.Tests
{
    [TestClass()]
    public class CatalogTests
    {
        [TestMethod()]
        public void ReadCatalogTestFileStream()
        {

            var catalog = new Catalog();
            catalog.Seed();
            catalog.SaveCatalog("myFile.xml");


            catalog.ReadCatalog(new FileStream("myfile.xml", FileMode.Open));
            catalog.PrintLibrary();

            Assert.AreEqual(7, catalog.GetItems().Count());
        }

        [TestMethod()]
        public void ReadCatalogTestMemoryStream()
        {

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<library date=""06-Jun-18"" info=""The best library you've ever seen"">
  <patent>
    <name>Zubrowka lite</name>
    <author>Ivanov</author>
    <country>Norway</country>
    <registrationnumber>45613-321654</registrationnumber>
    <submission>2000-12-01T00:00:00.0000000</submission>
    <publication>2015-12-01T00:00:00.0000000</publication>
    <pages>191</pages>
    <note />
  </patent>
  <book>
    <name>Odd Tomas</name>
    <author>Dean Koontz</author>
    <placeofpublication>USA</placeofpublication>
    <publishinghouse>Hammer and sickle</publishinghouse>
    <year>2003</year>
    <note />
    <ISBN>14-74404-28-0424</ISBN>
  </book>
</library>");

            writer.Flush();
            stream.Position = 0;

            var catalog = new Catalog();

            catalog.ReadCatalog(stream);
            catalog.PrintLibrary();

            Assert.AreEqual(2, catalog.GetItems().Count());
        }

        [TestMethod()]
        [ExpectedException(typeof(XmlException))]
        public void ReadCatalogTestIncorrectItem()
        {

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<library date=""06-Jun-18"" info=""The best library you've ever seen"">
  <kotik>
    <name>Fluffy</name>
    <author>Murka</author>
    <country>Egypt</country>
    <registrationnumber>321-5</registrationnumber>
    <year>2017</year>
    <color>peach</color>
    <note />
  </kotik>
  <book>
    <name>Odd Tomas</name>
    <author>Dean Koontz</author>
    <placeofpublication>USA</placeofpublication>
    <publishinghouse>Hammer and sickle</publishinghouse>
    <year>2003</year>
    <note />
    <ISBN>14-74404-28-0424</ISBN>
  </book>
</library>");

            writer.Flush();
            stream.Position = 0;

            var catalog = new Catalog();

            catalog.ReadCatalog(stream);
        }

        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void ReadCatalogTestWrongData()
        {

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<library date=""06-Jun-18"" info=""The best library you've ever seen"">
  <patent>
    <name>Zubrowka lite</name>
    <author>Ivanov</author>
    <country>Norway</country>
    <registrationnumber>45613-321654</registrationnumber>
    <submission>2000-12-01T00:00:00.0000000</submission>
    <publication>yesterday</publication>
    <pages>five</pages>
    <note />
  </patent>
  <book>
    <name>Odd Tomas</name>
    <author>Dean Koontz</author>
    <placeofpublication>USA</placeofpublication>
    <publishinghouse>Hammer and sickle</publishinghouse>
    <year>2003</year>
    <note />
    <ISBN>14-74404-28-0424</ISBN>
  </book>
</library>");

            writer.Flush();
            stream.Position = 0;

            var catalog = new Catalog();

            catalog.ReadCatalog(stream);
        }
    }
}