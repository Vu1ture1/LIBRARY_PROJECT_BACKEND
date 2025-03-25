using FluentValidation;

namespace BooksApi.PostEntities.EntitiesValidators
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
