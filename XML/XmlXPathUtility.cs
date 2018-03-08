using System;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;
using System.Xml.Xsl;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;
using System.Security.Policy;
using System.Reflection;
using Sirona.Utilities.Strings;

namespace Sirona.Utilities.XML
{
    /// <summary>
    /// Utility to read, transform and serialize xml into Xpathdocument objects
    /// </summary>
    public static class XmlXPathUtility
    {
        public const string EMBEDDED_RESOURCE = "embedded:";

        // Display the validation error.
        static void ValidationCallback(object sender, ValidationEventArgs args)
        {
            //Event logs???
        }

        #region XSLT transform methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcDoc"></param>
        /// <param name="styleSheet"></param>
        /// <returns></returns>
        public static XPathDocument Transform(IXPathNavigable srcDoc,
                                              IXPathNavigable styleSheet)
        {
            return Transform(srcDoc, styleSheet, null, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcDoc"></param>
        /// <param name="styleSheet"></param>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public static XPathDocument Transform(IXPathNavigable srcDoc,
                                              IXPathNavigable styleSheet,
                                              XmlResolver resolver)
        {
            return Transform(srcDoc, styleSheet, null, resolver);
        }
        /// <summary>
        /// A more efficient way of using transforms
        /// </summary>
        /// <param name="srcDoc"></param>
        /// <param name="styleSheet"></param>
        /// <param name="arguments"></param>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public static XPathDocument Transform(IXPathNavigable srcDoc,
                                              IXPathNavigable styleSheet,
                                              XsltArgumentList arguments,
                                              XmlResolver resolver)
        {
            string transformed = TransformToString(srcDoc, styleSheet, arguments, resolver);
            XmlTextReader reader = null;
            try
            {
                StringReader sr = new StringReader(transformed);
                reader = new XmlTextReader(sr);
                return new XPathDocument(reader);
            }
            finally
            {
                if (reader != null) reader.Close();
            }

        }
        /// <summary>
        /// Efficient way of transforming xml using stylesheets
        /// </summary>
        /// <param name="srcDoc"></param>
        /// <param name="styleSheet"></param>
        /// <param name="arguments"></param>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public static string TransformToString(IXPathNavigable srcDoc,
                                              IXPathNavigable styleSheet,
                                              XsltArgumentList arguments,
                                              XmlResolver resolver)
        {
            StringWriter sw = null;
            XmlTextWriter writer = null;
            try
            {
                StringBuilder sb = new StringBuilder();
                sw = new StringWriter(sb);
                writer = new XmlTextWriter(sw);
                XslCompiledTransform xsltransform = new XslCompiledTransform();
                XsltSettings settings = new XsltSettings();
                xsltransform.Load(styleSheet, settings, resolver);
                xsltransform.Transform(srcDoc, arguments, writer);
                return sb.ToString();
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }
        #endregion
        #region Parsing methods
        /// <summary>
        /// Parsing
        /// </summary>
        /// <param name="txtReader"></param>
        /// <returns></returns>
        public static XPathDocument Parse(XmlTextReader txtReader)
        {
            return Parse(txtReader, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="txtReader"></param>
        /// <param name="validate"></param>
        /// <returns></returns>
        public static XPathDocument Parse(XmlTextReader txtReader, bool validate)
        {
            if (validate)
            {
                XmlReader reader = null;
                try
                {
                    XmlReaderSettings settings = new XmlReaderSettings();
                    ValidationEventHandler eventHandler = new ValidationEventHandler(ValidationCallback);
                    settings.ValidationEventHandler += eventHandler;
                    settings.ValidationType = ValidationType.Schema;
                    reader = XmlReader.Create(txtReader, settings);
                    return new XPathDocument(reader);
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }
            else
            {
                return new XPathDocument(txtReader);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        public static XPathDocument Parse(Stream inputStream)
        {
            return Parse(inputStream, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="validate"></param>
        /// <returns></returns>
        public static XPathDocument Parse(Stream inputStream, bool validate)
        {
            if (inputStream == null)
                return null;
            XmlTextReader txtReader = null;
            try
            {

                txtReader = new XmlTextReader(inputStream);

                // Create the validating reader and specify Auto validation.
                return Parse(txtReader, validate);
            }
            finally
            {
                if (txtReader != null)
                    txtReader.Close();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static XPathDocument Parse(string fileName)
        {
            return Parse(fileName, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="validate"></param>
        /// <returns></returns>
        public static XPathDocument Parse(string fileName, bool validate)
        {
            Stream fis = null;
            if (fileName.StartsWith(EMBEDDED_RESOURCE))
            {
                fis = ReadEmbeddedResource(fileName);
            }
            else
            {
                fis = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            }

            return Parse(fis, validate);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcXml"></param>
        /// <returns></returns>
        public static XPathDocument ParseFromString(string srcXml)
        {
            StringReader tr = new StringReader(srcXml);
            XmlTextReader sr = new XmlTextReader(tr);
            try
            {
                return Parse(sr);
            }
            finally
            {
                tr.Close();
                sr.Close();
            }
        }
        #endregion

        #region Xml serialization
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pObject"></param>
        /// <returns></returns>
        public static XPathDocument SerializeObject(Object pObject)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            settings.IndentChars = "  ";
            return SerializeObject(pObject, settings);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pObject"></param>
        /// <param name="pSettings"></param>
        /// <returns></returns>
        public static XPathDocument SerializeObject(Object pObject, XmlWriterSettings pSettings)
        {
            return SerializeObject(pObject, pSettings, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pObject"></param>
        /// <param name="pSettings"></param>
        /// <param name="pNamespaces"></param>
        /// <returns></returns>
        public static XPathDocument SerializeObject(Object pObject, XmlWriterSettings pSettings, XmlSerializerNamespaces pNamespaces)
        {
            if (pObject != null)
            {
                XmlWriter writer = null;
                StringBuilder sb = new StringBuilder();
                try
                {
                    writer = XmlTextWriter.Create(sb, pSettings);
                    XmlSerializer sin = new XmlSerializer(pObject.GetType());
                    if (pNamespaces == null)
                    {
                        sin.Serialize(writer, pObject);
                    }
                    else
                    {
                        sin.Serialize(writer, pObject, pNamespaces);
                    }
                }
                finally
                {
                    if (writer != null)
                    {
                        writer.Close();
                    }
                }
                return ParseFromString(sb.ToString());
            }
            return null;
        }
        /// <summary>
        /// Serialize object to XML string
        /// </summary>
        /// <param name="pObject">object to serialize</param>
        /// <returns>returns XML string representation of object</returns>
        public static string SerializeObjectToXmlString(Object pObject)
        {
            return SerializeObject(pObject).CreateNavigator().OuterXml;
        }

        /// <summary>
        /// Serializes an object into an xml document at the given path or file name using UTF-8 Encoding
        /// </summary>
        /// <param name="pObject">the object that we want to serialize</param>
        /// <param name="pPath">The path to the file we want to write</param>
        public static void SerializeObjectToFile(Object pObject, string pPath)
        {
            SerializeObjectToFile(pObject, pPath, Encoding.UTF8);
        }
        /// <summary>
        /// Serializes an object into an xml document at the given path or file name using UTF-8 Encoding
        /// </summary>
        /// <param name="pObject">the object that we want to serialize</param>
        /// <param name="pPath">The path to the file we want to write</param>
        /// <param name="namespaces">The namespaces to use for serialization</param>
        public static void SerializeObjectToFile(Object pObject, string pPath, XmlSerializerNamespaces namespaces)
        {
            SerializeObjectToFile(pObject, pPath, Encoding.UTF8, namespaces);
        }
        /// <summary>
        /// Serializes an object into an xml document at the given path or file name
        /// </summary>
        /// <param name="pObject">the object that we want to serialize</param>
        /// <param name="pPath">The path to the file we want to write</param>
        /// <param name="pEncoding">The encoding type to use i.e. utf-8</param>
        public static void SerializeObjectToFile(Object pObject, string pPath, Encoding pEncoding)
        {
            SerializeObjectToFile(pObject, pPath, pEncoding, null);
        }
        /// <summary>
        /// Serializes an object into an xml document at the given path or file name
        /// </summary>
        /// <param name="pObject">the object that we want to serialize</param>
        /// <param name="pPath">The path to the file we want to write</param>
        /// <param name="pSettings">The settings of the writer for formatting</param>
        public static void SerializeObjectToFile(Object pObject, string pPath, XmlWriterSettings pSettings)
        {
            SerializeObjectToFile(pObject, pPath, pSettings, null);
        }
        /// <summary>
        /// Serializes an object into an xml document at the given path or file name
        /// </summary>
        /// <param name="pObject">the object that we want to serialize</param>
        /// <param name="pPath">The path to the file we want to write</param>
        /// <param name="pSettings">The settings of the writer for formatting</param>
        /// <param name="pNamespaces">Namespaces to use when serializing</param>
        public static void SerializeObjectToFile(Object pObject, string pPath, XmlWriterSettings pSettings, XmlSerializerNamespaces pNamespaces)
        {
            if (pObject != null)
            {
                XmlWriter writer = null;
                try
                {
                    writer = XmlTextWriter.Create(pPath, pSettings);
                    XmlSerializer sin = new XmlSerializer(pObject.GetType());
                    if (pNamespaces == null)
                    {
                        sin.Serialize(writer, pObject);
                    }
                    else
                    {
                        sin.Serialize(writer, pObject, pNamespaces);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    if (writer != null)
                    {
                        writer.Close();
                    }
                }
            }
        }
        /// <summary>
        /// Serializes an object into an xml document at the given path or file name
        /// </summary>
        /// <param name="pObject">the object that we want to serialize</param>
        /// <param name="pPath">The path to the file we want to write</param>
        /// <param name="pEncoding">The encoding type to use i.e. utf-8</param>
        /// <param name="namespaces">Namesapces to use when serializing</param>
        public static void SerializeObjectToFile(Object pObject, string pPath, Encoding pEncoding, XmlSerializerNamespaces namespaces)
        {

            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.Indent = true;
            writerSettings.IndentChars = "  ";
            writerSettings.Encoding = pEncoding;
            SerializeObjectToFile(pObject, pPath, writerSettings, namespaces);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T Deserialize<T>(IXPathNavigable xml)
        {
            return Deserialize<T>(xml, new UTF8Encoding());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static T Deserialize<T>(IXPathNavigable xml, Encoding encoding)
        {
            XmlTextWriter xmlTextWriter = null;
            try
            {
                string xmlString = xml.CreateNavigator().InnerXml;
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                byte[] byteArray = encoding.GetBytes(xmlString);
                using (MemoryStream memoryStream = new MemoryStream(byteArray))
                {
                    xmlTextWriter = new XmlTextWriter(memoryStream, encoding);
                    return (T)serializer.Deserialize(memoryStream);
                }
            }
            finally
            {
                if (xmlTextWriter != null) xmlTextWriter.Close();
            }
        }
        /// <summary>
        /// Deserializes an object of the generic given type T from an XML string
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static T DeserializeString<T>(string xmlString)
        {
            return DeserializeString<T>(xmlString, new UTF8Encoding());
        }
        /// <summary>
        /// Deserializes an object of the generic given type T from an XML string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlString"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static T DeserializeString<T>(string xmlString, Encoding encoding)
        {
            if (string.IsNullOrEmpty(xmlString))
            {
                return default(T);
            }
            XmlTextWriter xmlTextWriter = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                byte[] byteArray = encoding.GetBytes(xmlString);
                using (MemoryStream memoryStream = new MemoryStream(byteArray))
                {
                    xmlTextWriter = new XmlTextWriter(memoryStream, encoding);
                    return (T)serializer.Deserialize(memoryStream);
                }
            }
            finally
            {
                if (xmlTextWriter != null) xmlTextWriter.Close();
            }
        }
        #endregion

        #region Read files embedded

        /// <summary>
        /// Reads an embedded resource from a dll assembly
        /// </summary>
        /// <param name="location">
        /// The location of the embedded resource that includes the heading "embedded:" 
        /// plus the assembly name and the resource name. 
        /// <br/><br/>The format is :<br/><br/>
        /// embedded:[assembly name fully qualified]:{resource.location} for example
        /// <br/><br/>embedded:MAMCFramework, version=1.3.0.1, Culture=neutral, PublicKeyToken=9b2b13529c3c2584:Config.ondi.config
        /// <br/><br/>This will resolve in the file located at Config\ondi.config from the sources root of the code that built
        /// assembly for MAMCFramework.
        /// </param>
        /// <returns></returns>
        public static Stream ReadEmbeddedResource(string location)
        {
            if (location.StartsWith(EMBEDDED_RESOURCE))
            {
                StringTokenizer tokenizer = new StringTokenizer(location, @":");
                string assemblyName = tokenizer.TokenAt(1);
                if (assemblyName != null)
                {
                    Assembly assem = null;
                    assem = Assembly.Load(assemblyName);
                    if (assem != null)
                    {
                        //Testing assembly contents
                        //        string[] rNames = assem.GetManifestResourceNames();
                        //        Trace.WriteLine("Resources:");
                        //        foreach(string name in rNames)
                        //        {
                        //          Trace.WriteLine(name);
                        //        }
                        // This is for embedded files
                        string resourceName = tokenizer.TokenAt(2);
                        if (resourceName != null)
                        {
                            return ReadEmbeddedResource(assem, resourceName);
                        }
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Reads an embedded resource from an assembly file. 
        /// It is flexible enough so that if the resource is not fully defined, it will look for a name match in the available resources
        /// ignoring case.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="name">
        /// loose location which means for example if name is app.config, a resource called APP.config can be found or
        /// Config.App.Config will also be found. A word of caution using this name, the first match is returned in case that 
        /// the name does not exactly matches a resource in the library so in some cases the result may be unknown.
        /// You must qualify the name as much as possible, meaning looking for a resource that is actually named Config.App.config
        /// config.app.config will work better than app.config or config.app.
        /// </param>
        /// <returns></returns>
        public static Stream ReadEmbeddedResource(Assembly assembly, string name)
        {
            if (assembly != null)
            {
                // This is for embedded files
                if (name != null)
                {
                    //string resourceName = assembly.GetName().Name + name;
                    Stream st = assembly.GetManifestResourceStream(name);
                    if (st == null)
                    {
                        //Testing assembly contents
                        string[] rNames = assembly.GetManifestResourceNames();
                        foreach (string rName in rNames)
                        {
                            if (ContainsIgnoreCase(rName, name))
                            {
                                //resourceName = assembly.GetName().Name + "." + rName;
                                st = assembly.GetManifestResourceStream(rName);
                                if (st != null)
                                {
                                    return st;
                                }
                            }
                        }
                    }
                    return st;
                }
            }
            return null;
        }
        public static bool ContainsIgnoreCase(string orig, string s1)
        {
            if (orig == null || s1 == null)
            {
                return false;
            }
            return orig.ToLower().IndexOf(s1.ToLower()) >= 0;
        }
        #endregion
    }
}

