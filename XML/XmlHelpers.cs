using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;

namespace Sirona.Utilities.XML
{
    public static class XmlHelpers
    {
        public static String CheckEmptyAttribute(XmlNode node, String attributeName, String nullValue)
        {
            if (node.Attributes.GetNamedItem(attributeName) != null)
            {
                if (node.Attributes.GetNamedItem(attributeName).Value != String.Empty)
                {
                    return node.Attributes.GetNamedItem(attributeName).Value;
                }
                else
                {
                    return nullValue;
                }
            }
            else
            {
                return nullValue;
            }
        }

        public static String CheckEmptyNode(XmlNode node, String nodeName, String nullValue)
        {
            if (node.SelectSingleNode(nodeName) != null)
            {
                if (node.SelectSingleNode(nodeName).InnerText != String.Empty)
                {
                    return node.SelectSingleNode(nodeName).InnerText;
                }
                else
                {
                    return nullValue;
                }
            }
            else
            {
                return nullValue;
            }
        }

        public static String GetXmlItem(String xmlDom, String xPath, String nullValue)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlDom);
            return GetXmlItem(xmlDoc, xPath, nullValue);
        }

        public static String GetXmlItem(XmlDocument xmlDoc, String xPath, String nullValue)
        {
            if (xmlDoc.SelectSingleNode(xPath) != null)
            {
                if (xmlDoc.SelectSingleNode(xPath).Value == String.Empty)
                    return nullValue;
                else
                    return xmlDoc.SelectSingleNode(xPath).Value;
            }
            else
            {
                return nullValue;
            }
        }

        public static String GetXmlItem(String xmlDom, String xPath, String nullValue, XmlNamespaceManager namespaces)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlDom);
            return GetXmlItem(xmlDoc, xPath, nullValue, namespaces);
        }

        public static String GetXmlItem(XmlDocument xmlDoc, String xPath, String nullValue, XmlNamespaceManager namespaces)
        {

            if (xmlDoc.SelectSingleNode(xPath, namespaces) != null)
            {
                if (xmlDoc.SelectSingleNode(xPath, namespaces).Value == String.Empty)
                    return nullValue;
                else
                    return xmlDoc.SelectSingleNode(xPath, namespaces).Value;
            }
            else
            {
                return nullValue;
            }
        }

        public static String GetXmlItem(XmlNode xmlNode, String xPath, String nullValue, XmlNamespaceManager namespaces)
        {

            if (xmlNode.SelectSingleNode(xPath, namespaces) != null)
            {
                if (xmlNode.SelectSingleNode(xPath, namespaces).Value == String.Empty)
                    return nullValue;
                else
                    return xmlNode.SelectSingleNode(xPath, namespaces).Value;
            }
            else
            {
                return nullValue;
            }
        }

        public static XmlDocument LoadXmlDocumentFromString(String xmlString)
        {
            XmlDocument xmlDoc = new XmlDocument();
            if (xmlString.Length > 0 && xmlString != String.Empty)
            {
                xmlDoc.LoadXml(xmlString);
            }
            else
            {
                throw new Exception("Missing data to load in LoadXmlDocumentFromString()");
            }
            return xmlDoc;
        }
        
    }
}
