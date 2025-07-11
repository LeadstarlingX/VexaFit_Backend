using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    public class IdentityAppDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {

    }
}
