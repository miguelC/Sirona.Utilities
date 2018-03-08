using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Sirona.Utilities
{
    public class EnumUtil
    {
        public static string GetStringValue(Enum ValueType)
        {
            Type oType = ValueType.GetType();
            System.Reflection.FieldInfo oFieldInfo= oType.GetField(ValueType.ToString());

            StringValueAttribute[] attributes = (StringValueAttribute[])oFieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false);

            if (attributes.Length > 0)
            {
                return attributes[0].StringValue;
            }
            else
            {
                return Convert.DBNull.ToString();
            }
        }
    }

    public  class StringValueAttribute : Attribute
    {
        private string stringValueField;

        public string StringValue
        {
            get { return stringValueField; }
            set { stringValueField = value; }
        }


        public StringValueAttribute(string Value)
        {
            stringValueField = Value;
        }
    }
}