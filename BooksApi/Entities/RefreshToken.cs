﻿namespace BooksApi.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public string Token { get; set; }

        public int UserId { get; set; }

        public DateTime ExpiredDate { get; set; }

        public User User { get; set; }
    }
}