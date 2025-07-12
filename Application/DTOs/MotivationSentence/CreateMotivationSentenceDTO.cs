using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.MotivationSentence
{
    public class CreateMotivationSentenceDTO
    {
        [Required]
        [StringLength(10, ErrorMessage = "Motivation Sentence should be 10 characters minimum")]
        public string Text { get; set; } = string.Empty;
        [Required]
        public bool IsActive { get; set; }
    }
}
