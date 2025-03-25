namespace BooksApi.Entities
{
    public class User
    {
        public User()
        {
            Email = "";
            Password = "";
            Role = "User";
        }
        public int Id { get; set; }

        public string Role { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
