namespace Book_eCommerce_Store.Models
{
    public class Book : Product
    {
        public Book() { }
        public Book(int Id, string Name, string Description, int priceInCent, int Quantity,
            string Genre, string Author, int PageCount, string Publisher, DateTime PublicationDate)
        {
            this.Id = Id; 
            this.Name= Name; 
            this.Description = Description;
            this.PriceInCent = priceInCent;
            this.Quantity = Quantity;

            this.Genre = Genre;
            this.Author = Author;
            this.PageCount = PageCount;
            this.Publisher = Publisher;
            this.PublicationDate = PublicationDate;

        }

        public string? Genre { get; set; }
        public string? Author { get; set; }
        public int? PageCount { get; set; }
        public string? Publisher { get; set; }
        public DateTime? PublicationDate { get; set; }

    }
}
