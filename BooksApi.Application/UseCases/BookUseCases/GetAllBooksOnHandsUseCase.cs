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
    public class GetAllBooksOnHandsUseCase
    {
        private readonly IBookRepository br;
        public GetAllBooksOnHandsUseCase(IBookRepository br)
        {
            this.br = br;
        }
        public async Task<List<Book>> GetAllBooksOnHands(int userId, CancellationToken cancellationToken)
        {
            var books = await br.FindUserBooksOnHands(userId, cancellationToken);

            if (books == null)
            {
                throw new BookNotFoundException("Пользователь не забирал никаких книг из библиотеки.");
            }

            List<Book> user_books = new List<Book>();

            foreach (Book bk in books)
            {
                if (bk.UserThatGetBook != 0 && bk.ReturnBy < DateTime.Now)
                {
                    bk.BorrowedAt = DateTime.MinValue;
                    bk.ReturnBy = DateTime.MinValue;
                    bk.UserThatGetBook = 0;

                    await br.ChangeBook(bk, cancellationToken);
                }
                else
                {
                    user_books.Add(bk);
                }
            }

            if (user_books.Count == 0)
            {
                throw new BookNotFoundException("Пользователь не забирал никаких книг из библиотеки.");
            }

            return user_books;
        }
    }
}
