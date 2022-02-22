using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using WebApiWebLoad.Services;

namespace WebApiWebLoad.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UspSelectCBRhistoryController : ControllerBase
    {
        private readonly IDateValidationService _dateValidationService;
        private readonly IUsageStatisticService _usageStatisticService;
        private readonly IUspSelectCBRhistoryControllerService _uspSelectCBRhistoryControllerService;
        public UspSelectCBRhistoryController(IDateValidationService dateValidationService, IUspSelectCBRhistoryControllerService uspSelectCBRhistoryControllerService,
            IUsageStatisticService usageStatisticService)
        {
            this._dateValidationService = dateValidationService;
            this._uspSelectCBRhistoryControllerService = uspSelectCBRhistoryControllerService;
            this._usageStatisticService = usageStatisticService;
        }

        [HttpGet]
        public async Task<List<CbrHistory>> Get(int numCode=0,string dtStartString="",string dtEndString="",string charCode="USD",int days=60)
        {
            //https://srv-2022/load/UspSelectCBRhistory?dtEndString=2019-01-29&charCode=USD&days=60
             try
             {
                 _usageStatisticService.DoIncreaseCurrentRequests(); 
                 List<CbrHistory> cbrHistoryResponse;
                 CbrHistoryRequest cbhr = new CbrHistoryRequest();
                 cbhr.DtStart = (dtStartString == "")
                     ? DateTime.MinValue
                     : _dateValidationService.GetValidDateFromString(dtStartString);
                 cbhr.DtEnd = (dtEndString == "")
                     ? DateTime.MinValue
                     : _dateValidationService.GetValidDateFromString(dtEndString);
                 cbhr.NumCodeID = numCode;
                 cbhr.Days = days;
                 cbhr.CharCode = charCode;

#if !DEBUG
#pragma warning disable CA1416 // Validate platform compatibility
            await WindowsIdentity.RunImpersonatedAsync(((WindowsIdentity)HttpContext.User.Identity).AccessToken,
                async () =>
                {
#endif
                 cbrHistoryResponse = await _uspSelectCBRhistoryControllerService.GetDataAsync(cbhr);
#if !DEBUG
                });
#pragma warning restore CA1416 // Validate platform compatibility
#endif
                 return cbrHistoryResponse;
             }
             catch (Exception)
             {
                 throw;
             }
             finally
             {
                _usageStatisticService.DoDecrementCurrentRequests();
             }
            
        }
    }
    


}
