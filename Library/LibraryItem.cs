namespace Library
{
    public abstract class LibraryItem
    {
        public string Name { get; set; }

        public abstract string Info();
    }
}