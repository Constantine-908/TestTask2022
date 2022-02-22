using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleWebLoadMuktitask
{
    public interface IResponseProcessor
    {
        void DoResponseProcessing(Config config, string payload, TimeSpan timeTaken, int i);
    }

    public class ResponseProcessor : IResponseProcessor
    {
        private readonly int _consoleWidth = Console.WindowWidth;
        public void DoResponseProcessing(Config config, string payload, TimeSpan timeTaken, int i)
        {
            if (i % config.DisplayRecordsInMultiplesOf == 0 || payload.IndexOf("error", StringComparison.CurrentCultureIgnoreCase) > -1)
                Console.WriteLine($"{i} {DateTime.Now:HH:mm:ss.ff} time:{timeTaken:m\\:ss\\.ffff} " +
                                  $"{((payload.Length > _consoleWidth - 35) ? payload.Substring(0, _consoleWidth - 35) : payload)}");
        }
    }
}
