using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sirona.Utilities.XML
{
    public class XslExtensionFunctions
    {
        public string Concat(string arg1, string separ, string arg2)
        {
            return arg1 + separ + arg2;
        }
        public string CurrentDateTime()
        {
            return (DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"));
        }
        public string YearFromDateTime(string dateTime)
        {
            DateTime dt = DateTime.Parse(dateTime);
            return dt.Year.ToString();
        }
        public string MonthFromDateTime(string dateTime)
        {
            DateTime dt = DateTime.Parse(dateTime);
            return dt.Month.ToString();
        }
        public string DayFromDateTime(string dateTime)
        {
            DateTime dt = DateTime.Parse(dateTime);
            return dt.Day.ToString();
        }
        public string Hl7Date(string dateTime)
        {
            DateTime dt = DateTime.Parse(dateTime);
            return dt.ToString("yyyyMMdd");
        }
        public string Hl7Timestamp(string dateTime)
        {
            DateTime dt = DateTime.Parse(dateTime);
            return dt.ToString("yyyyMMddhhmmss");
        }
        public string Hl7Time(string dateTime)
        {
            DateTime dt = DateTime.Parse(dateTime);
            return dt.ToString("hhmmss");
        }
    }
}
