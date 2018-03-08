using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Sirona.Utilities.Install
{
    public class InstallConfigurationProperties
    {
        public bool RemoveSetupFilesAfterInstall { get; set; }
        [XmlArrayItem("add")]
        public List<InstallConfigurationProperty> Properties { get; set; }

        public string GetPropertyValue(string propertyName)
        {
            if (Properties != null)
            {
                foreach (InstallConfigurationProperty prop in this.Properties)
                {
                    if (prop.Name.Equals(propertyName))
                    {
                        return prop.Value;
                    }
                }
            }
            return null;
        }
        public T GetPropertyValue<T>(string propertyName)
        {
            string val = this.GetPropertyValue(propertyName);
            if (!string.IsNullOrEmpty(val))
            {

                if (typeof(T) == typeof(TimeSpan))
                    return (T)(object)TimeSpan.Parse(val);

                return (T)Convert.ChangeType(val, typeof(T));
            }
            return default(T);
        }
    }
   public class InstallConfigurationProperty
   {
       [XmlAttribute("name")]
       public string Name { get; set; }
       [XmlText()]
       public string Value { get; set; }
   }
}
