﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Authentication
{
    public class TokenDTO
    {
        public string JwtToken { get; set; } = string.Empty;
        public bool Success { get; set; }
        public IList<string> Roles { get; set; } = []!;
    }
}
