using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sirona.Utilities.Encryption
{
    public class BinaryKeyVectorPair
    {
        private byte[] key;

        public byte[] Key
        {
            get { return key; }
            set { key = value; }
        }
        private byte[] vector;

        public byte[] Vector
        {
            get { return vector; }
            set { vector = value; }
        }
    }
}
