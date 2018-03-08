using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Sirona.Utilities.Strings
{

  /// <summary>
  /// Parses text that includes escapes sequences of values that define objects from an object list
  /// </summary>
  public static class TemplateTextParser
  {

    public const string DEFAULT_EscapeMarkerStart = "@{";
    public const string DEFAULT_EscapeMarkerEnd = "}@";
    public const string DEFAULT_IndexedRefStart = "[";
    public const string DEFAULT_IndexedRefEnd = "]";
    public const string DEFAULT_NullString = "";
    public const string DEFAULT_ForeachKeyStart = "foreach|";
    public const string DEFAULT_ForeachKeyEnd = "endforeach";
    public const string DEFAULT_ForeachItemKey = "endforeach";


    public static string ParseObjectsIntoString(
                                                 Dictionary<string, object> objects,
                                                 string value)
    {
      return ParseObjectsIntoString(
        DEFAULT_EscapeMarkerStart, 
        DEFAULT_EscapeMarkerEnd, 
        DEFAULT_IndexedRefStart, 
        DEFAULT_IndexedRefEnd, 
        DEFAULT_NullString, 
        DEFAULT_ForeachKeyStart, 
        DEFAULT_ForeachKeyEnd, 
        DEFAULT_ForeachItemKey, 
        objects, 
        value);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="escapeMarkerStart"></param>
    /// <param name="escapeMarkerEnd"></param>
    /// <param name="indexedRefStart"></param>
    /// <param name="indexedRefEnd"></param>
    /// <param name="nullString"></param>
    /// <param name="foreachKeyStart"></param>
    /// <param name="foreachKeyEnd"></param>
    /// <param name="foreachItemKey"></param>
    /// <param name="objects"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ParseObjectsIntoString(
                                                 string escapeMarkerStart,
                                                 string escapeMarkerEnd,
                                                 string indexedRefStart,
                                                 string indexedRefEnd,
                                                 string nullString,
                                                 string foreachKeyStart,
                                                 string foreachKeyEnd,
                                                 string foreachItemKey,
                                                 Dictionary<string, object> objects, 
                                                 string value)
    {
      if (string.IsNullOrEmpty(value))
      {
        return value;
      }
      if (objects != null)
      {
        value = ResolveLists(  escapeMarkerStart,
                             escapeMarkerEnd,
                             indexedRefStart,
                             indexedRefEnd,
                             nullString,
                             foreachKeyStart,
                             foreachKeyEnd,
                             foreachItemKey,
                             objects,
                             value);
        while (StringUtility.ContainsDelimiters(value, escapeMarkerStart, escapeMarkerEnd))
        {
          string refName = StringUtility.ExtractFromDelimiters(value, escapeMarkerStart, escapeMarkerEnd);
          object refValue = GetObjectFromReference( 
                                             indexedRefStart, 
                                             indexedRefEnd, 
                                             objects, 
                                             refName);
          string refValString = refValue == null ? nullString : refValue.ToString();
          value = StringUtility.ReplaceIgnoreCase(value, escapeMarkerStart + refName + escapeMarkerEnd, refValString);
        }
      }
      return value;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="objects"></param>
    /// <param name="refName"></param>
    /// <returns></returns>
    public static object GetObjectFromReference(
                                                   Dictionary<string, object> objects,
                                                   string refName
                                                   )
    {
        return GetObjectFromReference("[", "]", objects, refName);
    }
    /// <summary>
    /// resolves a value from a set of objects base on a reference that contains the key to the object in the dictionary
    /// and optionally a dot notation reference to properties of the object.
    /// </summary>
    /// <param name="indexedRefStart">an indexed reference start string such as [ in some.reference[index]</param>
    /// <param name="indexedRefEnd">an indexed reference start string such as ] in some.reference[index]</param>
    /// <param name="objects"></param>
    /// <param name="refName"></param>
    /// <returns></returns>
    public static object GetObjectFromReference(
                                                   string indexedRefStart,
                                                   string indexedRefEnd,
                                                   Dictionary<string, object> objects,
                                                   string refName
                                                   )
    {
      object refValue = null;
      int firstIndexOfDot = refName.IndexOf(".");
      int firstIndexOfSquareBracket = refName.IndexOf(indexedRefStart);
      if (StringUtility.Contains(refName, ".") &&
                 (firstIndexOfSquareBracket < 0 || firstIndexOfDot < firstIndexOfSquareBracket))
      {
        StringTokenizer tokenizer = new StringTokenizer(refName, ".");
        string envName = tokenizer.NextToken();
        object obj = null;
        if (StringUtility.NotEmpty(envName))
        {
          obj = objects[envName];
          if (obj != null)
          {
            while (tokenizer.HasMoreTokens())
            {
              if (obj == null)
              {
                break;
              }
              string token = tokenizer.NextToken();
              string propName = token;
              if (StringUtility.NotEmpty(token))
              {
                bool hasIndex = false;
                if (StringUtility.Contains(token, indexedRefStart))
                {
                  hasIndex = true;
                  propName = token.Substring(0, token.IndexOf(indexedRefStart));
                }
                PropertyInfo prop = obj.GetType().GetProperty(propName);
                if (prop != null && prop.CanRead)
                {
                  if ( typeof(IDictionary).IsAssignableFrom( prop.PropertyType))
                  {
                    IDictionary dictObj = (IDictionary)prop.GetValue(obj, null);
                    if (hasIndex)
                    {
                      obj = dictObj[StringUtility.ExtractFromDelimiters(token, indexedRefStart, indexedRefEnd)];
                    }
                    else
                    {
                      obj = dictObj;
                    }
                  }
                  else if ( typeof(IList).IsAssignableFrom( prop.PropertyType ))
                  {
                    IList listObj = (IList)prop.GetValue(obj, null);
                    if (hasIndex)
                    {
                      try
                      {
                        int indx = Convert.ToInt32(StringUtility.ExtractFromDelimiters(token, indexedRefStart, indexedRefEnd));
                        obj = listObj[indx];
                      }
                      catch
                      {
                        obj = listObj;
                      }
                    }
                    else
                    {
                      obj = listObj;
                    }
                  }
                  else
                  {
                    obj = prop.GetValue(obj, null);
                  }
                }
                else
                {
                  obj = null;
                  break;
                }
              }
            }
          }
        }
        refValue = obj;
      }
      else
      {
        bool hasIndex = false;
        string propName = refName;
        if (StringUtility.Contains(refName, indexedRefStart))
        {
          hasIndex = true;
          propName = refName.Substring(0, refName.IndexOf(indexedRefStart));
        }
        object obj = objects[propName];
        if (obj == null)
        {
          refValue = null;
        }
        else
        {

          if (hasIndex)
          {
            if (typeof(IDictionary).IsAssignableFrom(obj.GetType()))
            {
                IDictionary dictObj = (IDictionary)obj;
                obj = dictObj[StringUtility.ExtractFromDelimiters(refName, indexedRefStart, indexedRefEnd)];
            }
            else if (typeof(IList).IsAssignableFrom(obj.GetType()))
            {
                IList listObj = (IList)obj;
                int indx = Convert.ToInt32(StringUtility.ExtractFromDelimiters(refName, indexedRefStart, indexedRefEnd));
                obj = listObj[indx];
            }
          }
          refValue = obj;
        }
      }
      return refValue;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="escapeMarkerStart"></param>
    /// <param name="escapeMarkerEnd"></param>
    /// <param name="indexedRefStart"></param>
    /// <param name="indexedRefEnd"></param>
    /// <param name="nullString"></param>
    /// <param name="foreachKeyStart"></param>
    /// <param name="foreachKeyEnd"></param>
    /// <param name="foreachItemKey"></param>
    /// <param name="objDict"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ResolveLists(
                                   string escapeMarkerStart,
                                   string escapeMarkerEnd,
                                   string indexedRefStart,
                                   string indexedRefEnd,
                                   string nullString,
                                   string foreachKeyStart,
                                   string foreachKeyEnd,
                                   string foreachItemKey,
                                   Dictionary<string, object> objDict,
                                   string value)
    {
      string text = (string)value.Clone();
      string textBeforeLoop = string.Empty;
      string textAfterLoop = string.Empty;

      string markerStart = escapeMarkerStart + foreachKeyStart;
      string markerEnd = escapeMarkerStart + foreachKeyEnd + escapeMarkerEnd;
      while (text.Contains(markerStart))
      {
        if (text.IndexOf(markerEnd) < 0)
        {
          break;
        }
        int indxForeachStart = text.IndexOf(markerStart);
        int indxForeachStartCount = text.Substring(indxForeachStart).IndexOf(escapeMarkerEnd) + escapeMarkerEnd.Length;

        textBeforeLoop = text.Substring(0, indxForeachStart);

        string loopref = text.Substring(indxForeachStart, indxForeachStartCount);
        string loopObjName = loopref.Substring(markerStart.Length, loopref.IndexOf(escapeMarkerEnd) - markerStart.Length);
        int loopTextStartIndx = markerStart.Length + escapeMarkerEnd.Length + loopObjName.Length - 1;

        object loopObject = null;
        loopObject = GetObjectFromReference( 
                                               indexedRefStart,
                                               indexedRefEnd,
                                               objDict,
                                               loopObjName);
        if (loopObject == null)
        {
          loopObject = new ArrayList();
        }
        if (!typeof(ICollection).IsAssignableFrom(loopObject.GetType()))
        {
          throw new ApplicationException(string.Format(
                         "There is a problem resolving a reference to a collection named '{0}' in a text template",
                         loopObjName));
        }
        int indxStartingLoop = indxForeachStart + indxForeachStartCount;
        string toEnd = text.Substring(indxStartingLoop);
        string loopText = (string) toEnd.Clone();

        int innerLoopCount = 0;
        int indxMarkerStart = toEnd.IndexOf(markerStart);
        int indxMarkerEnd = toEnd.IndexOf(markerEnd);
        while (indxMarkerStart > 0 && indxMarkerStart < indxMarkerEnd)
        {
          innerLoopCount++;
          toEnd = toEnd.Substring(indxMarkerStart + escapeMarkerStart.Length + foreachKeyStart.Length);
          indxMarkerStart = toEnd.IndexOf(markerStart);
          indxMarkerEnd = toEnd.IndexOf(markerEnd);
        }
        toEnd = (string)loopText.Clone();
        indxMarkerEnd = toEnd.IndexOf(markerEnd) + markerEnd.Length;
        while (innerLoopCount > 0)
        {
          toEnd = toEnd.Substring(toEnd.IndexOf(markerEnd) + markerEnd.Length);
          indxMarkerEnd += toEnd.IndexOf(markerEnd) + markerEnd.Length;
          innerLoopCount--;
        }
        textAfterLoop = loopText.Substring(indxMarkerEnd);
        loopText = loopText.Substring(0, indxMarkerEnd - markerEnd.Length);
        List<string> loopItemTextList = new List<string>();
        foreach (object currentItem in (ICollection)loopObject)
        {
          Dictionary<string, object> loopItemList = new Dictionary<string,object>();
          loopItemList.Add(foreachItemKey, currentItem);
          string loopItemText = (string)loopText.Clone();
          //indxMarkerStart = loopText.IndexOf(markerStart);
          //while (indxMarkerStart > 0)
          //{
          //  loopItemText = ResolveLists(escapeMarkerStart,
          //                            escapeMarkerEnd,
          //                            indexedRefStart,
          //                            indexedRefEnd,
          //                            nullString,
          //                            foreachKeyStart,
          //                            foreachKeyEnd,
          //                            foreachItemKey,
          //                            loopItemList,
          //                            loopObject,
          //                            loopItemText);
          //  indxMarkerStart = loopItemText.IndexOf(markerStart);
          //}
          loopItemText = ParseObjectsIntoString(escapeMarkerStart,
                                              escapeMarkerEnd,
                                              indexedRefStart,
                                              indexedRefEnd,
                                              nullString,
                                              foreachKeyStart,
                                              foreachKeyEnd,
                                              foreachItemKey,
                                              loopItemList,
                                              loopItemText);
          loopItemTextList.Add(loopItemText);
        }
        loopText = string.Empty;
        foreach (string itemText in loopItemTextList)
        {
          loopText += itemText;
        }

        text = textBeforeLoop + Environment.NewLine + loopText + Environment.NewLine + textAfterLoop;
      }
      return text;

    }

  }
}
