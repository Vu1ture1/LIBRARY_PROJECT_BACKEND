namespace BooksApi.CustomExceptions
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException() : base("Неверный email или пароль.") { }
    }

    public class EmailAlreadyRegisteredException : Exception
    {
        public EmailAlreadyRegisteredException() : base("Такой email уже зарегистрирован.") { }
    }

    public class InvalidRefreshTokenException : Exception
    {
        public InvalidRefreshTokenException() : base("Refresh token недействителен.") { }
    }

    public class ExpiredRefreshTokenException : Exception
    {
        public ExpiredRefreshTokenException() : base("Refresh token истёк. Пожалуйста, войдите снова.") { }
    }
}
