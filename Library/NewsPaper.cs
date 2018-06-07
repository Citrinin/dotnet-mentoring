using System;

namespace Library
{
    public class NewsPaper : LibraryItem
    {
        public string PlaceOfPublication { get; set; }
        public string PublishingHouse { get; set; }
        public int YearOfPublication { get; set; }
        public int AmountOfPages { get; set; }
        public string Note { get; set; }
        public int Number { get; set; }
        public DateTime Date { get; set; }
        public string ISSN { get; set; }
        public override string Info()
        {
            return $"Newspaper {Name} - {Number}, {ISSN}, {Date.ToShortDateString()}";
        }
    }
}