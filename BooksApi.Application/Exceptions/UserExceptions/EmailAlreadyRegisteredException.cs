﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.Exceptions.UserExceptions
{
    public class EmailAlreadyRegisteredException : Exception
    {
        public EmailAlreadyRegisteredException() : base("Такой email уже зарегистрирован.") { }
    }
}
