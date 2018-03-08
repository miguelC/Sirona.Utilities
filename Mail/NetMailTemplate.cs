/* 
 * $URL$
 * $Revision$
 * $Author$
 * $Date$
 * $LastChangedBy$
 * $LastChangedDate$
 * 
 * ====================================================================
 * 
 *  
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml.XPath;
using Sirona.Utilities.Strings;
using Sirona.Utilities.XML;
using Sirona.Utilities.IO;

namespace Sirona.Utilities.Mail
{
  /// <summary>
  /// A templating fixture for composing email bodies
  /// </summary>
  public class NetMailTemplate
  {
    private string foreachKey = "foreach|";
    /// <summary>
    /// Start of a collection loop
    /// </summary>
    public string ForeachKey
    {
      get { return foreachKey; }
      set { foreachKey = value; }
    }

    private string foreachKeyEnd = "endforeach";
    /// <summary>
    /// End of the collection loop
    /// </summary>
    public string ForeachKeyEnd
    {
      get { return foreachKeyEnd; }
      set { foreachKeyEnd = value; }
    }

    private string foreachItemKey = "item";
    /// <summary>
    /// text that identifies an item in a collection while cycling through it
    /// </summary>
    public string ForeachItemKey
    {
      get { return foreachItemKey; }
      set { foreachItemKey = value; }
    }

    private string dataStartDelimiter = "@{";
    /// <summary>
    /// Start of the data object reference
    /// </summary>
    public string DataStartDelimiter
    {
      get { return dataStartDelimiter; }
      set { dataStartDelimiter = value; }
    }

    private string dataEndDelimiter = "}@";
    /// <summary>
    /// End of delimiter of a data object reference
    /// </summary>
    public string DataEndDelimiter
    {
      get { return dataEndDelimiter; }
      set { dataEndDelimiter = value; }
    }

    private string collectionIndexStart = "[";
    /// <summary>
    /// start of the index of a collection reference
    /// </summary>
    public string CollectionIndexStart
    {
      get { return collectionIndexStart; }
      set { collectionIndexStart = value; }
    }

    private string collectionIndexEnd = "]";
    /// <summary>
    /// End of the index of a collection reference
    /// </summary>
    public string CollectionIndexEnd
    {
      get { return collectionIndexEnd; }
      set { collectionIndexEnd = value; }
    }

    private string nullString = "[null]";
    /// <summary>
    /// Text to use when null values are found
    /// </summary>
    public string NullString
    {
      get { return nullString; }
      set { nullString = value; }
    }


    private string messageSubject;
    /// <summary>
    /// The subject line
    /// </summary>
    public string MessageSubject
    {
      get { return messageSubject; }
      set { messageSubject = value; }
    }
    private string messageBodyFile;
    /// <summary>
    /// The template file
    /// </summary>
    public string MessageBodyFile
    {
      get { return messageBodyFile; }
      set { messageBodyFile = value; }
    }

    private string messageBody;
    /// <summary>
    /// The template file
    /// </summary>
    public string MessageBody
    {
        get { return messageBody; }
        set { messageBody = value; }
    }

    private NetMailTemplateTextStyle textStyle = NetMailTemplateTextStyle.Text;
    /// <summary>
    /// The style of the email body
    /// </summary>
    public NetMailTemplateTextStyle BodyStyle
    {
      get { return textStyle; }
      set { textStyle = value; }
    }

    private NetMailTemplateParserType parserType = NetMailTemplateParserType.Text;
    /// <summary>
    /// The type of parser to use
    /// </summary>
    public NetMailTemplateParserType BodyParserType
    {
      get { return parserType; }
      set { parserType = value; }
    }

    private string xslParserRootNodeName = "MailObjects";
    /// <summary>
    /// The name of the node that is to be the root of the xml that contains the objects fed into the xsl template
    /// </summary>
    public string XslParserRootNodeName
    {
      get { return xslParserRootNodeName; }
      set { xslParserRootNodeName = value; }
    }

    private string xslParserObjectNodeName = "add";
    /// <summary>
    /// The name of the node that is to contain the XML of each object.
    /// This node is to have an attribute that defines the name of the object in the list.
    /// </summary>
    public string XslParserObjectNodeName
    {
      get { return xslParserObjectNodeName; }
      set { xslParserObjectNodeName = value; }
    }
    private string xslParserObjectNameAttributeName = "name";
    /// <summary>
    /// The name of an attribute that defines the name of the object in the list.
    /// </summary>
    public string XslParserObjectNameAttributeName
    {
      get { return xslParserObjectNameAttributeName; }
      set { xslParserObjectNameAttributeName = value; }
    }

    /// <summary>
    /// Turns an array of objects into a dictionary of objects with the type name as keys
    /// </summary>
    /// <param name="objects"></param>
    /// <returns></returns>
    protected Dictionary<string, object> GetDictionaryOfObjects(object[] objects)
    {
      Dictionary<string, object> dict = new Dictionary<string, object>();
      foreach (object o in objects)
      {
        string typeName = o.GetType().Name;
        dict.Add(typeName, o);
      }
      return dict;
    }
    /// <summary>
    /// Replaces any appearances of escaped values to the value of the objects passed in
    /// </summary>
    /// <param name="source"></param>
    /// <param name="objDict"></param>
    /// <returns></returns>
    public string ReplaceAllObjectReferencesToValues(string source, Dictionary<string, object> objDict)
    {
      string text = TemplateTextParser.ParseObjectsIntoString(
                                                    this.DataStartDelimiter,
                                                    this.DataEndDelimiter,
                                                    this.CollectionIndexStart,
                                                    this.CollectionIndexEnd,
                                                    this.NullString,
                                                    this.ForeachKey,
                                                    this.ForeachKeyEnd,
                                                    this.ForeachItemKey,
                                                    objDict,
                                                    source);

      return text;
    }
    /// <summary>
    /// Geenrates the subject text by parsing the values in a 
    /// predefined subject text string corresponding to values from the given array of objects
    /// </summary>
    /// <param name="dataObjects"></param>
    /// <returns></returns>
    public string GenerateSubject(Dictionary<string, object> dataObjects)
    {
      return this.ReplaceAllObjectReferencesToValues(this.MessageSubject, dataObjects);
    }
    /// <summary>
    /// Generates the body of the message based on the template settings and the given object array
    /// </summary>
    /// <param name="dataObjects"></param>
    /// <returns></returns>
    public string GenerateBody(Dictionary<string, object> dataObjects)
    {
      string text = null;
      if (this.BodyParserType == NetMailTemplateParserType.Text)
      {
          if (!string.IsNullOrEmpty(this.MessageBody))
          {
              text = this.MessageBody;
          }
          else
          {
              text = FileUtility.ReadFileAsStringBuffer(this.MessageBodyFile).ToString();
          }
          text = TemplateTextParser.ParseObjectsIntoString(
                                                      this.DataStartDelimiter,
                                                      this.DataEndDelimiter,
                                                      this.CollectionIndexStart,
                                                      this.CollectionIndexEnd,
                                                      this.NullString,
                                                      this.ForeachKey,
                                                      this.ForeachKeyEnd,
                                                      this.ForeachItemKey,
                                                      dataObjects,
                                                      text);
      }
      else
      {
        StringBuilder allObjectsXML = new StringBuilder();
        allObjectsXML.Append("<?xml version=\"1.0\"?>");
        allObjectsXML.Append(Environment.NewLine).Append("<").Append(this.XslParserRootNodeName).Append(">");
        if (dataObjects != null)
        {
          foreach (string key in dataObjects.Keys)
          {
            allObjectsXML.Append(Environment.NewLine).Append("<");
            allObjectsXML.Append(this.XslParserObjectNodeName).Append(" ");
            allObjectsXML.Append(this.XslParserObjectNameAttributeName).Append("=\"").Append(key).Append("\">");
            
            object obj = dataObjects[key];
            XPathDocument objDoc = XmlXPathUtility.SerializeObject(obj);
            allObjectsXML.Append(Environment.NewLine).Append(objDoc.CreateNavigator().InnerXml);

            allObjectsXML.Append(Environment.NewLine).Append("</").Append(this.XslParserObjectNodeName).Append(">");
          }
        }
        allObjectsXML.Append(Environment.NewLine).Append("</").Append(this.XslParserRootNodeName).Append(">");
        XPathDocument objsDoc = XmlXPathUtility.ParseFromString(allObjectsXML.ToString());
        XPathDocument template = null;
        if (string.IsNullOrEmpty(this.MessageBody))
        {
            template = XmlXPathUtility.Parse(this.MessageBodyFile);
        }
        else
        {
            template = XmlXPathUtility.ParseFromString(this.MessageBody);
        }
        if (this.BodyStyle == NetMailTemplateTextStyle.Text)
        {
          text = XmlUtility.TransformToString(objsDoc, template, null, null);
        }
        else
        {
          XPathDocument result = XmlXPathUtility.Transform(objsDoc, template);
          text = result.CreateNavigator().InnerXml;
        }
      }
      return text;
    }
    
  }
  /// <summary>
  /// Defines the style of the email message
  /// </summary>
  public enum NetMailTemplateTextStyle
  {
    /// <summary>
    /// Email is plain text
    /// </summary>
    Text, 
    /// <summary>
    /// Email is HTML
    /// </summary>
    HTML
  }
  /// <summary>
  /// Defines the type of parser to use
  /// </summary>
  public enum NetMailTemplateParserType
  {
    /// <summary>
    /// Uses the text based parser
    /// </summary>
    Text, 
    /// <summary>
    /// Uses an xslt transform parser
    /// </summary>
    XSLT
  }
}
