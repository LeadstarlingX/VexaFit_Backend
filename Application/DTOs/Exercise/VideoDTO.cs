using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Common;

namespace Application.DTOs.Exercise
{
    public class VideoDTO : BaseDTO<int>
    {
        public string Url { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
