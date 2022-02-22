using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiWebLoad.Services
{
    public interface IDateValidationService
    {
        DateTime GetValidDateFromString(string dtString);
    }

    public class DateValidationService : IDateValidationService
    {
        public DateTime GetValidDateFromString(string dtString)
        {
            //return valid date from 1994 to now or now
            DateTime dateR;
            DateTime.TryParse(dtString, out dateR);
            if (dateR < new DateTime(1993, 01, 01) || dateR > DateTime.Now)
                dateR = DateTime.Now;
            return dateR;
        }
    }
}
