using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration.Json;

namespace ConsoleWebLoadMuktitask
{
    class ProgramMultitask
    {
        private static SemaphoreSlim _semaphoreHttp;
        private static readonly HttpClientHandler ClientHandler= new HttpClientHandler();
        private static readonly HttpClient Client= new HttpClient(ClientHandler);
        private static ICallHttpServer _callHttpServer;
        private static Config _config;
        static async Task Main(string[] args)
        {
            string configFile = "appsettings.json";
            if (args.Length > 0) configFile = args[0];
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile(configFile);
            IConfigurationRoot configuration = builder.Build();
            _config=configuration.Get<Config>();

            _semaphoreHttp = new SemaphoreSlim(_config.MaxThreads);
            
            if (_config.IgnoreCertificateValidation)
            ClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            Client.DefaultRequestHeaders.Add("User-Agent", Assembly.GetExecutingAssembly().GetName().Name);
            var byteArray = Encoding.ASCII.GetBytes($"{_config.UserName}:{_config.Password}");
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            
            _callHttpServer = new CallHttpServer(Client,_semaphoreHttp,_config);
            
            Task.WaitAll(CallOtherAPI().ToArray());
        }

        public static IEnumerable<Task> CallOtherAPI()
        {
            for (int i = 0; i < _config.TotalRequests; i++)
            {
                yield return _callHttpServer.DoApiGet(_config.Url,i);
            }
        }
    }
}
