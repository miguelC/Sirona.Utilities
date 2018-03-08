using System;
using System.Collections.Generic;
using System.Text;

namespace Sirona.Utilities.Mail
{
  /// <summary>
  /// Mailing exception wrapper
  /// </summary>
  public class NetMailException : ApplicationException
  {
    /// <summary>
    /// Constructor
    /// </summary>
    public NetMailException()
      : base()
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public NetMailException(string message)
      : base(message)
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inner"></param>
    public NetMailException(string message, Exception inner)
      : base(message, inner)
    {
    }
  }
}
