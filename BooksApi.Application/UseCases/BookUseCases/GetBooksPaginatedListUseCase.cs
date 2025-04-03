using AutoMapper;
using BooksApi.Application.Interfaces;
using BooksApi.Domain.Common;
using BooksApi.Domain.Entities;
using BooksApi.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.UseCases.BookUseCases
{
    public class GetBooksPaginatedListUseCase
    {
        private readonly IBookRepository br;
        public GetBooksPaginatedListUseCase(IBookRepository br)
        {
            this.br = br;
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

                    await br.ChangeBook(bk, cancellationToken);
                }
            }

            return books;
        }
    }
}
