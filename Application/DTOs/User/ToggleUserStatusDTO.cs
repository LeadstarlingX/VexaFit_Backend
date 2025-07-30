using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.User
{
    public class ToggleUserStatusDTO
    {
        [Required]
        public string UserId { get; set; }
    }
}
