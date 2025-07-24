using AmsApi.Data;
using System.Diagnostics;
using AmsApi.Services;
using AmsApi.Models;

namespace AmsApi.Services
{
    public class LoggerService:ILoggerService
    {
        private readonly AMSDbContext _aMSDbContext;

        public LoggerService(AMSDbContext aMSDbContext)
        {
            _aMSDbContext = aMSDbContext;
        }
        public void LogEvent(string message, string category, int triggeredBy, string oldValue = null, string newValue = null)
        {
            var log = new EventLogs
            {
                Event = message,
                EventTriggeredBy = triggeredBy,
                Category = category,
                OldValue = oldValue,  // Log the old value if available
                NewValue = newValue   // Log the new value if available
            };
            _aMSDbContext.eventLogs.Add(log);
            _aMSDbContext.SaveChanges();
        }

        public void LogError(string error, string errormessage, string controller)
        {
            var log = new ErrorLog
            {
                Error = error,
                Message = errormessage,
                OccuranceSpace = controller,
            };

            _aMSDbContext.errorLogs.Add(log);
            _aMSDbContext.SaveChanges();
        }
    }
}
