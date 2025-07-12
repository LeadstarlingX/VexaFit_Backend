using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Common;

namespace Domain.Entities.AppEntities
{
    public class Image : BaseEntity
    {
        public int ImageId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string AlternativeText { get; set; } = string.Empty;

    }
}
