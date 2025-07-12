using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Common;

namespace Domain.Entities.AppEntities
{
    public class MotivationSentence : BaseEntity
    {
        public int SentenceId { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
