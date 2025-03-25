namespace BooksApi.CustomExceptions
{
    public class BookNotFoundException : Exception
    {
        public BookNotFoundException(int id) : base($"Книга с id {id} не найдена.") { }
        public BookNotFoundException(string message) : base(message) { }
    }

    public class AuthorBooksNotFoundException : Exception
    {
        public AuthorBooksNotFoundException(int authorId) : base($"Книги автора с id {authorId} не найдены.") { }
    }

    public class BookAlreadyExistsException : Exception
    {
        public BookAlreadyExistsException(string isbn) : base($"Книга с ISBN {isbn} уже существует.") { }
    }

    public class InvalidFileSizeException : Exception
    {
        public InvalidFileSizeException() : base("Размер файла не должен превышать 1MB.") { }
    }

    public class InvalidFileFormatException : Exception
    {
        public InvalidFileFormatException() : base("Неверный формат файла. Разрешены только .jpg, .jpeg, .png.") { }
    }

    public class UnauthorizedUserException : Exception
    {
        public UnauthorizedUserException() : base("Недопустимый или отсутствующий идентификатор пользователя.") { }
    }

}
