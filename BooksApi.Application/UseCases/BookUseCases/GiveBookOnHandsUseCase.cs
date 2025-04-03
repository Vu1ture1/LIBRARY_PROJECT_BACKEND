using AutoMapper;
using BooksApi.Application.Exceptions.BookExceptions;
using BooksApi.Application.Exceptions.UserExceptions;
using BooksApi.Application.Interfaces;
using BooksApi.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.UseCases.BookUseCases
{
    public class GiveBookOnHandsUseCase
    {
        private readonly IBookRepository br;

        private readonly IUserRepository ur;
        public GiveBookOnHandsUseCase(IBookRepository br, IUserRepository ur)
        {
            this.br = br;
            this.ur = ur;
        }
        public async Task GiveBookOnHands(int id, int userId, CancellationToken cancellationToken)
        {
            var book = await br.GetBookById(id, cancellationToken);

            var user = await ur.GetUserById(id, cancellationToken);

            if (user == null)
            {
                throw new UserNotFoundException();
            }

            if (book == null)
            {
                throw new BookNotFoundException(id);
            }

            if (book.UserThatGetBook != 0)
            {
                throw new BookOnHandsException();
            }

            await br.GiveBookToUser(id, userId, cancellationToken);

            return;
        }
    }
}
