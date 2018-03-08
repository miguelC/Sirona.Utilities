using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sirona.Utilities.Strings
{
    public static class StringHelpers
    {
        public static string GenerateUniqueID(string oid)
        {
            String year, month, day, hours, minutes, seconds, milliseconds;
            DateTime dt = DateTime.Now;
            year = dt.Year.ToString();
            month = dt.Month.ToString();
            day = dt.Day.ToString();
            hours = dt.Hour.ToString();
            minutes = dt.Minute.ToString();
            seconds = dt.Second.ToString();
            milliseconds = dt.Millisecond.ToString();

            oid += "." + year + month + day + "." + hours + minutes + seconds + milliseconds;

            return oid;
        }

        public static string NewUrnUuidGuid()
        {
            return "urn:uuid:" + Guid.NewGuid().ToString();
        }

        public static string BuildSystemOID(string root, string extension)
        {
            return root + (string.IsNullOrEmpty(extension) ? string.Empty : ("." + extension));
        }

        public static string BuildPatientId(string root, string extension)
        {
            return extension + "^^^&" + root + "&ISO";
        }

        public static string ExtractRootFromPatientId(string patientId, string rootExtSeparator, string idTrail)
        {
            if (patientId.EndsWith("'") && string.IsNullOrEmpty(idTrail))
            {
                idTrail = "'";
            }
            if (string.IsNullOrEmpty(idTrail))
            {
                return patientId.Substring(patientId.IndexOf(rootExtSeparator) + rootExtSeparator.Length);
            }
            return StringUtility.ExtractFromDelimiters(patientId, rootExtSeparator, idTrail);
        }

        public static string ExtractExtensionFromPatientId(string patientId, string rootExtSeparator)
        {
            if (patientId.StartsWith("'"))
            {
                patientId = patientId.Substring(1);
            }
            if (patientId.Contains(rootExtSeparator))
            {
                return patientId.Substring(0, patientId.IndexOf(rootExtSeparator));
            }
            return "-1";
        }
        public static string ExtractRootFromPatientId(string patientId)
        {
            return StringUtility.ExtractFromDelimiters(patientId, "^^^&", "&ISO");
        }

        public static string ExtractExtensionFromPatientId(string patientId)
        {
            if (patientId.StartsWith("'"))
            {
                patientId = patientId.Substring(1);
            }
            if (patientId.Contains("^^^&"))
            {
                return patientId.Substring(0, patientId.IndexOf("^^^&"));
            }
            return "-1";
        }

        public static string BuildPatientIdWithUrnOid(string root, string extension)
        {
            return extension + "^^^&urn:oid:" + root + "&ISO";
        }

        public static string ExtractRootFromPatientIdWithUrnOid(string patientId)
        {
            return StringUtility.ExtractFromDelimiters(patientId, "^^^&urn:oid:", "&ISO");
        }

        public static string ExtractExtensionFromPatientIdWithUrnOid(string patientId)
        {
            if (patientId.StartsWith("'"))
            {
                patientId = patientId.Substring(1);
            }
            if (patientId.Contains("^^^&urn:oid:"))
            {
                return patientId.Substring(0, patientId.IndexOf("^^^&"));
            }
            return "-1";
        }
        public static bool IsNumeric(string number)
        {
            //[Ee] - allows for exponents
            // (\d+ \d*) (\d? \d+) - allows for decimal value without number before decimal point
            System.Text.RegularExpressions.Regex r =
                new System.Text.RegularExpressions.Regex(@"(^[-+]?\d+(,?\d*)*\.?\d*([Ee][-+]\d*)?$)|(^[-+]?\d?(,?\d*)*\.\d+([Ee][-+]\d*)?$)");
            return r.Match(number).Success;
        }

    }
}
