namespace Library
{
    public class Book : LibraryItem
    {
        public string Author { get; set; }
        public string PlaceOfPublication { get; set; }
        public string PublishingHouse { get; set; }
        public int YearOfPublication { get; set; }
        public string Note { get; set; }
        public string ISBN { get; set; }

        public override string Info()
        {
            return $"Book {Name} - {Author}, {ISBN}";
        }
    }
}
