﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR.Console
{
    public class ConsolePublishLog : IPublishLog
    {
        public void Log(string message)
        {
            System.Console.WriteLine(message);
        }
    }
}