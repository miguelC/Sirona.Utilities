using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Sirona.Utilities.Serialization
{
    public static class ObjectUtility
    {
        public static T DeepCopy<T>(T obj)
        {
            object result = null;

            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                result = (T)formatter.Deserialize(ms);
                ms.Close();
            }

            return (T)result;
        }

        public static object Copy(object source)
        {

            if (source == null)
                return null;

            else
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new MemoryStream();

                using (stream)
                {
                    formatter.Serialize(stream, source);
                    stream.Seek(0, SeekOrigin.Begin);

                    return formatter.Deserialize(stream);
                }
            }
        }


    }
}
