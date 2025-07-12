using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Common;

namespace Application.DTOs.MotivationSentence
{
    public class MotivationSentenceDTO : BaseDTO<int>
    {
        public string Text { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
