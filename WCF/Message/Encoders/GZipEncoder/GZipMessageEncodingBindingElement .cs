using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;

namespace Sirona.Utilities.WCF.Message.Encoders.GZipEncoder
{
    public sealed class GZipMessageEncodingBindingElement : MessageEncodingBindingElement, IPolicyExportExtension
    {
        private MessageEncodingBindingElement _innerBindingElement;

        public MessageEncodingBindingElement InnerMessageEncodingBindingElement
        {
            get { return _innerBindingElement; }
            set { _innerBindingElement = value; }
        }

        public override MessageVersion MessageVersion
        {
            get { return _innerBindingElement.MessageVersion; }
            set { _innerBindingElement.MessageVersion = value; }
        }
        public XmlDictionaryReaderQuotas ReaderQuotas
        {
            get
            {
                if (_innerBindingElement.GetType() == typeof(TextMessageEncodingBindingElement))
                {
                    TextMessageEncodingBindingElement binding = (TextMessageEncodingBindingElement)_innerBindingElement;
                    return binding.ReaderQuotas;
                }
                else
                {
                    BinaryMessageEncodingBindingElement binding = (BinaryMessageEncodingBindingElement)_innerBindingElement;
                    return binding.ReaderQuotas;
                }
            }
        }

        public GZipMessageEncodingBindingElement()
            : this(new TextMessageEncodingBindingElement())
        {
        }
        public GZipMessageEncodingBindingElement(MessageEncodingBindingElement messageEncodingBindingElement)
        {
            _innerBindingElement = messageEncodingBindingElement;
        }

        public override MessageEncoderFactory CreateMessageEncoderFactory()
        {
            return new GZipMessageEncoderFactory(_innerBindingElement.CreateMessageEncoderFactory());
        }
        public override BindingElement Clone()
        {
            return new GZipMessageEncodingBindingElement(_innerBindingElement);
        }
        public override T GetProperty<T>(BindingContext context)
        {
            return typeof(T) == typeof(XmlDictionaryReaderQuotas) ? _innerBindingElement.GetProperty<T>(context) : base.GetProperty<T>(context);
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }
        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            context.BindingParameters.Add(this);
            return context.BuildInnerChannelListener<TChannel>();
        }
        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            context.BindingParameters.Add(this);
            return context.CanBuildInnerChannelFactory<TChannel>();
        }
        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            context.BindingParameters.Add(this);
            return context.CanBuildInnerChannelListener<TChannel>();
        }


        void IPolicyExportExtension.ExportPolicy(MetadataExporter exporter, PolicyConversionContext policyContext)
        {
            if (policyContext == null)
            {
                throw new ArgumentNullException("policyContext");
            }
            XmlDocument document = new XmlDocument();
            policyContext.GetBindingAssertions().Add(document.CreateElement("gzip", "GZipEncoding", "http://Sample.Service.netgzip1"));
        }
    }
}
