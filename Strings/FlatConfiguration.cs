/* 
Header
 */

using System;
using System.Configuration;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;
using Sirona.Utilities.IO;

namespace Sirona.Utilities.Strings
{
  /// <summary>  
  /// Reads a flat configuration file. The object reads all configuration variables
  /// in the constructor and keeps them in memory for quick random access afterwards.
  /// TIP: 
  /// Setup a system or user environment variable i.e. APP_CONF that
  /// points to the config file so that you can use 
  /// </summary>
  /// <example>
  /// <code>
  /// FlatConfiguration conf = FlatConfiguration.Create("APP_CONF"); 
  /// </code>
  /// to get an instance of the configuration object.
  /// </example>
  /// <version>  	1.0 26 Jun 2003
  /// </version>
  /// <author>  	Miguel Curi
  /// *
  /// 
  /// </author>
  public class FlatConfiguration
  {
    /// <summary>
    /// The name of the variable that identifies the root directory where the framework 
    /// looks for configuration files. This is applicable to settings in the app.config
    /// </summary>
    public const string ROOT_VARIABLE_NAME = "FlatConfigurationFile";
    /// <summary>
    /// A holder of the name "app.config" for references within this file
    /// </summary>
    public const string APP_CONFIG_NAME = "app.config";

    private void InitBlock()
    {
      m_prop = new PropertyCollection();
    }
    private PropertyCollection m_prop;

    /// <summary> Create a new configuration.  Similar to the comparable constructor, but
    /// writes to stderr and returns null on error.
    /// </summary>
    /// <returns> a configuration object
    /// 
    /// </returns>
    public static FlatConfiguration Create()
    {
      return Create(APP_CONFIG_NAME);
    }

    /// <summary> Create a new configuration.  Similar to the comparable constructor, but
    /// writes to stderr and returns null on error.
    /// </summary>
    /// <returns> a configuration object
    /// 
    /// </returns>
    public static FlatConfiguration Create(String resource)
    {
      FlatConfiguration rv;
      try
      {
        rv = new FlatConfiguration(resource);
      }
      catch (Exception e)
      {
        //Console.WriteLine("Configuration error" + e.StackTrace);
        return null;
      }
      return rv;
    }
    public static FlatConfiguration CreateFromString(String content)
    {
        return CreateFromString(content, Encoding.ASCII);
    }
    public static FlatConfiguration CreateFromString(String content, Encoding encoding)
    {
        using (MemoryStream stream = new MemoryStream(encoding.GetBytes(content)))
        {
            return new FlatConfiguration(stream);
        }
    }
    public static FlatConfiguration CreateFromStream(Stream resourceStream)
    {
        return new FlatConfiguration(resourceStream);
    }

    /// <summary> Create a new configuration.
    /// </summary>
    private FlatConfiguration()
      : this(APP_CONFIG_NAME)
    {
    }

    /// <summary> Create a new configuration.
    /// </summary>
    /// <param name="resource">The name of the resource to retrieve values from.
    /// It can be the name of an environment variable or a file in the user's 
    /// directory or a fully qualified path in that order.
    /// </param>
    /// <exception cref="ApplicationException"> if initialization fails 
    /// </exception>
    private FlatConfiguration(String resource)
    {
        String filename = null;
        try
        {
            if (string.IsNullOrEmpty(resource) || resource.Equals(APP_CONFIG_NAME))
            {
                filename = System.Configuration.ConfigurationManager.AppSettings[ROOT_VARIABLE_NAME];
            }
            else
            {
                filename = System.Configuration.ConfigurationManager.AppSettings[resource];
            }

        }
        catch { }
        if (filename == null && !string.IsNullOrEmpty(resource))
        {
            filename = Environment.GetEnvironmentVariable(resource);
        }
        if (filename != null)
        {
            resource = filename;
        }
        //Make sure to use the right separator
        resource = FileUtility.VerifySeparator(resource);
        string postDriveSeq = ":" + Path.DirectorySeparatorChar.ToString();
        FileStream istream = null;
        if (resource.IndexOf(postDriveSeq) < 0)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Environment.GetEnvironmentVariable("HOMEDRIVE")).Append(Environment.GetEnvironmentVariable("HOMEPATH"));
                sb.Append(Path.DirectorySeparatorChar.ToString()).Append(resource);
                istream = new FileStream(sb.ToString(), FileMode.Open, FileAccess.Read);
            }
            catch
            {
                //Do nothing and try something else
            }
        }
        if (istream == null)
        {
            try
            {
                istream = new FileStream(resource, FileMode.Open, FileAccess.Read);
            }
            catch (IOException)
            {
                throw new ApplicationException("Error locating configuration resource " + resource);
            }
        }
        try
        {
            Init(istream);
        }
        finally
        {
            if (istream != null) istream.Close();
        }
    }
    private FlatConfiguration(Stream resourceStream)
    {
        Init(resourceStream);
    }
    private void Init(Stream resourceStream)
    {
      InitBlock();
      if (resourceStream != null)
      {
        try
        {
          StringBuilder sb = FileUtility.ReadFileAsStringBuffer(resourceStream);
          String content = StringUtility.Delete(sb.ToString(), "\r");
          StringTokenizer tokens = new StringTokenizer(content, "\n");
          if (tokens.CountTokens <= 0)
          {
            tokens = new StringTokenizer(content);
          }
          while (tokens.HasMoreTokens())
          {
            String currentProp = tokens.NextToken();
            if (currentProp.Trim().StartsWith("#"))
              continue;
            char splitter = '=';
            if (currentProp.IndexOf(splitter) == -1)
            {
              splitter = '\t';
            }
            if (currentProp.IndexOf(splitter) == -1)
            {
              splitter = ' ';
            }
            String[] nv = currentProp.Split(splitter);
            if (nv.Length > 0)
            {
              String name = nv[0].Trim();
              String valueP = "";
              if (nv.Length > 1)
              {
                for (int i = 1; i < nv.Length; i++)
                {
                  valueP += nv[i].Trim();
                  if (splitter.Equals(' '))
                  {
                    valueP += " ";
                  }
                }
                valueP = valueP.Trim();
              }
              m_prop.Add( name, valueP );
            }
          }

        }
        catch (IOException ioe)
        {
          throw new ApplicationException("Error reading configuration resource.", ioe);
        }
      }
    }

    /// <summary>Wrapper for <see cref="GetS(String,String)"/> and returns null if the value is not found 
    /// </summary>
    /// <param name="sName">Name of the property</param>
    /// <returns>The string value of the property sName in the config file</returns>
    public virtual String GetS(String sName)
    {
      return GetS(sName, null);
    }

    /// <summary>  Get a string configuration value. System properties will override values specified
    /// in the configuration file.
    /// </summary>
    /// <param name="sName">The name of the variable to retrieve.
    /// </param>
    /// <param name="sDefault">The default value to return if the given variable was not found.
    /// </param>
    /// <returns> The value associated with the given name, or <tt>sDefault</tt> if the
    /// value was not found. 
    /// </returns>
    public virtual String GetS(String sName, String sDefault)
    {
      String sValue;

      // System properties override configuration file properties
      sValue = Environment.GetEnvironmentVariable(sName);
      if (sValue != null)
        return sValue;

      // Return the configuration file property if it exists

      Object o = null;
      try
      {
        o = m_prop[sName];
      }
      catch { }
      if (o != null)
      {
        sValue = (String)o;
      }
      else if (sDefault != null)
      {
        sValue = sDefault;
      }
      while (StringUtility.NotEmpty(sValue) && sValue.IndexOf("${") >= 0)
      {
        int firstIndex = sValue.IndexOf("${");
        int lastIndex = sValue.IndexOf("}");
        String valStart = sValue.Substring(0, firstIndex);
        if (valStart == null) valStart = string.Empty;
        String valEnd = sValue.Substring(lastIndex + 1);
        if (valEnd == null) valEnd = string.Empty;
        String val = GetS(sValue.Substring(firstIndex + 2, (lastIndex - firstIndex - 2)));
        sValue = valStart + val + valEnd;
      }

      return sValue;
    }

    /// <summary>  Get an integer configuration value.
    /// </summary>
    /// <param name="sName">  The name of the variable in the conf file.
    /// </param>
    /// <returns> The value associated with the variable, or zero if
    /// not found.
    /// </returns>
    /// <seealso cref="Geti(string, int)"> 
    /// </seealso>
    public virtual int Geti(String sName)
    {
      return Geti(sName, 0);
    }

    /// <summary>  Get an integer configuration value.
    /// </summary>
    /// <param name="sName">The name of the variable in the conf file.
    /// </param>
    /// <param name="nDefault">The value to return if the variable is not found.
    /// </param>
    /// <returns> The value associated with the variable, or <tt>nDefault</tt>
    /// if not found.
    /// </returns>
    public virtual int Geti(String sName, int nDefault)
    {
      String sVal = GetS(sName);

      if (sVal == null)
        return nDefault;

      try
      {
        return Int32.Parse(sVal);
      }
      catch
      {
        return nDefault;
      }
    }

    /// <summary>  Get a boolean configuration value. A boolean <tt>true</tt> value
    /// may be represented as a non-zero integer (ie "1"), "yes", "y",
    /// "true", or "t".  A boolean <tt>false</tt> value may be represented
    /// as "0", "no", "n", "false", or "f".  The string comparisons
    /// are not case sensitive.
    /// </summary>
    /// <param name="sName">The name of the variable in the conf file.
    /// </param>
    /// <param name="bDefault">The value to return if the variable is not found.
    /// </param>
    /// <returns> The value associated with the variable, or <tt>bDefault</tt>
    /// if not found.
    /// </returns>
    public virtual bool GetBoolean(String sName, bool bDefault)
    {
      String sVal = GetS(sName);

      // Not found?
      if ((sVal == null) || (sVal.Length == 0))
        return bDefault;

      // English?
      if (sVal.ToUpper().Equals("yes".ToUpper()) || sVal.ToUpper().Equals("true".ToUpper()))
        return true;
      else if (sVal.ToUpper().Equals("y".ToUpper()) || sVal.ToUpper().Equals("t".ToUpper()))
        return true;
      else if (sVal.ToUpper().Equals("no".ToUpper()) || sVal.ToUpper().Equals("false".ToUpper()))
        return false;
      else if (sVal.ToUpper().Equals("n".ToUpper()) || sVal.ToUpper().Equals("f".ToUpper()))
        return false;

      // Number?
      try
      {
        return (Int32.Parse(sVal) != 0);
      }
      catch
      {
        // Whatever, man...
      }

      return bDefault;
    }


    /// <summary> Fetch a numeric double configuration value.
    /// </summary>
    /// <param name="name">The name of the variable in the conf file.
    /// </param>
    /// <returns> The value associated with the variable, or 0
    /// if not found.
    /// </returns>
    public virtual double Getd(String name)
    {
      String val = GetS(name);
      if (val == null)
        return 0;
      try
      {
        return (Double.Parse(val));
      }
      catch (System.FormatException)
      {
        return 0.0;
      }
    }

    /// <summary> Get names of all available configuration keys.
    /// </summary>
    /// <returns>An enumeration of the keys (variable names) found in the config file</returns>
    public virtual IEnumerator Elements()
    {
      return m_prop.Keys.GetEnumerator();
    }
    /// <summary>
    /// Accessor for all the properties.
    /// </summary>
    public PropertyCollection Properties
    {
      get
      {
        return m_prop;
      }
    }

    /// <summary> Get all names from Configuration file that begin with the specified String.
    /// </summary>
    /// <param name="startStr">Starting section of the name of the variable</param>
    /// <returns>A list of the variable names that start with <tt>startStr</tt></returns>
    public virtual ArrayList GetKeysStartingWith(String startStr)
    {
      ArrayList results = new ArrayList();
      String tmp;

      for (IEnumerator e = m_prop.Keys.GetEnumerator(); e.MoveNext(); )
      {
        if (((tmp = (String)e.Current)).StartsWith(startStr))
        {
          results.Add(tmp);
        }
      }

      return results;
    }


    /// <summary>  Get unique endings for Configuration files that start with the specified String.
    /// </summary>
    /// <param name="startStr">The start section of the names to look for</param>
    /// <returns>A list of the ending part of all names that start with <tt>startStr</tt></returns>
    public virtual ArrayList GetUniqueEndingsForKeysWith(String startStr)
    {
      ArrayList results = new ArrayList();
      String tmp;

      for (IEnumerator e = m_prop.Keys.GetEnumerator(); e.MoveNext(); )
      {
        if (((tmp = (String)e.Current)).StartsWith(startStr))
        {
          results.Add(tmp.Substring(startStr.Length));
        }
      }

      return results;
    }


    /// <summary>  Parses a list of string tokens from a variable in the conf file
    /// and returns them in a ArrayList.
    /// </summary>
    /// <param name="sVarName">    The name of the conf file variable whose value
    /// contains the tokens.
    /// </param>
    /// <param name="sDelimeters"> The delimeter characters that will be passed
    /// into the <see cref="StringTokenizer"/>  constructor.
    /// Pass null to use the default whitespace characters
    /// (see the <see cref="StringTokenizer"/> documentation for
    /// the complete list).
    /// </param>
    /// <param name="bLowercase">  Pass true to lowercase all tokens before storing
    /// in the set, false to leave case as-is.
    /// </param>
    /// <returns> A list containing the tokens found, or null if
    /// no tokens were found.
    /// 
    /// </returns>
    public virtual ArrayList GetTokens(String sVarName, String sDelimeters, bool bLowercase)
    {
      ArrayList hsTokens = null;
      String sTokens = GetS(sVarName);

      if (StringUtility.NotEmpty(sTokens))
      {
        // Build the string tokenizer
        StringTokenizer tokens;
        if (StringUtility.NotEmpty(sDelimeters))
          tokens = new StringTokenizer(sTokens, sDelimeters);
        else
          tokens = new StringTokenizer(sTokens);

        // Add the tokens to the set
        String sToken;
        while (tokens.HasMoreTokens())
        {
          sToken = tokens.NextToken();

          if (hsTokens == null)
            hsTokens = new ArrayList();

          if (bLowercase)
            sToken = sToken.ToLower();

          hsTokens.Add(sToken);
        }
      }

      return hsTokens;
    }

    /// <summary>  Produces a string listing all keys and their associated values.
    /// </summary>
    public override String ToString()
    {
      return ToString(false);
    }


    /// <summary>  Produces a string listing all keys and their associated values.
    /// </summary>
    /// <param name="bUseLineBreaks"> Pass <tt>true</tt> to produce a string
    /// where only one key is listed per line.
    /// Pass <tt>false</tt> to produce a string
    /// where all data is packed on a single line.
    /// </param>
    public virtual String ToString(bool bUseLineBreaks)
    {
      System.Text.StringBuilder sb = new System.Text.StringBuilder();
      String sKey, sValueProp, sValueSys;

      sb.Append(GetType().FullName);
      sb.Append('{');
      if (bUseLineBreaks)
        sb.Append("\r\n");

      for (IEnumerator e = m_prop.Keys.GetEnumerator(); e.MoveNext(); )
      {
        sKey = (String)e.Current;
        sValueProp = (String)m_prop[sKey];
        sValueSys = Environment.GetEnvironmentVariable(sKey);

        // When showing one key/value pair per line, indent the content
        // to visually emphasize that these are owned by a single Configuration
        if (bUseLineBreaks)
          sb.Append("   ");

        sb.Append(sKey);
        sb.Append("='");

        if (sValueSys != null)
        {
          sb.Append(sValueSys);
          sb.Append("' (system value overrides stored value of '");
          sb.Append(sValueProp);
          sb.Append("')");
        }
        else
        {
          sb.Append(sValueProp);
          sb.Append("'");
        }

        if (bUseLineBreaks)
          sb.Append(Environment.NewLine);
        else
          sb.Append("; ");
      }

      sb.Append('}');

      return sb.ToString();
    }
    /// <summary>  Produces a string listing all keys and their associated values in XML format.
    /// </summary>
    public virtual String ToXMLString()
    {
      System.Text.StringBuilder sb = new System.Text.StringBuilder();
      String sKey, sValueProp, sValueSys;

      sb.Append(GetType().FullName);
      sb.Append("{<br>");

      for (IEnumerator e = m_prop.Keys.GetEnumerator(); e.MoveNext(); )
      {
        sKey = (String)e.Current;
        sValueProp = (String)m_prop[sKey];
        sValueSys = Environment.GetEnvironmentVariable(sKey);

        // When showing one key/value pair per line, indent the content
        // to visually emphasize that these are owned by a single Configuration
        sb.Append("&nbsp;&nbsp;&nbsp;&nbsp;   ");

        sb.Append(sKey);
        sb.Append(" = '");

        if (sValueSys != null)
        {
          sb.Append(sValueSys);
          sb.Append("' (system value overrides stored value of '");
          sb.Append(sValueProp);
          sb.Append("')");
        }
        else
        {
          sb.Append(sValueProp);
          sb.Append("'");
        }
        sb.Append("<br>");
      }

      sb.Append("}<br>");

      return sb.ToString();
    }


    ///// <summary>
    ///// UNIT TESTING *
    ///// </summary>
    //[STAThread]
    //public static void Main(String[] asArgs)
    //{
    //  OctlConfiguration conf = null;
    //  if (asArgs.Length > 0)
    //    conf = OctlConfiguration.Create(asArgs[0]);
    //  else
    //    conf = OctlConfiguration.Create();
    //  Console.Out.WriteLine(conf.ToString(true));
    //  // Test tokens
    //  ArrayList hs = conf.GetTokens("AppletKeywords.Code", null, true);
    //  Console.Out.WriteLine("hs = " + hs);
    //  Console.Out.WriteLine(System.Environment.GetEnvironmentVariable("CLASSPATH"));
    //}
  }
}
