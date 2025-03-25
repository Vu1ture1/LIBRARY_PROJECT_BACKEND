using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BooksApi.PostEntities
{
    public class AuthorData
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Counry { get; set; }
        public DateTime BornDate { get; set; }
    }
}
