using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Domain.Entities
{
    public class Author
    {
        public Author()
        {
            Name = "";
            Surname = "";
            Counry = "";
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Counry { get; set; }
        public DateTime BornDate { get; set; }
    }
}
