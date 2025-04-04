﻿using BooksApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.Responses
{
    public record Response(string accessToken, RefreshToken refreshToken);
}