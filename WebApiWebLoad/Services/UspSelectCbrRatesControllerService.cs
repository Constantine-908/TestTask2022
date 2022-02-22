using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WebApiWebLoad.Services;


namespace WebApiWebLoad.Services
{
    public interface IUspSelectCbrRatesControllerService
    {
        Task<ValCurs> GetDataAsync(DateTime dt);
    }

    public class UspSelectCbrRatesControllerService : IUspSelectCbrRatesControllerService
    {
        private readonly IDAL _dal;
        public UspSelectCbrRatesControllerService(IDAL dal)
        {
            this._dal = dal;
        }
      
        public async Task<ValCurs> GetDataAsync(DateTime dateR)
        {
            ValCurs vc = new ValCurs();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            var p = sqlCmd.Parameters.Add("@date_req", SqlDbType.Date);
            p.Value = dateR;
            sqlCmd.CommandText = "usp_selectCBRRates";
            var dt = await _dal.GetTableFromCommandAsync(sqlCmd);
            var vlList = _dal.GetListFromTable<Valute>(dt);
            vc.Valute = vlList;
            vc.Date = dateR.ToString("yyyy-MM-dd");
            return vc;

        }
    }


    [XmlRoot(ElementName = "Valute")]
    public class Valute
    {  
        [XmlAttribute(AttributeName = "ID")]
        public string ID { get; set; }
        [XmlElement(ElementName = "NumCode")]
        public string NumCode { get; set; }
        [XmlElement(ElementName = "CharCode")]
        public string CharCode { get; set; }
        [XmlElement(ElementName = "Nominal")]
        public string Nominal { get; set; }
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "Value")]
        public string Value { get; set; }
      
    }

    [XmlRoot(ElementName = "ValCurs")]
    public class ValCurs
    {
        [XmlAttribute(AttributeName = "Date")]
        public string Date { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; } = "Foreign Currency Market";
        [XmlElement(ElementName = "Valute")]
        public List<Valute> Valute { get; set; }
    }
}
