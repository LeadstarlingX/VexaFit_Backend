﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Common
{
    public class BaseDTO<T>
    {
        public T Id { get; set; }
    }
}
