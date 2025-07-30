using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.User
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime JoinedDate { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
