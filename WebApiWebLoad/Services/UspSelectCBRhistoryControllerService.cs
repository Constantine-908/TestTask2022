using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiWebLoad.Services
{
    public interface IUspSelectCBRhistoryControllerService
    {
        Task<List<CbrHistory>> GetDataAsync(CbrHistoryRequest chr);
    }

    public class UspSelectCBRhistoryControllerService : IUspSelectCBRhistoryControllerService
    {
        private readonly IDAL _dal;
        public UspSelectCBRhistoryControllerService(IDAL dal)
        {
            this._dal = dal;
        }
        public async Task<List<CbrHistory>> GetDataAsync(CbrHistoryRequest chr)
        {
            
            var dt=await _dal.GetTableFromClassAsync<CbrHistoryRequest>(chr, "usp_selectCBRhistory");
            var ch= _dal.GetListFromTable<CbrHistory>(dt);
            return ch;
        }
    }

    public class CbrHistory
    {
        public DateTime Date { get; set; }
        public float Value { get; set; }
        public int Nominal { get; set; }

    }
    public class CbrHistoryRequest
    {
        public int NumCodeID { get; set; } = 0;
        public string CharCode  { get; set; }
        public DateTime DtStart { get; set; }
        public DateTime DtEnd { get; set; }
        public int Days { get; set; }
    }
}
