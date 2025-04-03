using AutoMapper;
using BooksApi.Application.Exceptions.BookExceptions;
using BooksApi.Application.Interfaces;
using BooksApi.Domain.Entities;
using BooksApi.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.UseCases.BookUseCases
{
    public class FindBooksByAuthorUseCase
    {
        private readonly IBookRepository br;
        public FindBooksByAuthorUseCase(IBookRepository br)
        {
            this.br = br;
        }
        public async Task<List<Book>> FindBooksByAuthor(int author_id, CancellationToken cancellationToken)
        {
            var books = await br.FindBooksByAuthor(author_id, cancellationToken);

            if (books == null)
            {
                throw new AuthorBooksNotFoundException(author_id);
            }

            foreach (Book bk in books)
            {
                if (bk.UserThatGetBook != 0 && bk.ReturnBy < DateTime.Now)
                {
                    bk.BorrowedAt = DateTime.MinValue;
                    bk.ReturnBy = DateTime.MinValue;
                    bk.UserThatGetBook = 0;

                    await br.ChangeBook(bk, cancellationToken);
                }
            }

            return books;
        }
    }
}
