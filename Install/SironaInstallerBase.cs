using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Configuration.Install;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Sirona.Utilities.IO;
using Sirona.Utilities.XML;
using Sirona.Utilities.Strings;
using Sirona.Utilities.ExceptionHandling;

namespace Sirona.Utilities.Install
{
    public class SironaInstallerBase : Installer
    {

        #region Vars

        public const string PARAM_APP_CONFIG = "AppConfig";

        /// <summary></summary>
        public const string TAG_APP_CONFIG = "configuration";
        /// <summary></summary>
        public const string TAG_APP_CONFIG_SETTINGS = "appSettings";
        /// <summary></summary>
        public const string TAG_APP_CONFIG_SETTINGS_ADD = "add";
        /// <summary></summary>
        public const string TAG_APP_CONFIG_SETTINGS_ADD_KEY = "key";
        /// <summary></summary>
        public const string TAG_APP_CONFIG_SETTINGS_ADD_VALUE = "value";
        /// <summary></summary>
        public const string PARAM_ASSEMBLY_PATH = "assemblypath";
        /// <summary></summary>
        public const string PARAM_CONFIG_ASSEMBLY = "ConfigAssembly";

        protected string mAppConfigLocation = null;
        protected Assembly mConfigAssembly = Assembly.GetExecutingAssembly();

        #endregion

        #region Initialization
        /// <summary>
        /// Initializes the installer
        /// </summary>
        protected void Init()
        {
            InitLog();
        }
        protected void Init(string basePath)
        {
            this.InitLog(basePath);
        }
        /// <summary>
        /// Initializes the installation logs
        /// </summary>
        protected void InitLog()
        {
            this.InitLog(null);
        }
        protected void InitLog(string logFolder)
        {
            try
            {
                string logFile = "install.log";
                if (string.IsNullOrEmpty(logFolder))
                {
                    string assemblyPath = this.Context.Parameters[PARAM_ASSEMBLY_PATH];
                    if (assemblyPath != null)
                    {
                        FileInfo fi = new FileInfo(assemblyPath);
                        logFolder = fi.DirectoryName;
                    }
                }
                if (!string.IsNullOrEmpty(logFolder))
                {
                    if (!logFolder.EndsWith(FileUtility.SLASH_FORWARD))
                        logFolder = logFolder + FileUtility.SLASH_FORWARD;
                    if (!Directory.Exists(logFolder))
                        Directory.CreateDirectory(logFolder);
                    logFile = logFolder + logFile;
                }
                try
                {
                    if (Trace.Listeners[logFile] == null)
                    {
                        Trace.Listeners.Add(new TextWriterTraceListener(logFile, logFile));
                        Trace.AutoFlush = true;
                    }
                }
                catch (Exception e)
                {
                    throw new InstallException("Could not attach listener to log installation actions on logFile=" + logFile, e);
                }
            }
            catch (Exception e)
            {
                throw new InstallException("Could not initialize installer ", e);
            }

        }
        #endregion

        protected InstallConfigurationProperties GetHostInstallConfigurationPropertiesAndCleanUp()
        {
            return this.GetHostInstallConfigurationPropertiesAndCleanUp(null);
        }
        protected InstallConfigurationProperties GetHostInstallConfigurationPropertiesAndCleanUp(string setupDir)
        {
            InstallConfigurationProperties props = this.ReadHostnameSetupFile(setupDir);
            if (props != null)
            {
                this.CleanupHostSpecificSetupFiles(setupDir, props);
                return props;
            }
            this.WriteToTrace("No properties file found for host, returning null.");
            return null;
        }

        protected string GetHostSpecificInstallProperty(string propertyName)
        {
            InstallConfigurationProperties props = this.ReadHostnameSetupFile();
            if (props != null)
            {
                return props.GetPropertyValue(propertyName);
            }
            return null;
        }

        protected void CleanupHostSpecificSetupFiles(string setupDir)
        {
            InstallConfigurationProperties props = this.ReadHostnameSetupFile(setupDir);
            this.CleanupHostSpecificSetupFiles(setupDir, props);
        }

        protected void CleanupHostSpecificSetupFiles(string setupDir, InstallConfigurationProperties props)
        {
            if (props != null && props.RemoveSetupFilesAfterInstall)
            {
                if (string.IsNullOrEmpty(setupDir))
                {
                   setupDir = this.GetAppRootDir();
                }

                if (!string.IsNullOrEmpty(setupDir))
                {
                    this.WriteToTrace("Cleaning host config files from " + setupDir);
                    if (Directory.Exists(setupDir))
                    {
                        DirectoryInfo di = new DirectoryInfo(setupDir);
                        try
                        {
                            foreach (FileInfo fi in di.GetFiles())
                            {
                                if (fi.Name.EndsWith(".setup.config"))
                                {
                                    this.WriteToTrace("removing file " + fi.Name);
                                    fi.Delete();
                                }
                            }
                        }
                        catch (Exception exc)
                        {
                            this.WriteToTrace("Failed to remove some setup config files", exc);
                        }
                    }
                }

            }
        }

        protected InstallConfigurationProperties ReadHostnameSetupFile()
        {
            string appRootDir = this.GetAppRootDir();
            return this.ReadHostnameSetupFile(appRootDir);
        }
        protected InstallConfigurationProperties ReadHostnameSetupFile(string setupDir)
        {
            if (string.IsNullOrEmpty(setupDir))
            {
                setupDir = this.GetAppRootDir();
            }
            if (!string.IsNullOrEmpty(setupDir))
            {
                string hostname = System.Net.Dns.GetHostName();
                string hostConfigFile = setupDir + Path.DirectorySeparatorChar + hostname + ".setup.config";
                this.WriteToTrace("Attempting to read host config file " + hostConfigFile);
                if (File.Exists(hostConfigFile))
                {
                    try
                    {
                        return XmlXPathUtility.Deserialize<InstallConfigurationProperties>(XmlXPathUtility.Parse(hostConfigFile));
                    }
                    catch(Exception ex)
                    {
                        this.WriteToTrace("Error reading host config file", ex);
                    }
                }
            }
                
            return null;

        }
        /// <summary>
        /// Gets the value of the key identified with the given name in the given stream.
        /// If the stream cannot be parsed into xml a null is returned
        /// </summary>
        /// <param name="pSourceDocument"></param>
        /// <param name="pKeyName"></param>
        /// <returns></returns>
        protected string GetConfigKeyValue(XmlDocument pSourceDocument, string pKeyName)
        {
            if (pSourceDocument != null)
            {
                XmlNodeList nodes = pSourceDocument.GetElementsByTagName(TAG_APP_CONFIG_SETTINGS);
                foreach (XmlNode node in nodes)
                {
                    XmlNodeList keys = node.SelectNodes(TAG_APP_CONFIG_SETTINGS_ADD);
                    if (keys != null)
                    {
                        foreach (XmlNode key in keys)
                        {
                            string keyName = null;
                            string keyValue = null;
                            XmlAttribute attr = key.Attributes[TAG_APP_CONFIG_SETTINGS_ADD_KEY];
                            if (attr != null)
                            {
                                keyName = attr.Value;
                                if (keyName.Equals(pKeyName))
                                {
                                    XmlAttribute valAttr = key.Attributes[TAG_APP_CONFIG_SETTINGS_ADD_VALUE];
                                    if (valAttr != null)
                                    {
                                        keyValue = valAttr.Value;
                                        return keyValue;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// This method changes the value of a key defined in the add tags of the app.config file.
        /// for example :<br/><br/>
        /// &lt;configuration><br/>
        ///   &lt;appSettings><br/>
        ///     &lt;add key="KEYNAME" value="OLDVALUE"/><br/>
        ///   &lt;/appSettings><br/>
        /// &lt;/configuration><br/>
        /// <br/><br/>
        /// calling ChangeConfigValue(doc, "KEYNAME", "NEWVALUE");
        /// will result in doc mutated to :<br/><br/>
        /// &lt;configuration><br/>
        ///   &lt;appSettings><br/>
        ///     &lt;add key="KEYNAME" value="NEWVALUE"/><br/>
        ///   &lt;/appSettings><br/>
        /// &lt;/configuration><br/>
        /// 
        /// </summary>
        /// <param name="pSourceDocument">The source document that will be mutated</param>
        /// <param name="pKeyName">the name of the key in the add tag. If no key is found with the name, one is added</param>
        /// <param name="pKeyValue">The new value to set in the add tag.</param>
        protected void ChangeConfigKeyValue(XmlDocument pSourceDocument, string pKeyName, string pKeyValue)
        {
            if (pSourceDocument != null)
            {

                XmlNodeList nodes = pSourceDocument.GetElementsByTagName(TAG_APP_CONFIG_SETTINGS);
                //Remove nodes with the given key name from appSettings
                if (nodes != null)
                {
                    foreach (XmlNode node in nodes)
                    {
                        XmlNodeList keys = node.SelectNodes(TAG_APP_CONFIG_SETTINGS_ADD);
                        List<XmlNode> toAdd = new List<XmlNode>();
                        if (keys != null)
                        {
                            foreach (XmlNode key in keys)
                            {
                                string keyName = null;
                                XmlAttribute attr = key.Attributes[TAG_APP_CONFIG_SETTINGS_ADD_KEY];
                                if (attr != null)
                                {
                                    keyName = attr.Value;
                                    if (!keyName.Equals(pKeyName))
                                    {
                                        toAdd.Add(key);
                                    }
                                }
                                else
                                {
                                    toAdd.Add(key);
                                }
                                node.RemoveChild(key);
                            }
                        }
                        foreach (XmlNode taNode in toAdd)
                        {
                            node.AppendChild(taNode);
                        }
                    }
                }
                //Add a proper add node to appSettings with the given keyName and keyValue
                XmlNode appConfigNode = pSourceDocument.SelectSingleNode(TAG_APP_CONFIG);
                if (appConfigNode == null)
                {
                    appConfigNode = pSourceDocument.CreateElement(TAG_APP_CONFIG);
                    pSourceDocument.AppendChild(appConfigNode);
                }
                XmlNode appSettingsNode = appConfigNode.SelectSingleNode(TAG_APP_CONFIG_SETTINGS);
                if (appSettingsNode == null)
                {
                    appSettingsNode = pSourceDocument.CreateElement(TAG_APP_CONFIG_SETTINGS);
                    appConfigNode.AppendChild(appSettingsNode);
                }
                XmlNode addNode = pSourceDocument.CreateElement(TAG_APP_CONFIG_SETTINGS_ADD);
                XmlAttribute keyAttr = pSourceDocument.CreateAttribute(TAG_APP_CONFIG_SETTINGS_ADD_KEY);
                keyAttr.Value = pKeyName;
                addNode.Attributes.Append(keyAttr);
                XmlAttribute valueAttr = pSourceDocument.CreateAttribute(TAG_APP_CONFIG_SETTINGS_ADD_VALUE);
                valueAttr.Value = pKeyValue;
                addNode.Attributes.Append(valueAttr);
                appSettingsNode.AppendChild(addNode);
            }
        }
        /// <summary>
        /// Changes all references to an old directory path with a new one
        /// </summary>
        /// <param name="pSourceDocument">The document</param>
        /// <param name="pOldPath">The old path</param>
        /// <param name="pNewPath">The new path</param>
        protected void ChangeConfigPathAppConfig(XmlDocument pSourceDocument, string pOldPath, string pNewPath)
        {
            if (pSourceDocument != null)
            {

                XmlNodeList nodes = pSourceDocument.GetElementsByTagName(TAG_APP_CONFIG_SETTINGS);
                //Remove nodes with the given key name from appSettings
                if (nodes != null)
                {
                    foreach (XmlNode node in nodes)
                    {
                        XmlNodeList keys = node.SelectNodes(TAG_APP_CONFIG_SETTINGS_ADD);
                        if (keys != null)
                        {
                            foreach (XmlNode key in keys)
                            {
                                XmlAttribute attr = key.Attributes[TAG_APP_CONFIG_SETTINGS_ADD_VALUE];
                                if (attr != null)
                                {
                                    attr.Value = this.ReplaceIgnoreCase(attr.Value, pOldPath, pNewPath);
                                }
                            }
                        }
                    }
                }
            }
        }

        protected string GetAppRootDir()
        {
            string appConfig = this.GetAppConfigPath();
            if (string.IsNullOrEmpty(appConfig)) return null;
            FileInfo fi = new FileInfo(appConfig);
            return fi.DirectoryName;
        }

        /// <summary>
        /// Looks for App.config, [assemblyName].exe.config or Web.config in that order
        /// and returns the path to the first file found or null if none found.
        /// If the "AppConfig" settings is passed to the custom intall action, this method returns that value instead of
        /// looking for the right file.
        /// </summary>
        /// <returns></returns>
        protected string GetAppConfigPath()
        {
            if (this.AppConfigLocation != null)
            {
                return this.AppConfigLocation;
            }
            Assembly assem = this.ConfigAssembly;
            //Testing assembly contents
            //        string[] rNames = assem.GetManifestResourceNames();
            //        Trace.WriteLine("Resources:");
            //        foreach(string name in rNames)
            //        {
            //          Trace.WriteLine(name);
            //        }
            // This is for embedded files
            //          string appConfigResourceName = assem.GetName().Name + ".App.config";
            //          WriteToTrace(Environment.NewLine + "Searching for the App.config resource ... " + appConfigResourceName);
            //          Stream asstream = assem.GetManifestResourceStream(appConfigResourceName);
            string appConfigResourceDir = this.TargetDirectory;
            if (!appConfigResourceDir.EndsWith(FileUtility.SLASH_FORWARD))
            {
                appConfigResourceDir += FileUtility.SLASH_FORWARD;
            }
            string appConfigResourceName = appConfigResourceDir + "App.config";
            WriteToTrace(Environment.NewLine + "Searching for the App.config resource ... " + appConfigResourceName);

            if (!File.Exists(appConfigResourceName))
            {
                appConfigResourceName = appConfigResourceDir + assem.GetName().Name + ".exe.config";
                WriteToTrace("Couldn't find App.config, searching for ... " + appConfigResourceName);
                if (!File.Exists(appConfigResourceName))
                {
                    WriteToTrace("Couldn't find " + appConfigResourceName + ".");
                    WriteToTrace("NOTE: If this is a windows or console application, make sure to deploy the App.Config file");
                    appConfigResourceName = appConfigResourceDir + @"..\Web.config";
                    WriteToTrace("Searching for ... " + appConfigResourceName);
                    if (!File.Exists(appConfigResourceName))
                    {
                        WriteToTrace("NOTE: Web.config resource not found ... if this is a asp.net or web service application, make sure to deploy your Web.Config file");
                    }
                    else
                    {
                        WriteToTrace("Found " + appConfigResourceName + ", parsing ... ");
                    }
                }
                else
                {
                    WriteToTrace("Found " + appConfigResourceName + ", parsing ... ");
                }
            }
            else
            {
                WriteToTrace("Found " + appConfigResourceName + ", parsing ... ");
            }
            if (File.Exists(appConfigResourceName))
            {
                this.AppConfigLocation = appConfigResourceName;
            }
            return appConfigResourceName;

        }

        /// <summary>
        /// Determine if a string contains another ignoring case
        /// </summary>
        /// <param name="orig">the original string</param>
        /// <param name="s1">the string to look for</param>
        /// <returns></returns>
        public bool ContainsIgnoreCase(string orig, string s1)
        {
            if (orig == null || s1 == null)
            {
                return false;
            }
            return orig.ToLower().IndexOf(s1.ToLower()) >= 0;
        }
        /// <summary>
        /// Replaces a string ignoring case
        /// </summary>
        /// <param name="orig">The original string</param>
        /// <param name="s1">the string to search for</param>
        /// <param name="s2">the string to replcae all</param>
        /// <returns></returns>
        public string ReplaceIgnoreCase(string orig, string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) || !ContainsIgnoreCase(orig, s1))
            {
                return orig;
            }
            if (s2 == null)
            {
                s2 = string.Empty;
            }
            int indx = orig.ToLower().IndexOf(s1.ToLower()); ;
            while (indx >= 0)
            {
                string sub = orig.Substring(0, indx) + s2;
                int startEnd = indx + s1.Length;
                if (startEnd < orig.Length)
                {
                    sub += orig.Substring(startEnd);
                }
                orig = sub.ToString();
                startEnd = indx + s2.Length;
                sub = orig.Substring(startEnd);
                indx = sub.ToLower().IndexOf(s1.ToLower());
                if (indx >= 0)
                {
                    indx += startEnd;
                }
            }
            return orig;
        }

        /// <summary>
        /// Application configuration location.
        /// If the "AppConfig" setting is passed to the custom action, this property returns this value.
        /// </summary>
        protected string AppConfigLocation
        {
            get
            {
                string appConfig = this.Context.Parameters[PARAM_APP_CONFIG];
                if (string.IsNullOrEmpty(appConfig))
                {
                    if (mAppConfigLocation != null)
                    {
                        appConfig = mAppConfigLocation;
                    }
                    else
                    {
                        appConfig = null;
                    }
                }
                return appConfig;

            }
            set
            {
                mAppConfigLocation = value;
            }
        }
        /// <summary>
        /// Sets the config assembly.
        /// Gets the config assembly from <see cref="PARAM_CONFIG_ASSEMBLY"/> (NamingConfigAssembly) 
        /// parameter passed in from the CustomActionData or
        /// a customized class that sets the <see cref="ConfigAssembly"/> property or the 
        /// assembly of this class.
        /// </summary>
        public Assembly ConfigAssembly
        {
            get
            {
                if (mConfigAssembly == null)
                {
                    if (!string.IsNullOrEmpty(this.Context.Parameters[PARAM_CONFIG_ASSEMBLY]))
                    {
                        string mamcelConfigAssembly = this.Context.Parameters[PARAM_CONFIG_ASSEMBLY];
                        try
                        {
                            Assembly lAsm = Assembly.Load(mamcelConfigAssembly);
                            if (lAsm != null)
                            {
                                mConfigAssembly = lAsm;
                            }
                        }
                        catch
                        {
                            WriteToTrace(Environment.NewLine + "WARN: Passed assembly " + mamcelConfigAssembly + " could not be loaded, ignoring ....");
                        }
                    }
                }
                return mConfigAssembly;
            }
            set { mConfigAssembly = value; }
        }
        /// <summary>
        /// Gets the target directory from the <see cref="PARAM_ASSEMBLY_PATH"/> 
        /// (assemblypath) parameter
        /// </summary>
        protected string TargetDirectory
        {
            get
            {
                string assemblyPath = this.Context.Parameters[PARAM_ASSEMBLY_PATH];
                if (!string.IsNullOrEmpty(assemblyPath))
                {
                    FileInfo fi = new FileInfo(assemblyPath);
                    assemblyPath = fi.DirectoryName;
                }
                return assemblyPath;
            }
        }

        #region Tracing or logging methods
        /// <summary>
        /// Writes the list of parameters passed in to the installer in the form name=value
        /// </summary>
        protected void WriteParamList()
        {
            try
            {
                Trace.WriteLine("Installation Parameters : ");
                foreach (string name in this.Context.Parameters.Keys)
                {
                    Trace.WriteLine("    " + name + " = " + Context.Parameters[name]);
                }
                Trace.WriteLine("");
            }
            catch { }
        }
        /// <summary>
        /// Writes a custom message to the log, catching and ignoring exceptions so
        /// if you don't see the message is because the Trace was unsuccesful writing.
        /// </summary>
        /// <param name="message"></param>
        protected void WriteToTrace(string message)
        {
            try
            {
                Trace.WriteLine(message);
            }
            catch { }
        }
        protected void WriteToTrace(string message, Exception exception)
        {
            try
            {
                Trace.WriteLine(message);
                Trace.WriteLine(ExceptionUtility.GetExceptionMessagesWithStackTrace(exception));
            }
            catch { }
        }
        /// <summary>
        /// Writes a header to the install.log file
        /// </summary>
        /// <param name="pAppName"></param>
        protected void WriteHeader(string pAppName)
        {
            try
            {
                Trace.WriteLine(Environment.NewLine + "********************************************************************************");
                if (pAppName != null)
                {
                    Trace.WriteLine(pAppName);
                }
                Trace.WriteLine("Installation log as of " + DateTime.Now.ToLongDateString() + " : " + DateTime.Now.ToLongTimeString());
                Trace.WriteLine("--------------------------------------------------------------------------------");
            }
            catch { }
        }
        #endregion



    }
}
