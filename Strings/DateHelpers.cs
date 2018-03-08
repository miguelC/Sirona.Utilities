using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sirona.Utilities.Strings
{
    public class DateHelpers
    {
        public static bool IsDate(String dateValue)
        {
            bool result;

            try
            {
                if (string.IsNullOrEmpty(dateValue))
                {
                    result = false;
                }
                else
                {
                    DateTime dt = DateTime.Parse(dateValue);
                    result = true;
                }
            }
            catch (FormatException ex)
            {
                result = false;
            }
            return result;
        }

        // not so sure if this works correctly
        public static String DateStringFormat(String date)
        {
            String s = "";
            s = date.Substring(4, 2) + "/" + date.Substring(6, 2) + "/" + date.Substring(0, 4);

            if (date.Length >= 12)
            {
                s = s + " " + date.Substring(8, 2) + ":" + date.Substring(10, 2);
            }
            return s;
        }

    }
}
