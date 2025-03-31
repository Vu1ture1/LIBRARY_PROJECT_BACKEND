using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.DTOs
{
    public class AuthorData
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Counry { get; set; }
        public DateTime BornDate { get; set; }
    }
}
