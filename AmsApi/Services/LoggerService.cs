using AmsApi.Data;
using AmsApi.Models;

namespace AmsApi.Services
{
    public class LoggerService : ILoggerService
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
                OldValue = oldValue,
                NewValue = newValue,
                CreatedAt = DateTime.Now
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
                CreatedAt = DateTime.Now
            };

            _aMSDbContext.errorLogs.Add(log);
            _aMSDbContext.SaveChanges();
        }
    }
}
