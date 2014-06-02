using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR.Providers
{
    public class DebugPublishLog : ILogger
    {
        public void Log(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
