using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sirona.Utilities.WCF
{
    public class TimeoutQuotas
    {
        public long CloseTimeout { get; set; }
        public long OpenTimeout { get; set; }
        public long ReceiveTimeout { get; set; }
        public long SendTimeout { get; set; }
    }
}
