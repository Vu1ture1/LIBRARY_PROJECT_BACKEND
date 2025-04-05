using AutoMapper;
using BooksApi.Application.Exceptions.AuthorExceptions;
using BooksApi.Application.Interfaces;
using BooksApi.Application.UseCasesInterfaces.IAuthorUseCases;
using BooksApi.Domain.Entities;
using BooksApi.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.UseCases.AuthorUseCases
{
    public class DeleteAuthorUseCase : IDeleteAuthorUseCase
    {
        private readonly IAuthorRepository ar;

        private readonly IBookRepository br;

        private IFileService fs;
        public DeleteAuthorUseCase(IAuthorRepository ar, IBookRepository br, IFileService fs)
        {
            this.ar = ar;
            this.br = br;
            this.fs = fs;
        }
        public async Task DeleteAuthor(int id, CancellationToken cancellationToken)
        {
            var existingAuthor = await ar.GetAuthorById(id, cancellationToken);

            if (existingAuthor == null)
            {
                throw new AuthorNotFoundException(id);
            }

            List<Book> books_to_delete = await br.FindBooksByAuthor(id, cancellationToken);

            if (books_to_delete != null || books_to_delete.Count != 0)
            {
                foreach (Book bk in books_to_delete)
                {
                    await br.RemoveBook(bk, cancellationToken);

                    if (bk.Image != "Default.jpg")
                    {
                        fs.DeleteFile(bk.Image, cancellationToken);
                    }
                }
            }

            await ar.RemoveAuthor(existingAuthor, cancellationToken);

            return;
        }
    }
}
