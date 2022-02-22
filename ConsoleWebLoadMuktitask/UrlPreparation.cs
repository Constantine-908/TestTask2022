using System;

namespace ConsoleWebLoadMuktitask
{
    public interface IUrlPreparation
    {
        string GetTestUrl(string url, int i);
    }

    public class UrlPreparation : IUrlPreparation
    {
        private readonly Random Random = new Random();
        public string GetTestUrl(string url, int i)
        {
            string url1 = url;
            if (url.Contains("UspAddIdAsync", StringComparison.CurrentCultureIgnoreCase))
            {
                url1 = $"{url}?i={i}";
            }
            if (url.Contains("UspSelectCbrRates", StringComparison.CurrentCultureIgnoreCase))
            {
                int daysMinus = Random.Next(7500); //7500-20.5 years
                DateTime dt = DateTime.Now.AddDays(-1 * daysMinus);
                url1 = $"{url}?date_req={dt:yyyy-MM-dd}";
            }
            //https://srv-2022/load/UspSelectCBRhistory?dtEndString=2019-01-29&charCode=USD&days=60
            if (url.Contains("UspSelectCBRhistory", StringComparison.CurrentCultureIgnoreCase))
            {
                int daysMinus = Random.Next(7500); 
                int daysRequest = Random.Next(20, 60);
                DateTime dt = DateTime.Now.AddDays(-1 * daysMinus);
                url1 = $"{url}?dtEndString={dt:yyyy-MM-dd}&charCode=USD&days={daysRequest}";

            }

            return url1;
        }
    }
}