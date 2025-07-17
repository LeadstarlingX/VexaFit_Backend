using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Common;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Exercise
{
    public class ImageDTO : BaseDTO<int>
    {
        public string AlternativeText { get; set; } = string.Empty;
        public IFormFile? ImageFile { get; set; }
    }
}
