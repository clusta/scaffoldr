﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public interface IJson
    {
        Task<T> Deserialize<T>(string data);
    }
}
