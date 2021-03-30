using Google.Api;
using Google.Cloud.Logging.Type;
using Google.Cloud.Logging.V2;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.Repositories
{
    public class LogRepository : ILogRepository
    {
        string projId;
        public LogRepository(IConfiguration config)
        {
             projId = config.GetSection("AppSettings").GetSection("ProjectId").Value;
        }

        public void Log(string message, LogSeverity severity)
        {

            var client = LoggingServiceV2Client.Create();
            LogName logName = new LogName(projId, "PFCSWD63a"); //logid = random
            LogEntry logEntry = new LogEntry
            {
                LogName = logName.ToString(),
                Severity = severity,
                TextPayload = $"{message}"
            };
           
            MonitoredResource resource = new MonitoredResource { Type = "global" }; //the cloud technologies that you are monitoring
            //IDictionary<string, string> entryLabels = new Dictionary<string, string>
            //{
            //    { "size", "large" },
            //    { "color", "red" }
            //};
            client.WriteLogEntries(logName, resource, null, new List<LogEntry>() { logEntry });
        }

        public void Log(Exception ex)
        {
            throw new NotImplementedException();
        }
    }
}
