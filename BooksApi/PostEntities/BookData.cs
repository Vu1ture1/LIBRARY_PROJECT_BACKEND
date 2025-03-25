using BooksApi.Entities;

namespace BooksApi.PostEntities
{
    public class BookData
    {
        public string ISBN { get; set; }
        public string Name { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public int AuthorId { get; set; }

        public IFormFile? ImageFile { get; set; }
    }

    public class BookUpdateData
    {
        public string ISBN { get; set; }
        public string Name { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public int AuthorId { get; set; }

        public string? Image { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
