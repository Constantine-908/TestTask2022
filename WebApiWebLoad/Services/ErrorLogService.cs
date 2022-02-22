using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiWebLoad.Services
{
    public interface IErrorLogService
    {
        Task AddErrorAsync(Exception ex, string controllerName);
    }

    public class ErrorLogService : IErrorLogService
    {
        public async Task AddErrorAsync(Exception ex,string controllerName)
        {//for future implementation (UDP to syslog ...
            var errorStackTrace = ex.StackTrace;
        }
    }
}
