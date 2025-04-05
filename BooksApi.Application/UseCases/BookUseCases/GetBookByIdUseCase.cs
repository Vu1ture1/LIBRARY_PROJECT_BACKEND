using AutoMapper;
using BooksApi.Application.Exceptions.BookExceptions;
using BooksApi.Application.Interfaces;
using BooksApi.Application.UseCasesInterfaces.IBookUseCases;
using BooksApi.Domain.Entities;
using BooksApi.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.UseCases.BookUseCases
{
    public class GetBookByIdUseCase : IGetBookByIdUseCase
    {
        private readonly IBookRepository br;

        public GetBookByIdUseCase(IBookRepository br) 
        {
            this.br = br;
        }
        public async Task<Book> GetBookById(int id, CancellationToken cancellationToken)
        {
            var book = await br.GetBookById(id, cancellationToken);

            if (book == null)
            {
                throw new BookNotFoundException(id);
            }

            if (book.UserThatGetBook != 0 && book.ReturnBy < DateTime.Now)
            {
                book.BorrowedAt = DateTime.MinValue;
                book.ReturnBy = DateTime.MinValue;
                book.UserThatGetBook = 0;

                await br.ChangeBook(book, cancellationToken);
            }

            return book;
        }
    }
}
