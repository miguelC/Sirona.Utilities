using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.ServiceModel.Channels;

namespace Sirona.Utilities.WCF.Message.Encoders.GZipEncoder
{
    public sealed class MessageVersionConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return typeof(string) == sourceType || base.CanConvertFrom(context, sourceType);
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return typeof(InstanceDescriptor) == destinationType || base.CanConvertTo(context, destinationType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string check = value as string;
            if (check == null)
            {
                return base.ConvertFrom(context, culture, value);
            }
            switch (check)
            {
                case "None":
                    return MessageVersion.None;
                case "Default":
                    return MessageVersion.Default;
                case "Soap11":
                    return MessageVersion.Soap11;
                case "Soap11WSAddressing10":
                    return MessageVersion.Soap11WSAddressing10;
                case "Soap11WSAddressingAugust2004":
                    return MessageVersion.Soap11WSAddressingAugust2004;
                case "Soap12":
                    return MessageVersion.Soap12;
                case "Soap12WSAddressing10":
                    return MessageVersion.Soap12WSAddressing10;
                case "Soap12WSAddressingAugust2004":
                    return MessageVersion.Soap12WSAddressingAugust2004;
                default:
                    throw new ArgumentOutOfRangeException("value");
            }
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            MessageVersion messageVersion = value as MessageVersion;

            if (typeof(string) != destinationType || messageVersion == null)
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            if (messageVersion == MessageVersion.Default)
            {
                return "Default";
            }
            if (messageVersion == MessageVersion.None)
            {
                return "None";
            }
            if (messageVersion == MessageVersion.Soap11)
            {
                return "Soap11";
            }
            if (messageVersion == MessageVersion.Soap11WSAddressing10)
            {
                return "Soap11WSAddressing10";
            }
            if (messageVersion == MessageVersion.Soap11WSAddressingAugust2004)
            {
                return "Soap11WSAddressingAugust2004";
            }
            if (messageVersion == MessageVersion.Soap12)
            {
                return "Soap11WSAddressingAugust2004";
            }
            if (messageVersion == MessageVersion.Soap12WSAddressing10)
            {
                return "Soap12WSAddressing10";
            }
            if (messageVersion == MessageVersion.Soap12WSAddressingAugust2004)
            {
                return "Soap12WSAddressingAugust2004";
            }
            throw new ArgumentOutOfRangeException("value");
        }
    }
}
