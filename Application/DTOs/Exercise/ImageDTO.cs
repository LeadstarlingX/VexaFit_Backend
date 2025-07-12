using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Common;

namespace Application.DTOs.Exercise
{
    public class ImageDTO : BaseDTO<int>
    {
        public string Url { get; set; } = string.Empty;
        public string AlternativeText { get; set; } = string.Empty;
    }
}
