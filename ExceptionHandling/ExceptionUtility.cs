using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sirona.Utilities.ExceptionHandling
{
    public static class ExceptionUtility
    {
        /// <summary>
        /// Returns a string with the error messages from an exception and its inner exceptions
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string GetExceptionMessages(Exception exception)
        {
            return GetExceptionMessages(exception, true);
        }
        /// <summary>
        /// 
        /// Returns a string with the error messages from an exception and its inner exceptions
        /// It tabs inner exceptons by level so:
        /// MainException
        ///   Level 1 Inner Exception
        ///     Level 2 Inner Exception
        /// .... etc.
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="tabbedLevels"></param>
        /// <returns></returns>
        public static string GetExceptionMessages(Exception exception, bool tabbedLevels)
        {
            if (exception != null)
            {
                System.Text.StringBuilder message = new System.Text.StringBuilder();
                message.Append("Exception: ").Append(exception.Message).Append(System.Environment.NewLine);
                if (exception.InnerException != null)
                {
                    message.Append(GetInnerExceptionMessages(exception.InnerException, 1, tabbedLevels, false));
                }
                return message.ToString();
            }
            return "";
        }
        /// <summary>
        /// Returns a string with the error messages from an exception and its inner exceptions, including the stack trace of all exceptions
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string GetExceptionMessagesWithStackTrace(Exception exception)
        {
            return GetExceptionMessagesWithStackTrace(exception, true);
        }
        /// <summary>
        /// Returns a string with the error messages from an exception and its inner exceptions, including the stack trace of all exceptions
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="tabbedLevels"></param>
        /// <returns></returns>
        public static string GetExceptionMessagesWithStackTrace(Exception exception, bool tabbedLevels)
        {
            if (exception != null)
            {
                System.Text.StringBuilder message = new System.Text.StringBuilder();
                message.Append("Exception: ").Append(exception.Message).Append(System.Environment.NewLine);
                message.Append("Stacktrace: ").Append(exception.StackTrace);
                if (exception.InnerException != null)
                {
                    message.Append(GetInnerExceptionMessages(exception.InnerException, 1, tabbedLevels, true));
                }
                return message.ToString();
            }
            return "";
        }
        private static string GetInnerExceptionMessages(Exception innerException, int level, bool tabbed, bool withStackTrace)
        {
            System.Text.StringBuilder message = new System.Text.StringBuilder().Append(System.Environment.NewLine).Append(System.Environment.NewLine);

            string tab = "";
            for (int i = 0; i <= level; i++)
            {
                tab += "\t";
            }
            message.Append(System.Environment.NewLine);
            message.Append(tab).Append("Inner Exception Level ").Append(level).Append(": ");
            if (withStackTrace)
            {
                message.Append(innerException.Message).Append(System.Environment.NewLine);
                message.Append(tab).Append("Stacktrace: ").Append(innerException.StackTrace);
            }
            if (innerException.InnerException != null)
            {
                message.Append(GetInnerExceptionMessages(innerException.InnerException, level++, tabbed, withStackTrace));
            }
            return message.ToString();

        }
    }
}
