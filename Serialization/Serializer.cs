using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Sirona.Utilities.Serialization
{
    public class Serializer
    {

        public static object Deserialize(string XML, System.Type oType)
        {
            System.Xml.Serialization.XmlSerializer sSerializer = null;
            System.IO.StringReader stringReader = null;
            try
            {
                sSerializer = new System.Xml.Serialization.XmlSerializer(oType);
                stringReader = new System.IO.StringReader(XML);
                return sSerializer.Deserialize(System.Xml.XmlReader.Create(stringReader));
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in Sirona.Utilities.Serialization.Serializer.Deserialize:" + Environment.NewLine + ex.Message, ex);
            }
            finally
            {
                if ((((stringReader) != null)))
                {
                    stringReader.Dispose();
                }
            }
        }

        public static string SerializeObjectToXMLString(object obj)
        {
            try
            {
                XmlSerializer xSerializer = new XmlSerializer(obj.GetType());
                System.IO.MemoryStream oMemStream = new System.IO.MemoryStream();
                XmlDocument xDoc = new XmlDocument();
                xSerializer.Serialize(oMemStream, obj);
                string sXML = System.Text.UTF8Encoding.UTF8.GetString(oMemStream.ToArray());
                xDoc.PreserveWhitespace = false;
                xDoc.LoadXml(sXML);

                return xDoc.OuterXml;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in Sirona.Utilities.Serialization.Serializer.SerializeObjectToXMLString:" + Environment.NewLine + ex.Message, ex);
            }
        }

        public static string SerializeObjectToXMLString(object obj, System.Xml.Serialization.XmlSerializerNamespaces ns)
        {
            try
            {
                XmlSerializer xSerializer = new XmlSerializer(obj.GetType());
                System.IO.MemoryStream oMemStream = new System.IO.MemoryStream();
                XmlDocument xDoc = new XmlDocument();
                xSerializer.Serialize(oMemStream, obj, ns);
                string sXML = System.Text.UTF8Encoding.UTF8.GetString(oMemStream.ToArray());
                xDoc.PreserveWhitespace = false;
                xDoc.LoadXml(sXML);

                return xDoc.OuterXml;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in Sirona.Utilities.Serialization.Serializer.SerializeObjectToXMLString:" + Environment.NewLine + ex.Message, ex);
            }
        }

        public static string SerializeObjectToXMLString(object obj, string defaultNamespace)
        {
            try
            {
                XmlSerializer xSerializer = null;
                if (defaultNamespace.Length > 0)
                    xSerializer = new XmlSerializer(obj.GetType(), defaultNamespace);
                else
                    xSerializer = new XmlSerializer(obj.GetType());

                System.IO.MemoryStream oMemStream = new System.IO.MemoryStream();
                XmlDocument xDoc = new XmlDocument();
                xSerializer.Serialize(oMemStream, obj);
                string sXML = System.Text.UTF8Encoding.UTF8.GetString(oMemStream.ToArray());
                xDoc.PreserveWhitespace = false;
                xDoc.LoadXml(sXML);

                return xDoc.OuterXml;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in Sirona.Utilities.Serialization.Serializer.SerializeObjectToXMLString:" + Environment.NewLine + ex.Message, ex);
            }
        }


        public static XmlDocument SerializeObjectToXMLDocument(Object obj)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                String xml = SerializeObjectToXMLString(obj);
                doc.LoadXml(xml);

                return doc;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in Sirona.Utilities.Serialization.Serializer.SerializeObjectToXMLDocument:" + Environment.NewLine + ex.Message, ex);
            }
        }

        public static XmlDocument SerializeObjectToXMLDocument(Object obj, System.Xml.Serialization.XmlSerializerNamespaces ns)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                String xml = SerializeObjectToXMLString(obj, ns);
                doc.LoadXml(xml);
                
                return doc;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in Sirona.Utilities.Serialization.Serializer.SerializeObjectToXMLDocument:" + Environment.NewLine + ex.Message, ex);
            }
        }


        public static string SerializeObjectToBase64(object obj)
        {
            try
            {
                string str = SerializeObjectToXMLString(obj);
                byte[] arrByte = System.Text.UTF8Encoding.UTF8.GetBytes(str);
                return Convert.ToBase64String(arrByte);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in Sirona.Utilities.Serialization.Serializer.SerializeObjectToBase64:" + Environment.NewLine + ex.Message, ex);
            }
        }


        public static string DeSerializeBase64ToString(string sBase64)
		{
			try {
				Byte[] arrByte = Convert.FromBase64String(sBase64);
				return System.Text.UTF8Encoding.UTF8.GetString(arrByte);
			} catch (Exception ex) {
				throw new Exception("Exception in Sirona.Utilities.Serialization.Serializer.DeSerializeBase64ToString:" + Environment.NewLine + ex.Message, ex);
			}
		}
    }
}
