using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace WebApiWebLoad.Services
{

    public class TimedHostedService : IHostedService, IDisposable
    {
        //https://docs.microsoft.com/ru-ru/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-5.0&tabs=visual-studio
     
        private readonly ILogger<TimedHostedService> _logger;
        private readonly IUsageStatisticService _usageStatisticService;
        private Timer _timer;

        public TimedHostedService(ILogger<TimedHostedService> logger, IUsageStatisticService usageStatisticService)
        {
            _logger = logger;
            this._usageStatisticService = usageStatisticService;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
           // _logger.LogInformation("Timed Hosted Service running.");
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(60));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var cRequests = _usageStatisticService.GetNumberOfCurrentRequests();
            var mRequests = _usageStatisticService.GetNumberOfMaximumRequests();
           // _logger.LogWarning($"Requests current: {cRequests} maximum: {mRequests}");
            if (DateTime.Now.Minute==0) _usageStatisticService.DoResetMaximum();
        }


        public Task StopAsync(CancellationToken stoppingToken)
        {
         //   _logger.LogInformation("Timed Hosted Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

}
