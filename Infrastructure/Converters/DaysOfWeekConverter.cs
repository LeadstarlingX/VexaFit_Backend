using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Converters
{
    public class DaysOfWeekConverter : ValueConverter<List<DayOfWeek>, string>
    {
        public DaysOfWeekConverter() : base(
            v => string.Join(",", v.Select(e => e.ToString())),

            v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => Enum.Parse<DayOfWeek>(s)).ToList())
        {
        }
    }
}
