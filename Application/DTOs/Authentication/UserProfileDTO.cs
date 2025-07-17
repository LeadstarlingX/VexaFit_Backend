using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Common;

namespace Application.DTOs.Authentication
{
    public class UserProfileDTO : BaseDTO<string>
    {
        public string UserName { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public TokenDTO Token { get; set; } = null!;
    }
}
