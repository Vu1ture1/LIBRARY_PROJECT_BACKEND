using AutoMapper;
using BooksApi.Application.Exceptions.BookExceptions;
using BooksApi.Application.Interfaces;
using BooksApi.Application.UseCasesInterfaces.IBookUseCases;
using BooksApi.Domain.Entities;
using BooksApi.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.UseCases.BookUseCases
{
    public class FindBooksByNameUseCase : IFindBooksByNameUseCase
    {
        private readonly IBookRepository br;
        public FindBooksByNameUseCase(IBookRepository br)
        {
            this.br = br;
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

                    await br.ChangeBook(bk, cancellationToken);
                }
            }

            return books;
        }
    }
}
