using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksApi.Domain.Entities;
using BooksApi.Application.DTOs;
using AutoMapper;

namespace BooksApi.Application.Mapper
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
