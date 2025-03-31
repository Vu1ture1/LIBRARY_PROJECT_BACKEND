﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Domain.Entities
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
