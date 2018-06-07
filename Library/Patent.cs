using System;

namespace Library
{
    public class Patent : LibraryItem
    {
        public string Author { get; set; }
        public string Country { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime SubmissionDate { get; set; }
        public DateTime PublicationDate { get; set; }
        public int AmountOfPages { get; set; }
        public string Note { get; set; }
        public override string Info()
        {
            return $"Patent {Name} - {Author}, {PublicationDate.ToShortDateString()} {RegistrationNumber}";
        }
    }
}