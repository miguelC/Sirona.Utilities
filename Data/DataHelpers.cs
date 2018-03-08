using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Sirona.Utilities.Data
{
    public static class DataHelpers
    {
        public static string CheckEmptyDataRowItem(DataRow row, string rowName, string nullValue)
        {
            try
            {
                return row[rowName].ToString();
            }
            catch (System.ArgumentException)
            {
                return nullValue;
            }
        }

    }
}
