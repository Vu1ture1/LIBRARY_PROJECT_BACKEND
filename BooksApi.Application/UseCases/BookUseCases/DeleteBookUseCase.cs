using AutoMapper;
using BooksApi.Application.Exceptions.BookExceptions;
using BooksApi.Application.Interfaces;
using BooksApi.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.UseCases.BookUseCases
{
    public class DeleteBookUseCase
    {
        private readonly IBookRepository br;

        private IFileService fs;
        public DeleteBookUseCase(IBookRepository br, IFileService fs)
        {
            this.br = br;
            this.fs = fs;
        }
        public async Task DeleteBook(int id, CancellationToken cancellationToken)
        {
            var existingBook = await br.GetBookById(id, cancellationToken);

            if (existingBook == null)
            {
                throw new BookNotFoundException(id);
            }

            await br.RemoveBook(existingBook, cancellationToken);

            if (existingBook.Image != "Default.jpg")
            {
                fs.DeleteFile(existingBook.Image, cancellationToken);
            }

            return;
        }
    }
}
