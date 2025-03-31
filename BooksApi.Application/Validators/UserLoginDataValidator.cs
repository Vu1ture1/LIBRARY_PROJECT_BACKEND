using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksApi.Application.DTOs;
using FluentValidation;

namespace BooksApi.Application.Validators
{
    public class UserLoginDataValidator : AbstractValidator<UserLoginData>
    {
        public UserLoginDataValidator()
        {
            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email обязателен.")
                .EmailAddress().WithMessage("Неверный формат Email.");

            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("Пароль обязателен.")
                .MinimumLength(6).WithMessage("Пароль должен содержать минимум 6 символов.");
        }
    }
}
