namespace ConsoleWebLoadMuktitask
{
    public class Config
    {
        public int MaxThreads { get; set; } = 100;  
        public int TotalRequests { get; set; } = 1000;
        public string Url { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IgnoreCertificateValidation { get; set; } = false;
        public int DisplayRecordsInMultiplesOf { get; set; } = 100;
    }
}