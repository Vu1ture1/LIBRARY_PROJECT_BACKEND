using AutoMapper;
using BooksApi.Application.DTOs;
using BooksApi.Application.Exceptions.BookExceptions;
using BooksApi.Application.Interfaces;
using BooksApi.Application.UseCasesInterfaces.IBookUseCases;
using BooksApi.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.UseCases.BookUseCases
{
    public class ChangeBookUseCase : IChangeBookUseCase
    {
        private readonly IBookRepository br;

        private IFileService fs;

        private readonly IMapper mapper;
        public ChangeBookUseCase(IBookRepository br, IFileService fs, IMapper mapper)
        {
            this.br = br;
            this.fs = fs;
            this.mapper = mapper;
        }
        public async Task ChangeBook(int id, BookUpdateData bkDTO, CancellationToken cancellationToken)
        {
            var existingBook = await br.GetBookById(id, cancellationToken);

            if (existingBook == null)
            {
                throw new BookNotFoundException(id);
            }

            if (bkDTO.ISBN != existingBook.ISBN && await br.GetBookByISBN(bkDTO.ISBN, cancellationToken) != null)
            {
                throw new BookAlreadyExistsException(bkDTO.ISBN);
            }

            string oldImage = existingBook.Image;

            if (bkDTO.ImageFile != null)
            {
                if (bkDTO.ImageFile?.Length > 1 * 1024 * 1024)
                {
                    throw new InvalidFileSizeException();
                }

                string[] allowedFileExtentions = [".jpg", ".jpeg", ".png"];

                string createdImageName = await fs.SaveFileAsync(bkDTO.ImageFile, allowedFileExtentions, cancellationToken);

                bkDTO.Image = createdImageName;
            }
            else
            {
                bkDTO.Image = existingBook.Image;
            }

            mapper.Map(bkDTO, existingBook);

            await br.ChangeBook(existingBook, cancellationToken);

            if (bkDTO.ImageFile != null && oldImage != "Default.jpg")
                fs.DeleteFile(oldImage, cancellationToken);

            return;
        }
    }
}
