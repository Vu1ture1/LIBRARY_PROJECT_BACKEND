﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksApi.Application.DTOs;
using FluentValidation;

namespace BooksApi.Application.Validators
{
    public class BookDataValidator : AbstractValidator<BookData>
    {
        public BookDataValidator()
        {
            RuleFor(b => b.ISBN)
                .NotEmpty().WithMessage("ISBN не должен быть пустым")
                .Length(13).WithMessage("ISBN должен содержать 13 символов");

            RuleFor(b => b.Name)
                .NotEmpty().WithMessage("Название книги обязательно")
                .MaximumLength(255).WithMessage("Название книги не должно превышать 255 символов");

            RuleFor(b => b.Genre)
                .NotEmpty().WithMessage("Жанр обязателен");

            RuleFor(b => b.Description)
                .NotEmpty().WithMessage("Описание обязательно")
                .MaximumLength(1000).WithMessage("Описание не должно превышать 1000 символов");
        }
    }
}
