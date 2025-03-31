using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BooksApi.Domain.Entities
{
    public class Book
    {
        public Book()
        {
            ISBN = "";
            Name = "";
            Genre = "";
            Description = "";
            book_author = null;
            UserThatGetBook = 0;

            BorrowedAt = DateTime.MinValue;
            ReturnBy = DateTime.MinValue;
        }
        public int Id { get; set; }
        public string ISBN { get; set; }
        public string Name { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public int AuthorId { get; set; }
        public Author book_author { get; set; }
        public DateTime BorrowedAt { get; set; }
        public DateTime ReturnBy { get; set; }

        public int UserThatGetBook { get; set; }
        public string? Image { get; set; }
    }
}
