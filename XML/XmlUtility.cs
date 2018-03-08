using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;
using System.Xml.Xsl;
using System.Xml.Linq;
using System.IO;
using System.Collections;
using System.Xml.Serialization;
using System.Security.Policy;
using Sirona.Utilities.Strings;
using Sirona.Utilities.IO;

namespace Sirona.Utilities.XML
{
    public static class XmlUtility
    {
        /// <summary>
        /// 
        /// </summary>
        public const int PARSE_SUCCESS = 0;
        /// <summary>
        /// 
        /// </summary>
        public const int PARSE_ERROR = 1;
        // Display the validation error.
        static void ValidationCallback(object sender, ValidationEventArgs args)
        {
            Console.WriteLine("Validation error loading xml");
            Console.WriteLine(args.Message);
        }
        /// <summary>
        /// Parses an xml document from a string
        /// </summary>
        /// <param name="srcXml"></param>
        /// <returns>The document object</returns>
        public static XmlDocument ParseFromString(string srcXml)
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
        /// <summary> Parsing to DOM
        /// 
        /// </summary>
        /// <param name="fileName">the name of the file to parse
        /// </param>
        /// <returns> The DOM document 
        /// </returns>
        public static XmlDocument Parse(String fileName)
        {
            return Parse(fileName, false);
        }
        /// <summary> 
        /// Parsing to DOM 
        /// </summary>
        /// <param name="fileName">the name of the file to parse
        /// </param>
        /// <param name="validate">whether to use auto validation
        /// </param>
        /// <returns> The DOM document
        /// </returns>
        public static XmlDocument Parse(String fileName, bool validate)
        {
            Stream fis = null;
            if (fileName.StartsWith(FileUtility.EMBEDDED_RESOURCE))
            {
                fis = FileUtility.ReadEmbeddedResource(fileName);
            }
            else
            {
                fis = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            }

            return Parse(fis, validate);
        }

        /// <summary> Parsing to DOM
        /// 
        /// </summary>
        /// <param name="inputStream">The input stream
        /// </param>
        /// <returns> The DOM document
        /// 
        /// </returns>
        public static XmlDocument Parse(Stream inputStream)
        {
            return Parse(inputStream, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="txtReader"></param>
        /// <returns></returns>
        public static XmlDocument Parse(XmlTextReader txtReader)
        {
            return Parse(txtReader, false);
        }
        /// <summary>
        /// Parses an xml from an xml text reader
        /// </summary>
        /// <param name="txtReader"></param>
        /// <param name="validate"></param>
        /// <returns></returns>
        public static XmlDocument Parse(XmlTextReader txtReader, bool validate)
        {
            XmlDocument doc = new XmlDocument();
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
                    doc.Load(reader);
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }
            else
            {
                doc.Load(txtReader);
            }

            //Console.WriteLine(doc.OuterXml);
            return doc;
        }
        /// <summary> Parsing to DOM
        /// </summary>
        /// <param name="inputStream">The input stream (will be closed after read)
        /// </param>
        /// <param name="validate">whether to use auto validation
        /// </param>
        /// <returns> The DOM document
        /// 
        /// </returns>
        public static XmlDocument Parse(Stream inputStream, bool validate)
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
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (txtReader != null)
                    txtReader.Close();
                inputStream.Close();
            }
        }

        /// <summary> Parsing to DOM
        /// 
        /// </summary>
        /// <param name="content">the content to parse as XML
        /// </param>
        /// <returns> The DOM document
        /// 
        /// </returns>
        public static XmlDocument Parse(byte[] content)
        {
            return Parse(content, false);
        }

        /// <summary> Parsing to DOM
        /// 
        /// </summary>
        /// <param name="content">the content to parse as XML
        /// </param>
        /// <param name="validate"></param>
        /// <returns> The DOM document
        /// 
        /// </returns>
        public static XmlDocument Parse(byte[] content, bool validate)
        {
            MemoryStream ms = new MemoryStream(content);
            return Parse(ms, validate);
        }
        /// <summary> Writing a DOM Tree to a file
        /// </summary>
        /// <param name="fileName">The fully qualified name of the xml file
        /// </param>
        /// <param name="doc">The document in DOM form
        /// 
        /// </param>
        public static void Write(XmlDocument doc, String fileName)
        {
            XmlTextWriter writer = null;
            try
            {
                writer = new XmlTextWriter(fileName, System.Text.Encoding.UTF8);
                Write(doc, writer);
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="outWriter"></param>
        public static void Write(XmlDocument doc, TextWriter outWriter)
        {
            XmlTextWriter writer = null;
            try
            {
                writer = new XmlTextWriter(outWriter);
                Write(doc, writer);
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="outWriter"></param>
        public static void Write(XmlDocument doc, XmlTextWriter outWriter)
        {
            try
            {
                outWriter.Formatting = Formatting.Indented;
                outWriter.Indentation = 2;
                //outWriter.QuoteChar = '\u0027';
                doc.WriteTo(outWriter);
                outWriter.Flush();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (outWriter != null)
                {
                    outWriter.Close();
                }
            }
        }

        /// <summary>
        /// Converts an object to another of specified through XML Serialization.  
        /// This is used primarily to convert web service proxy classes back to 
        /// their originial form.  In order for this to work, both types must belong
        /// to the same XML namespace.  This is done throug the use of the following
        /// attributes:
        /// 
        /// On the web service:
        /// [WebService(Namespace="http://Octl.ssofi/")]
        /// 
        /// On the bean class:
        /// [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://Octl.ssofi/")]
        /// </summary>
        /// <param name="oin">object to convert</param>
        /// <param name="t">type to convert object to</param>
        /// <returns>new object of supplied type</returns>
        public static Object ConvertObject(Object oin, System.Type t)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            XmlSerializer sin = new XmlSerializer(oin.GetType());
            sin.Serialize(writer, oin);
            writer.Close();

            XmlSerializer sout = new XmlSerializer(t);
            StringReader reader = new StringReader(sb.ToString());
            Object oout = sout.Deserialize(reader);
            reader.Close();
            return oout;
        }

        /// <summary>
        /// Serialize object to XML string
        /// </summary>
        /// <param name="pObject">object to serialize</param>
        /// <returns>returns XML string representation of object</returns>
        public static string SerializeObjectToXmlString(Object pObject)
        {

            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.Indent = false;
            writerSettings.Encoding = Encoding.UTF8;

            return SerializeObjectToXmlString(pObject, writerSettings, null);
        }

        public static string SerializeObjectToXmlString(Object pObject, XmlWriterSettings pSettings)
        {
            return SerializeObjectToXmlString(pObject, pSettings, null);
        }

        public static string SerializeObjectToXmlString(Object pObject, XmlWriterSettings pSettings, XmlSerializerNamespaces pNamespaces)
        {
            if (pObject == null)
                return null;

            MemoryStream stream = new MemoryStream();
            using (XmlWriter writer = XmlWriter.Create(stream, pSettings))
            {
                if (writer != null)
                {
                    XmlSerializer serializer = new XmlSerializer(pObject.GetType());
                    if (pNamespaces == null)
                    {
                        serializer.Serialize(writer, pObject);
                    }
                    else
                    {
                        serializer.Serialize(writer, pObject, pNamespaces);
                    }
                    return pSettings.Encoding.GetString(stream.ToArray());
                }

            }
            return null;
        }

        /// <summary>
        /// Serializes an object into an xml document in memory
        /// </summary>
        /// <param name="pObject">the object that we want to serialize</param>
        /// <returns>The xml document from the object</returns>
        public static XmlDocument SerializeObject(Object pObject)
        {
            return SerializeObjectToXmlDocument(pObject, null);
        }
        /// <summary>
        /// Serializes an object into an xml document in memory
        /// </summary>
        /// <param name="pObject">the object that we want to serialize</param>
        /// <returns>The xml document from the object</returns>
        public static XmlDocument SerializeObjectToXmlDocument(Object pObject, XmlSerializerNamespaces pNamespaces)
        {
            if (pObject != null)
            {
                StringBuilder sb = new StringBuilder();
                StringWriter writer = null;
                try
                {
                    writer = new StringWriter(sb);
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
                StringReader tr = new StringReader(sb.ToString());
                XmlTextReader sr = new XmlTextReader(tr);
                return Parse(sr);
            }
            return null;
        }
        /// <summary>
        /// Serializes an object into an xml document at the given path or file name using UTF-8 Encoding
        /// </summary>
        /// <param name="pObject">the object that we want to serialize</param>
        /// <param name="pPath">The path to the file we want to write</param>
        public static XmlDocument SerializeObject(Object pObject, string pPath)
        {
            return SerializeObject(pObject, pPath, Encoding.UTF8);
        }
        /// <summary>
        /// Serializes an object into an xml document at the given path or file name using UTF-8 Encoding
        /// </summary>
        /// <param name="pObject">the object that we want to serialize</param>
        /// <param name="pPath">The path to the file we want to write</param>
        /// <param name="namespaces">The namespaces to use for serialization</param>
        /// <returns></returns>
        public static XmlDocument SerializeObject(Object pObject, string pPath, XmlSerializerNamespaces namespaces)
        {
            return SerializeObject(pObject, pPath, Encoding.UTF8, namespaces);
        }
        /// <summary>
        /// Serializes an object into an xml document at the given path or file name
        /// </summary>
        /// <param name="pObject">the object that we want to serialize</param>
        /// <param name="pPath">The path to the file we want to write</param>
        /// <param name="pEncoding">The encoding type to use i.e. utf-8</param>
        public static XmlDocument SerializeObject(Object pObject, string pPath, Encoding pEncoding)
        {
            return SerializeObject(pObject, pPath, pEncoding, null);
        }
        /// <summary>
        /// Serializes an object into an xml document at the given path or file name
        /// </summary>
        /// <param name="pObject">the object that we want to serialize</param>
        /// <param name="pPath">The path to the file we want to write</param>
        /// <param name="pSettings">The settings of the writer for formatting</param>
        public static XmlDocument SerializeObject(Object pObject, string pPath, XmlWriterSettings pSettings)
        {
            return SerializeObject(pObject, pPath, pSettings, null);
        }
        /// <summary>
        /// Serializes an object into an xml document at the given path or file name
        /// </summary>
        /// <param name="pObject">the object that we want to serialize</param>
        /// <param name="pPath">The path to the file we want to write</param>
        /// <param name="pSettings">The settings of the writer for formatting</param>
        /// <param name="pNamespaces">Namespaces to use when serializing</param>
        public static XmlDocument SerializeObject(Object pObject, string pPath, XmlWriterSettings pSettings, XmlSerializerNamespaces pNamespaces)
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
                return Parse(pPath);
            }
            return null;
        }
        /// <summary>
        /// Serializes an object into an xml document at the given path or file name
        /// </summary>
        /// <param name="pObject">the object that we want to serialize</param>
        /// <param name="pPath">The path to the file we want to write</param>
        /// <param name="pEncoding">The encoding type to use i.e. utf-8</param>
        /// <param name="namespaces">Namesapces to use when serializing</param>
        public static XmlDocument SerializeObject(Object pObject, string pPath, Encoding pEncoding, XmlSerializerNamespaces namespaces)
        {

            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.Indent = true;
            writerSettings.IndentChars = "  ";
            writerSettings.Encoding = pEncoding;
            return SerializeObject(pObject, pPath, writerSettings, namespaces);
        }
        /// <summary>
        /// Deserializes an object from a file located at the given path
        /// </summary>
        /// <param name="pObjectType"></param>
        /// <param name="pPath"></param>
        /// <returns></returns>
        public static object DeSerializeObject(Type pObjectType, string pPath)
        {
            Stream s = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(pObjectType);
                s = new FileStream(pPath, FileMode.Open, FileAccess.Read);
                return serializer.Deserialize(s);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                }
            }
        }
        /// <summary>
        /// Deserializes an object of the generic given type T from an XML string
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string xmlString)
        {
            return Deserialize<T>(xmlString, new UTF8Encoding());
        }
        /// <summary>
        /// Deserializes an object of the generic given type T from an XML string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlString"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string xmlString, Encoding encoding)
        {
            XmlTextWriter xmlTextWriter = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                byte[] byteArray = encoding.GetBytes(xmlString);
                MemoryStream memoryStream = new MemoryStream(byteArray);
                xmlTextWriter = new XmlTextWriter(memoryStream, encoding);

                return (T)serializer.Deserialize(memoryStream);
            }
            finally
            {
                if (xmlTextWriter != null) xmlTextWriter.Close();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oin"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Object[] ConvertObjects(Object[] oin, System.Type t)
        {
            if (oin == null)
                return null;

            ArrayList oout = new ArrayList();
            foreach (Object o in oin)
            {
                oout.Add(XmlUtility.ConvertObject(o, t));
            }

            return (Object[])oout.ToArray(t);
        }
        /// <summary>
        /// Transforms a document from a specified location using a stylesheet from a given 
        /// location and writing the results to the given target location
        /// </summary>
        /// <param name="srcDoc">location of the source document</param>
        /// <param name="stylesheet">location of the stylesheet</param>
        /// <param name="targetDoc">location of the target results</param>
        public static void Transform(string srcDoc, string stylesheet, string targetDoc)
        {
            XslCompiledTransform transform = new XslCompiledTransform();
            transform.Load(stylesheet);
            transform.Transform(srcDoc, targetDoc);
        }
        /// <summary>
        /// XSLT tranformation utility
        /// </summary>
        /// <param name="srcDoc"></param>
        /// <param name="styleSheet"></param>
        /// <returns></returns>
        public static XmlDocument Transform(XmlDocument srcDoc, XmlDocument styleSheet)
        {

            return Transform(srcDoc, styleSheet, null);
        }
        /// <summary>
        /// XSLT tranformation utility
        /// </summary>
        /// <param name="srcDoc"></param>
        /// <param name="styleSheet"></param>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public static XmlDocument Transform(XmlDocument srcDoc, XmlDocument styleSheet, XmlResolver resolver)
        {

            return Transform(srcDoc, styleSheet, null, resolver);
        }
        /// <summary>
        /// XSLT tranformation utility
        /// </summary>
        /// <param name="srcDoc"></param>
        /// <param name="styleSheet"></param>
        /// <param name="arguments"></param>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public static XmlDocument Transform(XmlDocument srcDoc,
                                              XmlDocument styleSheet,
                                              XsltArgumentList arguments,
                                              XmlResolver resolver)
        {
            return ParseFromString(TransformToString(srcDoc, styleSheet, arguments, resolver));
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

            try
            {
                StringBuilder sb = new StringBuilder();
                sw = new StringWriter(sb);

                XslCompiledTransform xsltransform = new XslCompiledTransform();
                
                XsltSettings settings = new XsltSettings();
                settings.EnableScript = true;
                settings.EnableDocumentFunction = true;
                xsltransform.Load(styleSheet, settings, resolver);

                xsltransform.Transform(srcDoc, arguments, sw);
                return sb.ToString();
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }

        /// <summary>
        /// Extract a node by name ignoring case. First try is the name as given, 
        /// then all lowercase, then all uppercase, then capitalized
        /// </summary>
        /// <param name="pParentNode">The parent node</param>
        /// <param name="pXPath">the xpath expression to find the node from the parent</param>
        /// <returns>The first node found in the parent with the given name ignoring case</returns>
        public static XmlNode SelectSingleNodeIgnoreCase(XmlNode pParentNode, string pXPath)
        {
            XmlNode node = null;
            node = pParentNode.SelectSingleNode(pXPath);
            if (node == null)
            {
                node = pParentNode.SelectSingleNode(pXPath.ToLower());
            }
            if (node == null)
            {
                node = pParentNode.SelectSingleNode(pXPath.ToUpper());
            }
            if (node == null)
            {
                node = pParentNode.SelectSingleNode(StringUtility.Capitalize(pXPath));
            }
            return node;
        }
        /// <summary>
        /// Extract a list of nodes by name ignoring case.
        /// First try is the name as given, then all uppercase, then capitalized
        /// </summary>
        /// <param name="pParentNode">The parent node</param>
        /// <param name="pXPath">the name</param>
        /// <returns>The list of nodes found in the parent with the given name ignoring case</returns>
        public static IList SelectNodesIgnoreCase(XmlNode pParentNode, string pXPath)
        {
            IList nodes = new ArrayList();
            XmlNodeList nodeList = pParentNode.SelectNodes(pXPath);

            if (!pXPath.Equals(pXPath.ToLower()))
            {
                XmlNodeList nodeListLower = pParentNode.SelectNodes(pXPath.ToLower());
                if (nodeListLower != null)
                {
                    foreach (XmlNode node in nodeListLower)
                    {
                        nodes.Add(node);
                    }
                }
            }
            if (!pXPath.Equals(pXPath.ToUpper()))
            {
                XmlNodeList nodeListUpper = pParentNode.SelectNodes(pXPath.ToUpper());
                if (nodeListUpper != null)
                {
                    foreach (XmlNode node in nodeListUpper)
                    {
                        nodes.Add(node);
                    }
                }
            }
            if (!pXPath.Equals(StringUtility.Capitalize(pXPath)))
            {
                XmlNodeList nodeListCaps = pParentNode.SelectNodes(StringUtility.Capitalize(pXPath));
                if (nodeListCaps != null)
                {
                    foreach (XmlNode node in nodeListCaps)
                    {
                        nodes.Add(node);
                    }
                }
            }
            if (nodeList != null)
            {
                foreach (XmlNode node in nodeList)
                {
                    nodes.Add(node);
                }
            }
            return nodes;
        }
        #region useful methods for getting and changing data in xml nodes

        #region Inner text from node methods
        /// <summary>
        /// Retrieves the text inside a node
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public static string ReadInnerTextFromNode(XmlDocument source, string xpath)
        {
            return ReadInnerTextFromNode(source, xpath, false, null);
        }
        /// <summary>
        /// Retrieves the text inside a node
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="ignoreNotFound"></param>
        /// <returns></returns>
        public static string ReadInnerTextFromNode(XmlDocument source, string xpath, bool ignoreNotFound)
        {
            return ReadInnerTextFromNode(source, xpath, ignoreNotFound, null);
        }
        /// <summary>
        /// Retrieves the text inside a node
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="ignoreNotFound"></param>
        /// <param name="nsManager"></param>
        /// <returns></returns>
        public static string ReadInnerTextFromNode(XmlDocument source, string xpath, bool ignoreNotFound, XmlNamespaceManager nsManager)
        {
            XmlNode node = null;
            if (nsManager == null)
            {
                node = source.SelectSingleNode(xpath);
            }
            else
            {
                node = source.SelectSingleNode(xpath, nsManager);
            }
            if (node != null)
            {
                return node.InnerText;
            }
            else if (!ignoreNotFound)
            {
                throw new ApplicationException(string.Format(
                              "XML.ReadInnerTextFromNode:Node for xpath {0} not found in source XML", xpath));
            }
            return null;
        }
        /// <summary>
        /// Replaces the text inside a node with a given text
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static XmlDocument ChangeInnerTextOfNode(XmlDocument source, string xpath, string text)
        {
            return ChangeInnerTextOfNode(source, xpath, text, false, null);
        }
        /// <summary>
        /// Replaces the text inside a node with a given text
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="text"></param>
        /// <param name="ignoreNotFound"></param>
        /// <returns></returns>
        public static XmlDocument ChangeInnerTextOfNode(XmlDocument source, string xpath, string text, bool ignoreNotFound)
        {
            return ChangeInnerTextOfNode(source, xpath, text, ignoreNotFound, null);
        }
        /// <summary>
        /// Replaces the text inside a node with a given text
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="text"></param>
        /// <param name="ignoreNotFound"></param>
        /// <param name="nsManager"></param>
        /// <returns></returns>
        public static XmlDocument ChangeInnerTextOfNode(XmlDocument source, string xpath, string text,
                                                        bool ignoreNotFound, XmlNamespaceManager nsManager)
        {
            return ReplaceInnerTextOfNode(source, xpath, null, text, ignoreNotFound, nsManager);
        }
        /// <summary>
        /// Replaces the text inside a node with a given text
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="original"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static XmlDocument ReplaceInnerTextOfNode(XmlDocument source, string xpath,
                                                        string original, string text)
        {
            return ReplaceInnerTextOfNode(source, xpath, original, text, false, null);
        }
        /// <summary>
        /// Replaces the text inside a node with a given text
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="original"></param>
        /// <param name="text"></param>
        /// <param name="ignoreNotFound"></param>
        /// <returns></returns>
        public static XmlDocument ReplaceInnerTextOfNode(XmlDocument source, string xpath,
                                                        string original, string text, bool ignoreNotFound)
        {
            return ReplaceInnerTextOfNode(source, xpath, original, text, ignoreNotFound, null);
        }
        /// <summary>
        /// Replaces the text inside a node with a given text
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="original"></param>
        /// <param name="text"></param>
        /// <param name="ignoreNotFound"></param>
        /// <param name="nsManager"></param>
        /// <returns></returns>
        public static XmlDocument ReplaceInnerTextOfNode(XmlDocument source, string xpath,
                                                        string original, string text, bool ignoreNotFound,
                                                        XmlNamespaceManager nsManager)
        {
            XmlNode node = null;
            if (nsManager == null)
            {
                node = source.SelectSingleNode(xpath);
            }
            else
            {
                node = source.SelectSingleNode(xpath, nsManager);
            }
            if (node != null)
            {
                if (string.IsNullOrEmpty(original))
                {

                    node.InnerText = text;
                }
                else
                {
                    node.InnerText = node.InnerText.Replace(original, text);
                }
            }
            else if (!ignoreNotFound)
            {
                throw new ApplicationException(string.Format(
                              "XML.ChangeInnerTextOfNode:Node for xpath {0} not found in source XML", xpath));
            }
            return source;
        }
        #endregion

        #region InnerText of node list methods
        /// <summary>
        /// Read the inner text of a node in a list of nodes whose attributes match the given conditions
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath">spath to the node list</param>
        /// <param name="conditions"> attribute names and values for matching conditions</param>
        /// <returns>inner text of first matching node for the given condition</returns>
        public static string ReadInnerTextFromNodeInList(XmlDocument source, string xpath, Dictionary<string, string> conditions)
        {
            return ReadInnerTextFromNodeInList(source, xpath, conditions, false, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="conditions"></param>
        /// <param name="ignoreNotFound"></param>
        /// <returns></returns>
        public static string ReadInnerTextFromNodeInList(XmlDocument source, string xpath,
                                                         Dictionary<string, string> conditions, bool ignoreNotFound)
        {
            return ReadInnerTextFromNodeInList(source, xpath, conditions, ignoreNotFound, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="conditions"></param>
        /// <param name="ignoreNotFound"></param>
        /// <param name="nsManager"></param>
        /// <returns></returns>
        public static string ReadInnerTextFromNodeInList(XmlDocument source, string xpath,
                                                         Dictionary<string, string> conditions,
                                                         bool ignoreNotFound,
                                                         XmlNamespaceManager nsManager)
        {
            XmlNode matchingNode = SelectFirstMatchingNodeFromNodeList(source, xpath, conditions, ignoreNotFound, nsManager);
            if (matchingNode != null)
            {
                return matchingNode.InnerText;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="conditions"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static XmlDocument ChangeInnerTextOfNodeInList(XmlDocument source, string xpath,
                                                         Dictionary<string, string> conditions,
                                                         string text)
        {
            return ChangeInnerTextOfNodeInList(source, xpath, conditions, text, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="conditions"></param>
        /// <param name="text"></param>
        /// <param name="ignoreNotFound"></param>
        /// <returns></returns>
        public static XmlDocument ChangeInnerTextOfNodeInList(XmlDocument source, string xpath,
                                                         Dictionary<string, string> conditions,
                                                         string text,
                                                         bool ignoreNotFound)
        {
            return ChangeInnerTextOfNodeInList(source, xpath, conditions, text, ignoreNotFound, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="conditions"></param>
        /// <param name="text"></param>
        /// <param name="ignoreNotFound"></param>
        /// <param name="nsManager"></param>
        /// <returns></returns>
        public static XmlDocument ChangeInnerTextOfNodeInList(XmlDocument source, string xpath,
                                                         Dictionary<string, string> conditions,
                                                         string text,
                                                         bool ignoreNotFound,
                                                         XmlNamespaceManager nsManager)
        {
            return ReplaceInnerTextOfNodeInList(source, xpath, conditions, null, text, ignoreNotFound, nsManager);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="conditions"></param>
        /// <param name="originalText"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static XmlDocument ReplaceInnerTextOfNodeInList(XmlDocument source, string xpath,
                                                         Dictionary<string, string> conditions,
                                                         string originalText,
                                                         string text)
        {
            return ReplaceInnerTextOfNodeInList(source, xpath, conditions, originalText, text, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="conditions"></param>
        /// <param name="originalText"></param>
        /// <param name="text"></param>
        /// <param name="ignoreNotFound"></param>
        /// <returns></returns>
        public static XmlDocument ReplaceInnerTextOfNodeInList(XmlDocument source, string xpath,
                                                         Dictionary<string, string> conditions,
                                                         string originalText,
                                                         string text,
                                                         bool ignoreNotFound)
        {
            return ReplaceInnerTextOfNodeInList(source, xpath, conditions, originalText, text, ignoreNotFound, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="conditions"></param>
        /// <param name="originalText"></param>
        /// <param name="text"></param>
        /// <param name="ignoreNotFound"></param>
        /// <param name="nsManager"></param>
        /// <returns></returns>
        public static XmlDocument ReplaceInnerTextOfNodeInList(XmlDocument source, string xpath,
                                                         Dictionary<string, string> conditions,
                                                         string originalText,
                                                         string text,
                                                         bool ignoreNotFound,
                                                         XmlNamespaceManager nsManager)
        {
            XmlNode matchingNode = SelectFirstMatchingNodeFromNodeList(source, xpath, conditions, ignoreNotFound, nsManager);

            if (matchingNode != null)
            {
                if (string.IsNullOrEmpty(originalText))
                {
                    matchingNode.InnerText = text;
                }
                else
                {
                    matchingNode.InnerText = matchingNode.InnerText.Replace(originalText, text);
                }
            }
            return source;
        }
        #endregion

        #region Attribute values of node list methods
        /// <summary>
        /// Read the inner text of a node in a list of nodes whose attributes match the given conditions
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath">spath to the node list</param>
        /// <param name="conditions"> attribute names and values for matching conditions</param>
        /// <param name="attribute"></param>
        /// <returns>inner text of first matching node for the given condition</returns>
        public static string ReadAttributeValueFromNodeInList(XmlDocument source, string xpath, Dictionary<string, string> conditions, string attribute)
        {
            return ReadAttributeValueFromNodeInList(source, xpath, conditions, attribute, false, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="conditions"></param>
        /// <param name="attribute"></param>
        /// <param name="ignoreNotFound"></param>
        /// <returns></returns>
        public static string ReadAttributeValueFromNodeInList(XmlDocument source, string xpath,
                                                          Dictionary<string, string> conditions,
                                                          string attribute,
                                                          bool ignoreNotFound)
        {
            return ReadAttributeValueFromNodeInList(source, xpath, conditions, attribute, ignoreNotFound, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="conditions"></param>
        /// <param name="attribute"></param>
        /// <param name="ignoreNotFound"></param>
        /// <param name="nsManager"></param>
        /// <returns></returns>
        public static string ReadAttributeValueFromNodeInList(XmlDocument source, string xpath,
                                                          Dictionary<string, string> conditions,
                                                          string attribute,
                                                          bool ignoreNotFound, XmlNamespaceManager nsManager)
        {
            XmlNode matchingNode = SelectFirstMatchingNodeFromNodeList(source, xpath, conditions, ignoreNotFound, nsManager);

            if (matchingNode != null)
            {
                XmlAttribute att = matchingNode.Attributes[attribute];
                if (att != null)
                {
                    return att.Value;
                }
                else if (!ignoreNotFound)
                {
                    throw new ApplicationException(string.Format(
                                  "XML.ReadAttributeValueFromNodeInList:Attribute in node from list for xpath {0} not found in source XML", xpath));
                }
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="conditions"></param>
        /// <param name="attribute"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static XmlDocument ChangeAttributeValueOfNodeInList(XmlDocument source, string xpath,
                                                         Dictionary<string, string> conditions,
                                                         string attribute,
                                                         string text)
        {
            return ChangeAttributeValueOfNodeInList(source, xpath, conditions, attribute, text, false, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="conditions"></param>
        /// <param name="attribute"></param>
        /// <param name="text"></param>
        /// <param name="ignoreNotFound"></param>
        /// <returns></returns>
        public static XmlDocument ChangeAttributeValueOfNodeInList(XmlDocument source, string xpath,
                                                         Dictionary<string, string> conditions,
                                                         string attribute,
                                                         string text,
                                                         bool ignoreNotFound)
        {
            return ChangeAttributeValueOfNodeInList(source, xpath, conditions, attribute, text, ignoreNotFound, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="conditions"></param>
        /// <param name="attribute"></param>
        /// <param name="text"></param>
        /// <param name="ignoreNotFound"></param>
        /// <param name="nsManager"></param>
        /// <returns></returns>
        public static XmlDocument ChangeAttributeValueOfNodeInList(XmlDocument source, string xpath,
                                                         Dictionary<string, string> conditions,
                                                         string attribute,
                                                         string text,
                                                         bool ignoreNotFound, XmlNamespaceManager nsManager)
        {
            return ReplaceAttributeValueOfNodeInList(source, xpath, conditions, attribute, null, text, ignoreNotFound, nsManager);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="conditions"></param>
        /// <param name="attribute"></param>
        /// <param name="originalText"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static XmlDocument ReplaceAttributeValueOfNodeInList(XmlDocument source, string xpath,
                                                         Dictionary<string, string> conditions,
                                                         string attribute,
                                                         string originalText,
                                                         string text)
        {
            return ReplaceAttributeValueOfNodeInList(source, xpath, conditions, attribute, originalText, text, false, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="conditions"></param>
        /// <param name="attribute"></param>
        /// <param name="originalText"></param>
        /// <param name="text"></param>
        /// <param name="ignoreNotFound"></param>
        /// <returns></returns>
        public static XmlDocument ReplaceAttributeValueOfNodeInList(XmlDocument source, string xpath,
                                                         Dictionary<string, string> conditions,
                                                         string attribute,
                                                         string originalText,
                                                         string text,
                                                         bool ignoreNotFound)
        {

            return ReplaceAttributeValueOfNodeInList(source, xpath, conditions, attribute, originalText, text, ignoreNotFound, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="conditions"></param>
        /// <param name="attribute"></param>
        /// <param name="originalText"></param>
        /// <param name="text"></param>
        /// <param name="ignoreNotFound"></param>
        /// <param name="nsManager"></param>
        /// <returns></returns>
        public static XmlDocument ReplaceAttributeValueOfNodeInList(XmlDocument source, string xpath,
                                                         Dictionary<string, string> conditions,
                                                         string attribute,
                                                         string originalText,
                                                         string text,
                                                         bool ignoreNotFound,
                                                         XmlNamespaceManager nsManager)
        {
            XmlNode matchingNode = SelectFirstMatchingNodeFromNodeList(source, xpath, conditions, ignoreNotFound, nsManager);
            if (matchingNode != null)
            {
                XmlAttribute att = matchingNode.Attributes[attribute];
                if (att != null)
                {
                    if (string.IsNullOrEmpty(originalText))
                    {
                        att.Value = text;
                    }
                    else
                    {
                        att.Value = att.Value.Replace(originalText, text);
                    }
                }
                else if (!ignoreNotFound)
                {
                    throw new ApplicationException(string.Format(
                                  "XML.ReadInnerTextFromNodeInList:Attribute for xpath {0} with {1} conditions not found in source XML",
                                  xpath, conditions.Count));
                }
            }
            return source;
        }
        #endregion

        #region node attribute methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpathNode"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static string ReadValueFromNodeAttribute(XmlDocument source, string xpathNode, string attributeName)
        {
            return ReadValueFromNodeAttribute(source, xpathNode, attributeName, false, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpathNode"></param>
        /// <param name="attributeName"></param>
        /// <param name="ignoreNotFound"></param>
        /// <returns></returns>
        public static string ReadValueFromNodeAttribute(XmlDocument source, string xpathNode, string attributeName, bool ignoreNotFound)
        {
            return ReadValueFromNodeAttribute(source, xpathNode, attributeName, ignoreNotFound, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpathNode"></param>
        /// <param name="attributeName"></param>
        /// <param name="ignoreNotFound"></param>
        /// <param name="nsManager"></param>
        /// <returns></returns>
        public static string ReadValueFromNodeAttribute(XmlDocument source, string xpathNode, string attributeName, bool ignoreNotFound, XmlNamespaceManager nsManager)
        {
            XmlNode node = null;
            if (nsManager == null)
            {
                node = source.SelectSingleNode(xpathNode);
            }
            else
            {
                node = source.SelectSingleNode(xpathNode, nsManager);
            }
            if (node != null)
            {
                XmlAttribute att = node.Attributes[attributeName];
                if (att != null)
                {
                    return att.Value;
                }
            }
            if (!ignoreNotFound)
            {
                throw new ApplicationException(string.Format(
                              "XML.ReadValueFromNodeAttribute:Node attribute for xpath {0} and attribute name {1} not found in source XML", xpathNode, attributeName));
            }
            return null;
        }
        public static XmlNode SetOrAddAttributeToNode(XmlDocument source, XmlNode node, string attributeName, string attributeValue)
        {
            bool found = false;
            
            foreach (XmlAttribute a in node.Attributes)
            {
                if (attributeName.Equals(a.Name))
                {
                    a.Value = attributeValue;
                    found = true;
                }
            }
            if (!found)
            {
                XmlAttribute addyAtt = source.CreateAttribute(attributeName);
                addyAtt.Value = attributeValue;
                node.Attributes.Append(addyAtt);
            }
            return node;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="attribute"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static XmlDocument ChangeValueOfNodeAttribute(XmlDocument source, string xpath, string attribute, string text)
        {
            return ChangeValueOfNodeAttribute(source, xpath, attribute, text, false, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="attribute"></param>
        /// <param name="text"></param>
        /// <param name="ignoreNotFound"></param>
        /// <returns></returns>
        public static XmlDocument ChangeValueOfNodeAttribute(XmlDocument source, string xpath,
                                                             string attribute, string text, bool ignoreNotFound)
        {
            return ChangeValueOfNodeAttribute(source, xpath, attribute, text, ignoreNotFound, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="attribute"></param>
        /// <param name="text"></param>
        /// <param name="ignoreNotFound"></param>
        /// <param name="nsManager"></param>
        /// <returns></returns>
        public static XmlDocument ChangeValueOfNodeAttribute(XmlDocument source, string xpath,
                                                             string attribute, string text, bool ignoreNotFound, XmlNamespaceManager nsManager)
        {
            return ReplaceValueOfNodeAttribute(source, xpath, attribute, null, text, ignoreNotFound, nsManager);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="attribute"></param>
        /// <param name="original"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static XmlDocument ReplaceValueOfNodeAttribute(XmlDocument source, string xpath,
                                                             string attribute, string original, string text)
        {
            return ReplaceValueOfNodeAttribute(source, xpath, attribute, original, text, false, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="attribute"></param>
        /// <param name="original"></param>
        /// <param name="text"></param>
        /// <param name="ignoreNotFound"></param>
        /// <returns></returns>
        public static XmlDocument ReplaceValueOfNodeAttribute(XmlDocument source, string xpath,
                                                             string attribute, string original,
                                                             string text, bool ignoreNotFound)
        {
            return ReplaceValueOfNodeAttribute(source, xpath, attribute, original, text, ignoreNotFound, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="attribute"></param>
        /// <param name="original"></param>
        /// <param name="text"></param>
        /// <param name="ignoreNotFound"></param>
        /// <param name="nsManager"></param>
        /// <returns></returns>
        public static XmlDocument ReplaceValueOfNodeAttribute(XmlDocument source, string xpath,
                                                             string attribute, string original,
                                                             string text, bool ignoreNotFound, XmlNamespaceManager nsManager)
        {
            XmlNode node = null;
            if (nsManager == null)
            {
                node = source.SelectSingleNode(xpath);
            }
            else
            {
                node = source.SelectSingleNode(xpath, nsManager);
            }
            if (node != null)
            {
                XmlAttribute att = node.Attributes[attribute];
                if (att != null)
                {
                    if (string.IsNullOrEmpty(original))
                    {
                        att.Value = text;
                    }
                    else
                    {
                        att.Value = att.Value.Replace(original, text);
                    }
                }
            }
            else if (!ignoreNotFound)
            {
                throw new ApplicationException(string.Format(
                              "XML.ChangeInnerTextOfNode:Node for xpath {0} not found in source XML", xpath));
            }
            return source;
        }
        #endregion

        #region selecting nodes based on attribute values
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public static XmlNode SelectFirstMatchingNodeFromNodeList(XmlDocument source, string xpath,
                                               Dictionary<string, string> conditions)
        {
            return SelectFirstMatchingNodeFromNodeList(source, xpath, conditions, false, null);
        }
        /// <summary>
        /// Selects the first node that matches all the given attribute conditions
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath">path to node list</param>
        /// <param name="conditions">name value pairs for conditions on attribute names and values of the node to select</param>
        /// <param name="ignoreNotFound"></param>
        /// <returns></returns>
        public static XmlNode SelectFirstMatchingNodeFromNodeList(XmlDocument source, string xpath,
                                               Dictionary<string, string> conditions,
                                               bool ignoreNotFound)
        {
            return SelectFirstMatchingNodeFromNodeList(source, xpath, conditions, ignoreNotFound, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="conditions"></param>
        /// <param name="ignoreNotFound"></param>
        /// <param name="nsManager"></param>
        /// <returns></returns>
        public static XmlNode SelectFirstMatchingNodeFromNodeList(XmlDocument source, string xpath,
                                               Dictionary<string, string> conditions,
                                               bool ignoreNotFound, XmlNamespaceManager nsManager)
        {
            XmlNodeList nodes = null;
            if (nsManager == null)
            {
                nodes = source.SelectNodes(xpath);
            }
            else
            {
                nodes = source.SelectNodes(xpath, nsManager);
            }
            if (nodes != null && nodes.Count > 0)
            {
                foreach (XmlNode node in nodes)
                {
                    bool match = true;
                    foreach (string attributeName in conditions.Keys)
                    {
                        XmlAttribute att = node.Attributes[attributeName];
                        if (att != null)
                        {
                            if (!att.Value.Equals(conditions[attributeName]))
                            {
                                match = false;
                                break;
                            }
                        }
                        else
                        {
                            match = false;
                            break;
                        }

                    }
                    if (match)
                    {
                        return node;
                    }
                }
            }
            else if (!ignoreNotFound)
            {
                throw new ApplicationException(string.Format(
                              "XML.SelectNodeFromNodeList:Node list for xpath {0} not found in source XML", xpath));
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public static List<XmlNode> SelectAllMatchingNodesFromNodeList(XmlDocument source, string xpath,
                                               Dictionary<string, string> conditions)
        {
            return SelectAllMatchingNodesFromNodeList(source, xpath, conditions, false, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="conditions"></param>
        /// <param name="ignoreNotFound"></param>
        /// <returns></returns>
        public static List<XmlNode> SelectAllMatchingNodesFromNodeList(XmlDocument source, string xpath,
                                               Dictionary<string, string> conditions,
                                               bool ignoreNotFound)
        {
            return SelectAllMatchingNodesFromNodeList(source, xpath, conditions, ignoreNotFound, null);
        }
        /// <summary>
        /// Retruns all nodes that match the given conditions for attribute values
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xpath"></param>
        /// <param name="conditions"></param>
        /// <param name="ignoreNotFound"></param>
        /// <param name="nsManager"></param>
        /// <returns></returns>
        public static List<XmlNode> SelectAllMatchingNodesFromNodeList(XmlDocument source, string xpath,
                                               Dictionary<string, string> conditions,
                                               bool ignoreNotFound, XmlNamespaceManager nsManager)
        {
            XmlNodeList nodes = null;
            if (nsManager == null)
            {
                nodes = source.SelectNodes(xpath);
            }
            else
            {
                nodes = source.SelectNodes(xpath, nsManager);
            }
            if (nodes != null && nodes.Count > 0)
            {
                List<XmlNode> matchingNodes = new List<XmlNode>();
                foreach (XmlNode node in nodes)
                {
                    bool match = true;
                    foreach (string attributeName in conditions.Keys)
                    {
                        XmlAttribute att = node.Attributes[attributeName];
                        if (att != null)
                        {
                            if (!att.Value.Equals(conditions[attributeName]))
                            {
                                match = false;
                                break;
                            }
                        }
                        else
                        {
                            match = false;
                            break;
                        }

                    }
                    if (match)
                    {
                        matchingNodes.Add(node);
                    }
                }
                return matchingNodes;
            }
            else if (!ignoreNotFound)
            {
                throw new ApplicationException(string.Format(
                              "XML.SelectNodeFromNodeList:Node list for xpath {0} not found in source XML", xpath));
            }
            return null;
        }
        #endregion

        #endregion

        public static XmlDocument ToXmlDocument(XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        public static XmlDocument ToXmlDocument(XElement xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        public static XmlDocument ToXmlDocument(XNode xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        public static XDocument ToXDocument(XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }

        public static string ToXmlString(XDocument document)
        {
            StringBuilder sb = new StringBuilder();
            using (TextWriter tr = new StringWriter(sb))
            {
                document.Save(tr);
                return sb.ToString();
            }
        }
        /// <summary> Main method for testing purposes
        /// </summary>
        [STAThread]
        public static void Main(String[] args)
        {

            if (args.Length <= 0)
            {
                Console.Out.WriteLine("Usage: .exe args");
                return;
            }

            String url = args[0];

            try
            {
                XmlDocument doc = XmlUtility.Parse(url);
                XmlUtility.Write(doc, Console.Out);
                // Process the root element
            }
            catch
            {
                Console.Out.WriteLine(url + " IO Exception.");
            }
        }
        // end main
    }
}
