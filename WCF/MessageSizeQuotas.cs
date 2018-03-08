using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sirona.Utilities.WCF
{
    public class MessageSizeQuotas
    {
        public int MaxBufferPoolSize { get; set; }
        public int MaxReceivedMessageSize { get; set; }
        public int MaxBufferSize { get; set; }
    }
}
