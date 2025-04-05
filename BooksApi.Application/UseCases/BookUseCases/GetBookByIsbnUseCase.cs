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
    public class GetBookByIsbnUseCase : IGetBookByIsbnUseCase
    {
        private readonly IBookRepository br;
        public GetBookByIsbnUseCase(IBookRepository br)
        {
            this.br = br;
        }
        public async Task<Book> GetBookByISBN(string ISBN, CancellationToken cancellationToken)
        {
            var book = await br.GetBookByISBN(ISBN, cancellationToken);

            if (book == null)
            {
                throw new BookNotFoundException($"Книги с таким isbn: {ISBN} нет.");
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
