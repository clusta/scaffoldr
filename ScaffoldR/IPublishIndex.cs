﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public interface IPublishIndex
    {
        Task AddOrUpdate(string containerName, object page);
    }
}
