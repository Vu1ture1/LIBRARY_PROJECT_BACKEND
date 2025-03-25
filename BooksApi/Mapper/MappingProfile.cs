using AutoMapper;
using BooksApi.Entities;
using BooksApi.PostEntities;

namespace BooksApi.Mapper
{
   public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // **Author**
            CreateMap<Author, AuthorData>().ReverseMap();

            // **Book**
            CreateMap<Book, BookData>().ReverseMap();
            CreateMap<Book, BookUpdateData>().ReverseMap();

            // **User**
            CreateMap<User, UserLoginData>().ReverseMap();

            // **Refresh Token**
            CreateMap<RefreshToken, RefreshTokenRequest>().ReverseMap();
        }
   }
}
