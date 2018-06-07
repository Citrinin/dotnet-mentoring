using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Library
{
    public class Catalog
    {
        private IEnumerable<LibraryItem> _library;

        public void Seed()
        {
            _library = new List<LibraryItem>
            {
                new NewsPaper
                {
                    Name = "Zarya",
                    Date = DateTime.Now,
                    PublishingHouse = "BelSouzDruk",
                    YearOfPublication = 2018,
                    PlaceOfPublication = "Minsk",
                    AmountOfPages = 10,
                    ISSN = "132-456-952621",
                    Number = 1
                },
                new NewsPaper
                {
                    Name = "Stolichnaya",
                    Date = DateTime.MinValue,
                    PublishingHouse = "Moskva",
                    YearOfPublication = 1997,
                    PlaceOfPublication = "Soligorsk",
                    AmountOfPages = 1000,
                    ISSN = "177-456-9523470",
                    Number = 14
                },
                new Book
                {
                    Author = "Железяны",
                    Name = "Хроники Амбера",
                    PlaceOfPublication = "Poland",
                    ISBN = "1265478-456-14",
                    PublishingHouse = "EKSMO",
                    YearOfPublication = 1945
                },
                new Patent
                {
                    Author = "Ivanov",
                    Country = "Norway",
                    Name = "Zubrowka lite",
                    SubmissionDate = new DateTime(2000,12,1),
                    PublicationDate = new DateTime(2015,12,1),
                    AmountOfPages = 191,
                    RegistrationNumber = "45613-321654"
                },
                new Book
                {
                    Author = "Author",
                    Name = "Book",
                    PlaceOfPublication = "USA",
                    ISBN = "154-1078-0424",
                    PublishingHouse = "Red Dawn",
                    YearOfPublication = 2002
                },
                new Book
                {
                    Author = "Mitchell",
                    Name = "Cloud atlas",
                    PlaceOfPublication = "USA",
                    ISBN = "145-41478-0424",
                    PublishingHouse = "Red October",
                    YearOfPublication = 2018
                },
                new Book
                {
                    Author = "Dean Koontz",
                    Name = "Odd Tomas",
                    PlaceOfPublication = "USA",
                    ISBN = "14-74404-28-0424",
                    PublishingHouse = "Hammer and sickle",
                    YearOfPublication = 2003
                },
            };

        }

        public void SaveCatalog(string fileName)
        {
            var writer = XmlWriter.Create(fileName, new XmlWriterSettings { Indent = true });
          
            writer.WriteStartElement("library");
            writer.WriteAttributeString("date", DateTime.Now.ToShortDateString());
            writer.WriteAttributeString("info", "The best library you've ever seen");
            foreach (var libraryItem in _library)
            {
                switch (libraryItem)
                {
                    case Book book:
                        {
                            var element = new XElement("book",
                                 new XElement("name", book.Name),
                                 new XElement("author", book.Author),
                                 new XElement("placeofpublication", book.PlaceOfPublication),
                                 new XElement("publishinghouse", book.PublishingHouse),
                                 new XElement("year", book.YearOfPublication),
                                 new XElement("note", book.Note),
                                 new XElement("ISBN", book.ISBN)
                                 );
                            element.WriteTo(writer);
                            break;
                        }
                    case NewsPaper newsPaper:
                        {
                            var element = new XElement("paper",
                                new XElement("name", newsPaper.Name),
                                new XElement("placeofpublication", newsPaper.PlaceOfPublication),
                                new XElement("publishinghouse", newsPaper.PublishingHouse),
                                new XElement("year", newsPaper.YearOfPublication),
                                new XElement("pages", newsPaper.AmountOfPages),
                                new XElement("number", newsPaper.Number),
                                new XElement("date", newsPaper.Date.ToString("O")),
                                new XElement("note", newsPaper.Note),
                                new XElement("ISSN", newsPaper.ISSN)
                                );
                            element.WriteTo(writer);
                            break;
                        }
                    case Patent patent:
                        {
                            var element = new XElement("patent",
                                new XElement("name", patent.Name),
                                new XElement("author", patent.Author),
                                new XElement("country", patent.Country),
                                new XElement("registrationnumber", patent.RegistrationNumber),
                                new XElement("submission", patent.SubmissionDate.ToString("O")),
                                new XElement("publication", patent.PublicationDate.ToString("O")),
                                new XElement("pages", patent.AmountOfPages),
                                new XElement("note", patent.Note)
                                );
                            element.WriteTo(writer);
                            break;
                        }
                    default:
                        {
                            throw new XmlException("Unexpected element in library");
                        }
                }
            }
            writer.WriteEndElement();
            writer.Close();
        }

        public void ReadCatalog(Stream sourceStream)
        {
            var newLibrary = new List<LibraryItem>();
            var reader = XmlReader.Create(sourceStream);
            reader.MoveToContent();
            while (reader.Read())
            {
                if (reader.NodeType != XmlNodeType.Element) continue;

                var element = XNode.ReadFrom(reader) as XElement;

                if (element == null) continue;

                switch (element.Name.LocalName)
                {
                    case "book":
                        {
                            newLibrary.Add(new Book
                            {
                                Name = element.Element("name")?.Value,
                                Author = element.Element("author")?.Value,
                                ISBN = element.Element("ISBN")?.Value,
                                Note = element.Element("note")?.Value,
                                PlaceOfPublication = element.Element("placeofpublication")?.Value,
                                PublishingHouse = element.Element("publishinghouse")?.Value,
                                YearOfPublication = XmlConvert.ToInt32(element.Element("year")?.Value ?? "0")
                            });
                            break;
                        }
                    case "paper":
                        {
                            newLibrary.Add(new NewsPaper
                            {
                                Name = element.Element("name")?.Value,
                                Date =
                                    element.Element("date") == null
                                    ? default(DateTime)
                                    : XmlConvert.ToDateTime(element.Element("date")?.Value, XmlDateTimeSerializationMode.Unspecified),
                                ISSN = element.Element("ISSN")?.Value,
                                Note = element.Element("note")?.Value,
                                PlaceOfPublication = element.Element("placeofpublication")?.Value,
                                PublishingHouse = element.Element("publishinghouse")?.Value,
                                YearOfPublication = XmlConvert.ToInt32(element.Element("year")?.Value ?? "0"),
                                AmountOfPages = XmlConvert.ToInt32(element.Element("pages")?.Value ?? "0"),
                                Number = XmlConvert.ToInt32(element.Element("number")?.Value ?? "0"),
                            });
                            break;
                        }
                    case "patent":
                        {
                            newLibrary.Add(new Patent
                            {
                                Name = element.Element("name")?.Value,
                                PublicationDate =
                                    element.Element("publication") == null
                                    ? default(DateTime)
                                    : XmlConvert.ToDateTime(element.Element("publication")?.Value, XmlDateTimeSerializationMode.Unspecified),
                                SubmissionDate =
                                    element.Element("submission") == null
                                    ? default(DateTime)
                                    : XmlConvert.ToDateTime(element.Element("submission")?.Value, XmlDateTimeSerializationMode.Unspecified),
                                RegistrationNumber = element.Element("registrationnumber")?.Value,
                                Note = element.Element("note")?.Value,
                                Country = element.Element("country")?.Value,
                                Author = element.Element("Author")?.Value,
                                AmountOfPages = XmlConvert.ToInt32(element.Element("pages")?.Value ?? "0")
                            });
                            break;
                        }
                    default:
                        {
                            throw new XmlException($"Incorrect input format: {element?.Name} is not library item");
                        }
                }

                _library = newLibrary;
            }
        }

        public IEnumerable<LibraryItem> GetItems()
        {
            return _library;
        }

        public void PrintLibrary()
        {
            foreach (var libraryItem in _library)
            {
                Console.WriteLine(libraryItem.Info());
            }
        }
    }
}
