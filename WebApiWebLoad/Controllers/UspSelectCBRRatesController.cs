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

    public class UspSelectCbrRatesController : ControllerBase
    {
        private readonly IDateValidationService _dateValidationService;
        private readonly IUspSelectCbrRatesControllerService _uspSelectCbrRatesControllerService;
        private readonly IUsageStatisticService _usageStatisticService;
        public UspSelectCbrRatesController(IDateValidationService dateValidationService, IUspSelectCbrRatesControllerService uspSelectCbrRatesControllerService, IUsageStatisticService usageStatisticService)
        {
            this._dateValidationService = dateValidationService;
            this._uspSelectCbrRatesControllerService = uspSelectCbrRatesControllerService;
            this._usageStatisticService = usageStatisticService;
        }
        [HttpGet]
        public async Task<ValCurs> Get(string date_req)
        {
            //http://localhost:21090/UspSelectCbrRates?date_req=2020-01-01
            try
            {
                _usageStatisticService.DoIncreaseCurrentRequests();
                DateTime dateR = _dateValidationService.GetValidDateFromString(date_req);
                ValCurs vl;
#if !DEBUG

#pragma warning disable CA1416 // Validate platform compatibility
            await WindowsIdentity.RunImpersonatedAsync(((WindowsIdentity)HttpContext.User.Identity).AccessToken,
                async () =>
                {
#endif
                vl = await _uspSelectCbrRatesControllerService.GetDataAsync(dateR);
#if !DEBUG

                });
#pragma warning restore CA1416 // Validate platform compatibility
#endif             
                return vl;
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
