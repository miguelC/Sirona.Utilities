using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Xml.XPath;
using System.Xml;
using System.IO;
using ServiceModelChannels = System.ServiceModel.Channels;
using Sirona.Utilities.XML;

namespace Sirona.Utilities.WCF.Message
{
    public static class WcfMessageHelpers
    {
        public const string Soap12Namespace = "http://www.w3.org/2003/05/soap-envelope"; 
        public const string WebServicesAddressingNamespace = "http://www.w3.org/2005/08/addressing";

        public static string ReadWcfMessageBufferAsString(ref ServiceModelChannels.Message message)
        {
            XDocument wcfMessageDoc = WcfMessageHelpers.ReadWcfMessageBuffer(ref message);
            var reader = wcfMessageDoc.CreateReader();
            reader.MoveToContent();
            return reader.ReadOuterXml();
        }

        public static XDocument ReadWcfMessageBuffer(ref ServiceModelChannels.Message message)
        {
            using (ServiceModelChannels.MessageBuffer msgbuf = message.CreateBufferedCopy(int.MaxValue))
            {
                ServiceModelChannels.Message messageCopy = msgbuf.CreateMessage();

                using (MemoryStream stream = new MemoryStream())
                {
                    using (XmlWriter writer = XmlWriter.Create(stream))
                    {
                        messageCopy.WriteMessage(writer);
                        writer.Flush();
                        stream.Position = 0;
                        XDocument xdoc = XDocument.Load(XmlReader.Create(stream));
                        message = msgbuf.CreateMessage();
                        return xdoc;
                    }
                }

            }
        }
        public static ServiceModelChannels.Message CopyMessageFromBuffer(ref ServiceModelChannels.Message message)
        {
            using (ServiceModelChannels.MessageBuffer msgbuf = message.CreateBufferedCopy(int.MaxValue))
            {
                ServiceModelChannels.Message returnMessage = msgbuf.CreateMessage();
                message = msgbuf.CreateMessage();
                return returnMessage;
            }
        }

        //public static ServiceModelChannels.Message BuildMessageFromXDocument(XDocument soapEnvelope)
        //{
        //    //TODO FINISH THIS IF NEEDED
        //    XElement soapHeader = GetSoapEnvelopeHeaderElement(soapEnvelope);
        //    string action = soapHeader.Descendants((XNamespace)"http://www.w3.org/2005/08/addressing" + "Action").FirstOrDefault().Value;
        //    ServiceModelChannels.Message tempMessage = ServiceModelChannels.Message.CreateMessage(
        //            ServiceModelChannels.MessageVersion.Soap12WSAddressing10, 
        //            action);
        //    foreach(XElement header in soapHeader.DescendantNodes())
        //    {
        //      tempMessage.Headers.Add(ServiceModelChannels.MessageHeader.CreateHeader();
        //    }
        //    return tempMessage;
        //}

        public static XmlDocument GetSoapEnvelopeBody(XDocument soapEnvelope)
        {

            XElement soapBody = (from xml in soapEnvelope.Descendants(XName.Get("Body", Soap12Namespace))
                                     select xml).FirstOrDefault();
            return XmlUtility.ToXmlDocument(soapBody);
        }

        public static XmlDocument GetSoapEnvelopeBodyContent(XDocument soapEnvelope)
        {

            XNode soapBodyContent = (from xml in soapEnvelope.Descendants(XName.Get("Body", Soap12Namespace))
                                 select xml).FirstOrDefault().FirstNode;
            return XmlUtility.ToXmlDocument(soapBodyContent);
        }

        public static XmlDocument GetSoapEnvelopeHeaders(XDocument soapEnvelope)
        {

            XElement soapHeader = (from xml in soapEnvelope.Descendants(XName.Get("Header", Soap12Namespace))
                                 select xml).FirstOrDefault();
            return XmlUtility.ToXmlDocument(soapHeader);
        }

        public static XElement GetSoapEnvelopeHeaderElement(XDocument soapEnvelope)
        {
            return GetSoapEnvelopeHeaderElement(soapEnvelope, null);
        }
        public static XElement GetSoapEnvelopeHeaderElement(XDocument soapEnvelope, XName xpath)
        {

            XElement soapHeader = (from xml in soapEnvelope.Descendants(XName.Get("Header", Soap12Namespace))
                                 select xml).FirstOrDefault();
            if (xpath != null)
            {
                return (from xml in soapHeader.Descendants(xpath)
                        select xml).FirstOrDefault();
            }
            return soapHeader;
        }
    }
}
