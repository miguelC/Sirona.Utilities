using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Security;

namespace Sirona.Utilities.Logging
{
    public class Log
    {
        /// <summary>
        /// Writes to system event log
        /// </summary>
        /// <param name="logName">Sirona</param>
        /// <param name="sourceName">Application or module</param>
        /// <param name="entryMessage">message to log</param>
        /// <param name="logEntryType">Error, Warning, Information, SuccessAudit, FailureAudit</param>
        public static void WriteToEventLog(string logName, string sourceName, string entryMessage, EventLogEntryType logEntryType)
        {
            try
            {
                if ((!EventLog.SourceExists(sourceName)))
                    EventLog.CreateEventSource(sourceName, "Sirona");
            }
            catch (System.Security.SecurityException e)
            {
                sourceName = "Web Error";
            }

            EventLog.WriteEntry(sourceName, entryMessage, logEntryType, 0);
        }

        /// <summary>
        /// Recurses through exceptions to return the full trail of [inner]exception messages
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>nested exception message</returns>
        public static string ReturnNestedExceptionMessage(System.Exception ex)
        {
            string message = ex.Message;
            if (ex.InnerException != null)
                message = message + "(" + ex.Source + ":" + ReturnNestedExceptionMessage(ex.InnerException) + ")";

            return message;
        }

    }
}
