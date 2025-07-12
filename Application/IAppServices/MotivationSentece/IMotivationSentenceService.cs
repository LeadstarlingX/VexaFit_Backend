using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.MotivationSentence;
using Application.IAppServices.Common;

namespace Application.IAppServices.MotivationSentece
{
    public interface IMotivationSentenceService : IService<MotivationSentenceDTO,
        CreateMotivationSentenceDTO, UpdateMotivationSentenceDTO>
    {
    }
}
