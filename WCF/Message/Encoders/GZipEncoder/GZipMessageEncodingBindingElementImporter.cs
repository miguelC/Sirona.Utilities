using System;
using System.Collections.Generic;
using System.ServiceModel.Description;
using System.Xml;

namespace Sirona.Utilities.WCF.Message.Encoders.GZipEncoder
{
    public sealed class GZipMessageEncodingBindingElementImporter : IPolicyImportExtension
    {
        void IPolicyImportExtension.ImportPolicy(MetadataImporter importer, PolicyConversionContext context)
        {
            if (importer == null)
            {
                throw new ArgumentNullException("importer");
            }
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            ICollection<XmlElement> assertations = context.GetBindingAssertions();
            foreach (XmlElement assertation in assertations)
            {
                if (assertation.NamespaceURI == "http://Sample.Service.netgzip1" && assertation.LocalName == "GZipEncoding")
                {
                    assertations.Remove(assertation);
                    context.BindingElements.Add(new GZipMessageEncodingBindingElement());
                }
            }
        }
    }
}
