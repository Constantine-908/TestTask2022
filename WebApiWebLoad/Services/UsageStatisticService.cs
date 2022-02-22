using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiWebLoad.Services
{
    public interface IUsageStatisticService
    {
        void DoIncreaseCurrentRequests();
        void DoDecrementCurrentRequests();
        public void DoResetMaximum();
        long GetNumberOfCurrentRequests(); 
        long GetNumberOfMaximumRequests();
        
    }

    public class UsageStatisticService : IUsageStatisticService
    {
        private  long _currentRequests  = 0;
        private long _maximumRequests = 0;

        public void DoIncreaseCurrentRequests()
        {
            long cr=Interlocked.Increment(ref _currentRequests);
            if (cr > _maximumRequests)
               Interlocked.Exchange(ref _maximumRequests ,cr) ;
        }
        public void DoDecrementCurrentRequests()
        {
            Interlocked.Decrement(ref _currentRequests);
        }

        public void DoResetMaximum()
        {
            Interlocked.Exchange(ref _maximumRequests, 0);
        }

        public long GetNumberOfCurrentRequests()
        {
            return Interlocked.Read( ref _currentRequests);
        }
        public long GetNumberOfMaximumRequests()
        {
            return Interlocked.Read(ref _maximumRequests);
        }

    }
}
