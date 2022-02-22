using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleWebLoadMuktitask
{
    public interface ICallHttpServer
    {
        Task DoApiGet(string url, int i);
    }

    public class CallHttpServer : ICallHttpServer
    {
            private readonly HttpClient _client;
            private readonly SemaphoreSlim _semaphoreHttp;
            private readonly IUrlPreparation _urlPreparation = new UrlPreparation();
            private readonly IResponseProcessor _responseProcessor = new ResponseProcessor();
            private readonly Config _config;
        public CallHttpServer(HttpClient client, SemaphoreSlim semaphoreSlim, Config config)
        {
                this._client = client;
                this._semaphoreHttp = semaphoreSlim;
                this._config = config;
        }

        public  async Task DoApiGet(string url, int i)
        {
            var payload = "";
            url = _urlPreparation.GetTestUrl(url, i);
            var stopW = new Stopwatch();
            await _semaphoreHttp.WaitAsync();
            stopW.Start();
            try
            {
               payload  = await _client.GetStringAsync(url);
                 
            }
            catch (Exception ex)
            {
                payload = ex.Message;
            }
            stopW.Stop();
            var timeTaken = stopW.Elapsed;
           _responseProcessor.DoResponseProcessing(_config,payload,timeTaken,i);
            _semaphoreHttp.Release();
        }
     
    }
}