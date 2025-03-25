namespace BooksApi.CustomExceptions
{
    public class AuthorNotFoundException : Exception
    {
        public AuthorNotFoundException(int id) : base($"Автор с id {id} не найден.") { }
        public AuthorNotFoundException() : base("Автор не найден.") { }
    }

    public class AuthorAlreadyExistsException : Exception
    {
        public AuthorAlreadyExistsException(string name, string surname)
            : base($"Автор {name} {surname} уже существует.") { }
    }

    public class InvalidAuthorDataException : Exception
    {
        public InvalidAuthorDataException() : base("Предоставлены недействительные данные об авторе.") { }
    }

}