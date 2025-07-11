using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.IdentityEntites
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Identity.IdentityRole&lt;System.Guid&gt;" />
    public class ApplicationRole : IdentityRole<Guid>
    {
    }
}
