using System;
using System.ComponentModel;
using System.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.Xml;

namespace Sirona.Utilities.WCF.Message.Encoders.GZipEncoder
{
    public class GZipMessageEncodingElement : BindingElementExtensionElement
    {
        public override Type BindingElementType
        {
            get { return typeof(GZipMessageEncodingBindingElement); }
        }

        [ConfigurationProperty("innerMessageEncoding", DefaultValue = "textMessageEncoding")]
        public string InnerMessageEncoding
        {
            get { return (string)base["innerMessageEncoding"]; }
            set { base["innerMessageEncoding"] = value; }
        }
        [ConfigurationProperty("messageVersion", DefaultValue = "Soap12")]
        [TypeConverter(typeof(MessageVersionConverter))]
        public MessageVersion MessageVersion
        {
            get { return (MessageVersion)base["messageVersion"]; }
            set { base["messageVersion"] = value; }
        }
        [ConfigurationProperty("readerQuotas")]
        public XmlDictionaryReaderQuotasElement ReaderQuotasElement
        {
            get { return (XmlDictionaryReaderQuotasElement)base["readerQuotas"]; }
        }

        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            GZipMessageEncodingBindingElement binding = (GZipMessageEncodingBindingElement)bindingElement;
            PropertyInformationCollection propertyInfo = ElementInformation.Properties;
            // ReSharper disable PossibleNullReferenceException
            if (propertyInfo["innerMessageEncoding"].ValueOrigin != PropertyValueOrigin.Default)
            // ReSharper restore PossibleNullReferenceException
            {
                switch (InnerMessageEncoding)
                {
                    case "textMessageEncoding":
                        binding.InnerMessageEncodingBindingElement = new TextMessageEncodingBindingElement();
                        break;
                    case "binaryMessageEncoding":
                        binding.InnerMessageEncodingBindingElement = new BinaryMessageEncodingBindingElement();
                        break;
                }
            }
            ApplyConfiguration(binding.ReaderQuotas);
        }

        private void ApplyConfiguration(XmlDictionaryReaderQuotas readerQuotas)
        {
            if (readerQuotas == null)
            {
                throw new ArgumentNullException("readerQuotas");
            }

            if (ReaderQuotasElement.MaxDepth != 0)
            {
                readerQuotas.MaxDepth = ReaderQuotasElement.MaxDepth;
            }
            if (ReaderQuotasElement.MaxStringContentLength != 0)
            {
                readerQuotas.MaxStringContentLength = ReaderQuotasElement.MaxStringContentLength;
            }
            if (ReaderQuotasElement.MaxArrayLength != 0)
            {
                readerQuotas.MaxArrayLength = ReaderQuotasElement.MaxArrayLength;
            }
            if (ReaderQuotasElement.MaxBytesPerRead != 0)
            {
                readerQuotas.MaxBytesPerRead = ReaderQuotasElement.MaxBytesPerRead;
            }
            if (ReaderQuotasElement.MaxNameTableCharCount != 0)
            {
                readerQuotas.MaxNameTableCharCount = ReaderQuotasElement.MaxNameTableCharCount;
            }
        }

        protected override BindingElement CreateBindingElement()
        {
            GZipMessageEncodingBindingElement bindingElement = new GZipMessageEncodingBindingElement();
            ApplyConfiguration(bindingElement);
            return bindingElement;
        }
    }
}
