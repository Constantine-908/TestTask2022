using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using WebApiWebLoad.Services;

namespace WebApiWebLoad.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IDAL _dal;
        private readonly IUsageStatisticService _usageStatisticService;

        public StatusController(IDAL dal, IUsageStatisticService usageStatisticService)
        {
            this._dal = dal;
            this._usageStatisticService = usageStatisticService;
        }
        [HttpGet]
        //https://srv-2022/load/Status
        public async Task<string> Get()
        {
            var timer = new Stopwatch();
            timer.Start();
            var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
            string ipStr = remoteIpAddress.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"];
            var user1 = HttpContext.User.Identity.Name;//  .Current.User.Identity.Name;
            var user2 = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            DataTable dt;
#if !DEBUG

            await WindowsIdentity.RunImpersonatedAsync(((WindowsIdentity) HttpContext.User.Identity).AccessToken,
                    async () =>
                   {
#endif
            dt = await _dal.GetTableFromSqlTextAsync("[testDB].[dbo].[usp_status]");
#if !DEBUG

                   });
#endif
            var cRequests = _usageStatisticService.GetNumberOfCurrentRequests();
            var mRequests = _usageStatisticService.GetNumberOfMaximumRequests();
            timer.Stop();
            TimeSpan timeTaken = timer.Elapsed;
            return $" {timeTaken:m\\:ss\\.fff} | CurReq: {cRequests} MaxReq: {mRequests} | ip: {ipStr} | IIS User {user1} | APPpool User {user2} " +
                   $"| SQL User {dt.Rows[0][1]} | SQL Server {dt.Rows[0][0]}" +
                $" | agent {userAgent} ";
        }
    }
}
