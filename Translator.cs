using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Sirona.Utilities
{
    public class Translator
    {
        public static string TranslateCDAToMobileHTML(string sCDAData)
        {
            try
            {
                System.Xml.Xsl.XslCompiledTransform xslttransform = new System.Xml.Xsl.XslCompiledTransform();
                System.IO.StreamReader oxsltstream = new System.IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MobileCDA.xslt"));
                System.Xml.XmlTextReader xtextreader = new System.Xml.XmlTextReader(oxsltstream);
                xslttransform.Load(xtextreader);

                System.Xml.XPath.XPathDocument xDoc = new System.Xml.XPath.XPathDocument(new System.IO.StringReader(sCDAData));

                System.Xml.XmlWriterSettings xWriterSettings = new System.Xml.XmlWriterSettings();
                xWriterSettings.ConformanceLevel = System.Xml.ConformanceLevel.Auto;

                StringBuilder sb = new StringBuilder();

                xslttransform.Transform(xDoc, System.Xml.XmlWriter.Create(sb, xWriterSettings));


                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in Sirona.Utilities.Translator.TranslateCDAToMobileHTML:\n" + ex.Message);
            }
        }


        //'/primepractice/clinical/stylesheets/labresult.xsl      'Regular Result
        //'/primepractice/clinical/stylesheets/labdocument.xsl    'Microbiology
        //'/primepractice/clinical/stylesheets/document.xsl       'Transcription
        public static string TranslateLabResultXMLToMobileHTML(string sXML, string sStyleSheetType)
        {
            try
            {
                System.Xml.Xsl.XslCompiledTransform xsltTransform = new System.Xml.Xsl.XslCompiledTransform();
                System.IO.StreamReader xsltStream;

                switch (sStyleSheetType.ToLower())
                {
                    case "document":
                        xsltStream = new System.IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MobileDocument.xslt"));
                        break;
                    case "labdocument":
                        xsltStream = new System.IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MobileLabDocument.xslt"));
                        break;
                    case "labresult":
                        xsltStream = new System.IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MobileLabResult.xslt"));
                        break;
                    default:
                        xsltStream = null;
                        break;
                }

                System.Xml.XmlTextReader xtextreader = new System.Xml.XmlTextReader(xsltStream);
                xsltTransform.Load(xtextreader);

                System.Xml.XmlDocument xLabResultDom = new System.Xml.XmlDocument();
                xLabResultDom.LoadXml(sXML);

                System.Xml.XmlWriterSettings xWriterSettings = new System.Xml.XmlWriterSettings();
                xWriterSettings.ConformanceLevel = System.Xml.ConformanceLevel.Auto;

                StringBuilder sb = new StringBuilder();

                //Dim xDoc As New System.Xml.XPath.XPathDocument(New System.IO.StringReader(sXML))
                //'xsltTransform.Transform(xDoc, System.Xml.XmlWriter.Create(sb, xWriterSettings))
                xsltTransform.Transform(new System.Xml.XmlNodeReader(xLabResultDom), System.Xml.XmlWriter.Create(sb, xWriterSettings));

                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in Sirona.Utilities.Translator.TranslateLabResultXMLToMobileHTML:  \n" + ex.Message);
            }
        }
    }
}
