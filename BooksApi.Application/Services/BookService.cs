using AutoMapper;
using BooksApi.Application.DTOs;
using BooksApi.Application.Interfaces;
using BooksApi.Domain.Common;
using BooksApi.Domain.Entities;
using BooksApi.Domain.Exceptions.BookExceptions;
using BooksApi.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace BooksApi.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository br;

        private IFileService fs;

        private readonly IMapper mapper;
        public BookService(IBookRepository br, IFileService fs, IMapper mapper)
        {
            this.br = br;
            this.fs = fs;
            this.mapper = mapper;
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

                await br.ChangeBook(book.Id, book, cancellationToken);
            }

            return book;
        }
        public async Task<PaginatedList<Book>> GetBooksPagList(int pageIndex, int pageSize, string? cat, int? authorId, CancellationToken cancellationToken)
        {
            PaginatedList<Book> books = null;

            if (cat == null && authorId == null)
            {
                books = await br.GetBooks(pageIndex, pageSize, cancellationToken);
            }
            else if (cat != null && authorId == null)
            {
                books = await br.GetBooksByCat(pageIndex, pageSize, cat, cancellationToken);
            }
            else if (cat == null && authorId != null)
            {
                books = await br.GetBooksByAuthor(pageIndex, pageSize, authorId.Value, cancellationToken);
            }
            else 
            {
                books = await br.GetBooksByAll(pageIndex, pageSize, cat, authorId.Value, cancellationToken);
            }

            foreach (Book bk in books.Items)
            {
                if (bk.UserThatGetBook != 0 && bk.ReturnBy < DateTime.Now)
                {
                    bk.BorrowedAt = DateTime.MinValue;
                    bk.ReturnBy = DateTime.MinValue;
                    bk.UserThatGetBook = 0;

                    await br.ChangeBook(bk.Id, bk, cancellationToken);
                }
            }

            return books;
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

                await br.ChangeBook(book.Id, book, cancellationToken);
            }

            return book;
        }
        public async Task GiveBookOnHands(int id, int userId, CancellationToken cancellationToken)
        {
            var book = await br.GetBookById(id, cancellationToken);

            if (book == null)
            {
                throw new BookNotFoundException(id);
            }

            await br.GiveBookToUser(id, userId, cancellationToken);

            return;
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

                    await br.ChangeBook(bk.Id, bk, cancellationToken);
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
        public async Task AddBook(BookData bkDTO, CancellationToken cancellationToken)
        {
            if (bkDTO.ImageFile != null && bkDTO.ImageFile?.Length > 1 * 1024 * 1024)
            {
                throw new InvalidFileSizeException();
            }

            string[] allowedFileExtentions = [".jpg", ".jpeg", ".png"];

            string createdImageName = await fs.SaveFileAsync(bkDTO.ImageFile, allowedFileExtentions, cancellationToken);

            var book = mapper.Map<Book>(bkDTO);

            book.Image = createdImageName;

            await br.AddBook(book, cancellationToken);

            return;
        }
        public async Task ChangeBook(int id, BookUpdateData bkDTO, CancellationToken cancellationToken)
        {
            var existingBook = await br.GetBookById(id, cancellationToken);

            if (existingBook == null)
            {
                throw new BookNotFoundException(id);
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

            await br.ChangeBook(id, existingBook, cancellationToken);

            if (bkDTO.ImageFile != null && oldImage != "Default.jpg")
                fs.DeleteFile(oldImage, cancellationToken);

            return;
        }
        public async Task DeleteBook(int id, CancellationToken cancellationToken)
        {
            var existingBook = await br.GetBookById(id, cancellationToken);

            if (existingBook == null)
            {
                throw new BookNotFoundException(id);
            }

            await br.RemoveBook(id, cancellationToken);

            if (existingBook.Image != "Default.jpg")
            {
                fs.DeleteFile(existingBook.Image, cancellationToken);
            }

            return;
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

                    await br.ChangeBook(bk.Id, bk, cancellationToken);
                }
            }

            return books;
        }
        public async Task<ActionResult<List<Book>>> FindBooksByName(string? book_subname, CancellationToken cancellationToken)
        {
            if (book_subname == null)
            {
                throw new BookNotFoundException($"Книг с указанной подстрокой имени: null не найдено.");
            }

            var books = await br.FindBookBySubstringName(book_subname, cancellationToken);

            if (books == null)
            {
                throw new BookNotFoundException($"Книг с указанной подстрокой имени: {book_subname} не найдено.");
            }

            foreach (Book bk in books)
            {
                if (bk.UserThatGetBook != 0 && bk.ReturnBy < DateTime.Now)
                {
                    bk.BorrowedAt = DateTime.MinValue;
                    bk.ReturnBy = DateTime.MinValue;
                    bk.UserThatGetBook = 0;

                    await br.ChangeBook(bk.Id, bk, cancellationToken);
                }
            }

            return books;
        }
    }
}
