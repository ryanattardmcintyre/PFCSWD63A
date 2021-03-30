using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Logging.Type;
using Google.Cloud.Logging.V2;

namespace WebApplication1.Services.Interfaces
{
    public interface ILogRepository
    {
        //information, error, warning, ....
        void Log(string message, LogSeverity severity);

        void Log(Exception ex);
    }
}
