using AutoMapper;
using BooksApi.Application.DTOs;
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
    public class AddBookUseCase
    {
        private readonly IBookRepository br;

        private IFileService fs;

        private readonly IMapper mapper;
        public AddBookUseCase(IBookRepository br, IFileService fs, IMapper mapper)
        {
            this.br = br;
            this.fs = fs;
            this.mapper = mapper;
        }
        public async Task AddBook(BookData bkDTO, CancellationToken cancellationToken)
        {
            if (bkDTO.ImageFile != null && bkDTO.ImageFile?.Length > 1 * 1024 * 1024)
            {
                throw new InvalidFileSizeException();
            }

            if (await br.GetBookByISBN(bkDTO.ISBN, cancellationToken) != null)
            {
                throw new BookAlreadyExistsException(bkDTO.ISBN);
            }

            string[] allowedFileExtentions = [".jpg", ".jpeg", ".png"];

            string createdImageName = await fs.SaveFileAsync(bkDTO.ImageFile, allowedFileExtentions, cancellationToken);

            var book = mapper.Map<Book>(bkDTO);

            book.Image = createdImageName;

            await br.AddBook(book, cancellationToken);

            return;
        }
    }
}
