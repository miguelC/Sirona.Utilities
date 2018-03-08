using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Sirona.Utilities.Strings;

namespace Sirona.Utilities.IO
{
    public class FileUtility
    {
        private class AnonymousClassLineReader : LineReader
        {
            public AnonymousClassLineReader(StringBuilder sb)
            {
                InitBlock(sb);
            }
            private void InitBlock(StringBuilder sb)
            {
                this.sb = sb;
            }
            private StringBuilder sb;
            public virtual bool OnRead(String s)
            {
                // Separate this line from previous lines
                if (sb.Length > 0)
                    sb.Append(Environment.NewLine);

                // Add this line
                sb.Append(s);

                return true;
            }
        }
        public const string SLASH_FORWARD = @"\";
        public const string SLASH_BACK = @"/";
        /// <summary>
        /// 
        /// </summary>
        public const int DIR = 0;
        /// <summary>
        /// Variable to define files
        /// </summary>
        public const int FILE = 1;
        /// <summary>
        /// Heading for filenames that refer to embedded resources
        /// </summary>
        public const string EMBEDDED_RESOURCE = "embedded:";

        /// <summary>  Strips the last component off of a fully qualified filename.
        /// For example:
        /// <pre>
        /// trimFilename(null) == null
        /// trimFilename("a:\") == "a:\"
        /// trimFilename("\") == "\"
        /// trimFilename("\one\two\three") == "\one\two\"
        /// </pre>
        /// </summary>
        /// <param name="sFilename">  the absolute filename, which may contain
        /// directory information, a drive name, etc.
        /// </param>
        /// <returns> null if sFilename is null.  Otherwise, returns sFilename
        /// without the rightmost file component, but including the
        /// trailing slash.
        /// 
        /// </returns>
        public static string TrimFilename(string sFilename)
        {
            if (!string.IsNullOrEmpty(sFilename))
            {
                VerifySeparator(sFilename);
                FileInfo fi = new FileInfo(sFilename);
                return fi.DirectoryName;
            }

            return sFilename;
        }

        /// <summary>  Produces a fully-qualified filename for a partial filename.
        /// </summary>
        /// <param name="sFilename">The existing filename.
        /// </param>
        /// <returns> The fully-qualified filename corresponding to <tt>sFilename</tt>,
        /// or <tt>null</tt> if <tt>sFilename</tt> is invalid.
        /// 
        /// </returns>
        public static String FullyQualify(String sFilename)
        {
            String sFull = null;

            if (sFilename != null)
            {
                FileInfo file = new FileInfo(sFilename);
                sFull = file.FullName;
            }

            return sFull;
        }
        /// <summary>
        /// Resolves a local path to a folder named Resources under the application's base directory
        /// </summary>
        /// <param name="sFilename"></param>
        /// <returns></returns>
        public static FileInfo ResolveApplicationResourcesPathInfo(String sFilename)
        {

            string appRoot = AppDomain.CurrentDomain.BaseDirectory;
            if (!appRoot.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                appRoot += Path.DirectorySeparatorChar.ToString();
            }
            appRoot += "Resources" + Path.DirectorySeparatorChar.ToString();
            sFilename = appRoot + sFilename;

            return new FileInfo(sFilename);
        }
        public static String ResolveApplicationResourcesPath(String sFilename)
        {
            FileInfo fi = ResolveApplicationResourcesPathInfo(sFilename);
            return fi.FullName;
        }
        /// <summary>Renames a file.
        /// </summary>
        /// <param name="sFilenameCurrent">the existing filename.
        /// </param>
        /// <param name="sFilenameNew">the new filename.
        /// </param>
        /// <returns> true if successful, false upon error.
        /// 
        /// </returns>
        public static bool Rename(String sFilenameCurrent, String sFilenameNew)
        {
            bool bSuccess = true;

            if (!string.IsNullOrEmpty(sFilenameCurrent) && !string.IsNullOrEmpty(sFilenameNew))
            {
                // Make File objects for the old and new filenames
                FileInfo fileCurrent = new FileInfo(sFilenameCurrent);
                FileInfo fileNew = new FileInfo(sFilenameNew);

                // Rename the current to the new!
                fileCurrent.MoveTo(fileNew.FullName);
            }

            return bSuccess;
        }

        /// <summary>  Deletes a file.
        /// </summary>
        /// <param name="sFilename">  the name of the file to delete.
        /// </param>
        /// <returns> true if successful, false upon error.  Even if no exception
        /// is caught, this attempt can fail if there are streams still
        /// open on the given file.
        /// 
        /// </returns>
        public static bool Delete(String sFilename)
        {
            bool bSuccess = false;

            if (!string.IsNullOrEmpty(sFilename))
            {
                FileInfo file = new FileInfo(sFilename);
                bool tmpBool;
                if (File.Exists(file.FullName))
                {
                    File.Delete(file.FullName);
                    tmpBool = true;
                }
                else if (Directory.Exists(file.FullName))
                {
                    Directory.Delete(file.FullName);
                    tmpBool = true;
                }
                else
                {
                    tmpBool = false;
                }
                bSuccess = tmpBool;
            }

            return bSuccess;
        }

        /// <summary>  Finds the last-modified timestamp for a given file.
        /// </summary>
        /// <param name="sFilename">   The name of the file to examine.
        /// </param>
        /// <returns> The time the file was last modified, or null if the file was
        /// not found.
        /// 
        /// </returns>
        public static DateTime GetTimestamp(String sFilename)
        {
            DateTime dateTimestamp = DateTime.Now;

            if (!string.IsNullOrEmpty(sFilename))
            {
                FileInfo file = new FileInfo(sFilename);
                dateTimestamp = new DateTime(file.LastWriteTime.Ticks * 10000 + 621355968000000000);
            }

            return dateTimestamp;
        }

        /// <summary>Convenient wrapper for {@link #readFile(InputStream,FileUtility.LineReader)}.
        /// </summary>
        /// <param name="reader">The line reader to use, an implementation of FileUtility.LineReader</param>
        /// <param name="sFilename">The fully qualified name of the file to read</param>
        public static void ReadFile(String sFilename, LineReader reader)
        {
            ReadFile(new FileStream(new FileInfo(sFilename).FullName, FileMode.Open, FileAccess.Read), reader);
        }

        /// <summary>  Reads a file send each line to a <tt>FileUtility.LineReader</tt>
        /// object.
        /// </summary>
        /// <param name="streamIn">The stream to read.
        /// </param>
        /// <param name="reader">The <tt>FileUtility.LineReader</tt> implementation
        /// that will process each line of the file.
        /// 
        /// </param>
        public static void ReadFile(Stream streamIn, LineReader reader)
        {
            if ((streamIn != null) && (reader != null))
            {
                StreamReader readerStream = null;
                StringBuilder sb = new StringBuilder();

                try
                {
                    // Construct the buffered reader
                    readerStream = new StreamReader(streamIn);

                    // Read all the lines
                    String sLine;

                    while (true)
                    {
                        sLine = readerStream.ReadLine();
                        if (sLine == null)
                            break;
                        else if (!reader.OnRead(sLine))
                            break;
                    }
                }
                finally
                {
                    // Clean up
                    try
                    {
                        readerStream.Close();
                    }
                    catch { }
                }
            }
        }
        /// <summary> Method to read a file into a FileInputStream
        /// that points to the named file in the directory specified by the 
        /// the HOMEPATH system variable
        /// </summary>
        /// <param name="resource">The non fully qualified name of the file
        /// </param>
        /// <returns> The handle to the file named resource in the current user's directory
        /// directory 
        /// </returns>
        public static FileStream ReadUserFile(String resource)
        {
            FileStream r_fis = null;
            FileUtility fu = new FileUtility();
            StringBuilder sb = new StringBuilder();
            sb.Append(Environment.GetEnvironmentVariable("HOMEDRIVE")).Append(Environment.GetEnvironmentVariable("HOMEPATH"));
            String confDir = sb.ToString();
            if (confDir != null)
            {
                if (!confDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    confDir = confDir + Path.DirectorySeparatorChar.ToString();
                }
                String pathtofile = confDir + resource;
                FileInfo resfile = new FileInfo(pathtofile);
                r_fis = new FileStream(resfile.FullName, FileMode.Open, FileAccess.Read);

            }
            return r_fis;
        }

        /// <summary>  Reads an entire text file and stores it in a <tt>StringBuffer</tt>.
        /// </summary>
        /// <param name="streamIn">   the stream to read.
        /// </param>
        /// <returns> a <tt>StringBuffer</tt> containing the entire file.
        /// </returns>
        public static StringBuilder ReadFileAsStringBuffer(Stream streamIn)
        {
            StringBuilder sb = new StringBuilder();

            ReadFile(streamIn, new AnonymousClassLineReader(sb));

            return sb;
        }

        /// <summary>  Reads a file and stores it in a <tt>StringBuilder</tt>.
        /// </summary>
        /// <param name="sFilename">  the name of the file to read.
        /// </param>
        /// <returns> a <tt>StringBuilder</tt> containing the entire file.
        /// </returns>
        public static StringBuilder ReadFileAsStringBuffer(String sFilename)
        {
            StringBuilder sb = null;
            FileStream fs = null;
            try
            {
                fs = new FileStream(new FileInfo(sFilename).FullName, FileMode.Open, FileAccess.Read);
                sb = ReadFileAsStringBuffer(fs);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }

            return sb;
        }

        /// <summary>  Reads an entire file and stores it in a <tt>sbyte</tt> array.
        /// </summary>
        /// <param name="sFilename">  The name of the file to read.
        /// </param>
        /// <returns> a <tt>sbyte</tt> array containing the entire file, or null
        /// upon error.
        /// </returns>
        public static sbyte[] ReadFileAsBytes(String sFilename)
        {
            sbyte[] abFile = null;
            FileStream in_Renamed = null;

            try
            {
                FileInfo f = new FileInfo(sFilename);
                int nLength = (int)f.Length;

                if (nLength >= 0)
                {
                    abFile = new sbyte[nLength];
                    if (nLength > 0)
                    {
                        in_Renamed = new FileStream(f.FullName, FileMode.Open, FileAccess.Read);
                        ReadInput(in_Renamed, ref abFile, 0, abFile.Length);
                    }
                }
            }
            finally
            {
                try
                {
                    in_Renamed.Close();
                }
                catch { }
            }

            return abFile;
        }

        /// <summary>  Reads a single serialized object from a file.
        /// </summary>
        /// <param name="file">The file to read.
        /// </param>
        /// <returns> The serialized object, or null upon error or if the
        /// <tt>file</tt> param is null, or if the file is empty.
        /// 
        /// </returns>
        public static Object ReadFileAsObject(FileInfo file)
        {
            Object object_Renamed = null;

            if (file != null)
            {
                BinaryReader ois = null;
                try
                {
                    FileStream fis = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
                    ois = new BinaryReader(fis);

                    object_Renamed = Deserialize(ois);
                }
                finally
                {
                    if (ois != null)
                    {
                        ois.Close();
                    }
                }
            }

            return object_Renamed;
        }
        /// <summary> Overwrites a file or creates a new file if none is found
        /// </summary>
        /// <param name="filename">The name of the file (fully qualified)
        /// </param>
        /// <param name="content">The byte array of data to write to the file
        /// </param>
        public static void OverwriteFile(String filename, byte[] content)
        {
            CreateDirs(filename);
            FileStream fos = null;
            try
            {
                fos = new FileStream(filename, FileMode.Create);
                fos.Write(content, 0, content.Length);
            }
            finally
            {
                if (fos != null)
                {
                    try
                    {
                        fos.Close();
                    }
                    catch { }
                }
            }
        }

        /// <summary> Overwrites a file or creates a new file if none is found
        /// </summary>
        /// <param name="filename">The name of the file (fully qualified)
        /// </param>
        /// <param name="content">The object to write to the file (Note that the object
        /// must implement serializable to be able to write and read correctly)
        /// 
        /// </param>
        public static void OverwriteFile(String filename, Object content)
        {
            CreateDirs(filename);
            FileStream fos = null;
            BinaryWriter oos = null;
            try
            {
                fos = new FileStream(filename, FileMode.Create);
                oos = new BinaryWriter(fos);
                Serialize(oos, content);
            }
            finally
            {
                try
                {
                    if (fos != null)
                        fos.Close();
                    if (oos != null)
                        oos.Close();
                }
                catch { }
            }
        }

        /// <summary> Deletes all files and directories in a given path
        /// </summary>
        /// <param name="dirname">The name of the directory (fully qualified)
        /// 
        /// </param>
        public static void DeleteDir(String dirname)
        {
            FileInfo srcDir = new FileInfo(dirname);
            Hashtable filesToDel = ListDir(dirname, FileUtility.FILE);
            IEnumerator fEnum = filesToDel.GetEnumerator();
            while (fEnum.MoveNext())
            {
                FileInfo cFile = (FileInfo)fEnum.Current;
                Delete(cFile.FullName);
            }
            fEnum = null;
            filesToDel.Clear();
            filesToDel = ListDir(dirname, FileUtility.DIR);
            fEnum = filesToDel.GetEnumerator();
            while (fEnum.MoveNext())
            {
                FileInfo cFile = (FileInfo)fEnum.Current;
                DeleteDir(cFile.FullName);
            }
            bool tmpBool;
            if (File.Exists(srcDir.FullName))
            {
                File.Delete(srcDir.FullName);
                tmpBool = true;
            }
            else if (Directory.Exists(srcDir.FullName))
            {
                Directory.Delete(srcDir.FullName);
                tmpBool = true;
            }
            else
            {
                tmpBool = false;
            }
            bool generatedAux = tmpBool;
        }
        /// <summary> Moves all files and directories in a given path to a different directory
        /// </summary>
        /// <param name="dirname">The name of the directory (fully qualified)
        /// </param>
        /// <param name="destination">The path to the destination directory
        /// 
        /// </param>
        public static void MoveDir(String dirname, String destination)
        {
            FileInfo srcDir = new FileInfo(dirname);
            Hashtable filesToMove = ListDir(dirname, FileUtility.FILE);
            IEnumerator fEnum = filesToMove.GetEnumerator();
            while (fEnum.MoveNext())
            {
                FileInfo cFile = (FileInfo)fEnum.Current;
                File.Move(cFile.FullName, destination);
                File.SetCreationTime(destination, DateTime.Now);
            }
            fEnum = null;
            filesToMove.Clear();
            filesToMove = ListDir(dirname, FileUtility.DIR);
            fEnum = filesToMove.GetEnumerator();
            while (fEnum.MoveNext())
            {
                FileInfo cFile = (FileInfo)fEnum.Current;
                destination = destination + Path.DirectorySeparatorChar.ToString() + cFile.Name;
                MoveDir(cFile.FullName, destination);
            }
            bool tmpBool;
            if (File.Exists(srcDir.FullName))
            {
                File.Delete(srcDir.FullName);
                tmpBool = true;
            }
            else if (Directory.Exists(srcDir.FullName))
            {
                Directory.Delete(srcDir.FullName);
                tmpBool = true;
            }
            else
            {
                tmpBool = false;
            }
            bool generatedAux2 = tmpBool;
        }

        /// <summary> Copies all files and directories in a given path to a different directory
        /// </summary>
        /// <param name="dirname">The name of the directory (fully qualified)
        /// </param>
        /// <param name="destination">The path to the destination directory
        /// 
        /// </param>
        public static void CopyDir(String dirname, String destination)
        {
            Hashtable filesToMove = ListDir(dirname, FileUtility.FILE);
            IEnumerator fEnum = filesToMove.GetEnumerator();
            while (fEnum.MoveNext())
            {
                FileInfo cFile = (FileInfo)fEnum.Current;
                File.Copy(cFile.FullName, destination);
            }
            fEnum = null;
            filesToMove.Clear();
            filesToMove = ListDir(dirname, FileUtility.DIR);
            fEnum = filesToMove.GetEnumerator();
            while (fEnum.MoveNext())
            {
                FileInfo cFile = (FileInfo)fEnum.Current;
                destination = destination + Path.DirectorySeparatorChar.ToString() + cFile.Name;
                CopyDir(cFile.FullName, destination);
            }
        }

        /// <summary> Moves a file to a different directory
        /// </summary>
        /// <param name="filename">The name of the file (fully qualified)
        /// </param>
        /// <param name="destination">The path to the destination directory
        /// 
        /// </param>
        public static void MoveTextFile(String filename, String destination)
        {
            CopyTextFile(filename, destination);
            Delete(filename);
        }

        /// <summary> Moves a file to a different directory
        /// </summary>
        /// <param name="filename">The name of the file (fully qualified)
        /// </param>
        /// <param name="destination">The path to the destination directory
        /// 
        /// </param>
        public static void MoveBinaryFile(String filename, String destination)
        {
            CopyBinaryFile(filename, destination);
            Delete(filename);
        }

        /// <summary> Copies a file to a different directory
        /// </summary>
        /// <param name="filename">The name of the file (fully qualified)
        /// </param>
        /// <param name="destination">The path to the destination directory
        /// 
        /// </param>
        public static void CopyBinaryFile(String filename, String destination)
        {
            if (filename == null || destination == null)
            {
                return;
            }
            FileInfo srcFile = new FileInfo(filename);
            bool tmpBool;
            if (File.Exists(srcFile.FullName))
                tmpBool = true;
            else
                tmpBool = Directory.Exists(srcFile.FullName);
            if (!tmpBool)
            {
                return;
            }
            //String destFileName = destination + Path.DirectorySeparatorChar.ToString() + srcFile.Name;
            sbyte[] srcContent = ReadFileAsBytes(filename);
            //OverwriteFile(destFileName, srcContent);
            OverwriteFile(destination, srcContent);
        }

        /// <summary> Copies a file to a different directory
        /// </summary>
        /// <param name="filename">The name of the file (fully qualified)
        /// </param>
        /// <param name="destination">The path to the destination directory
        /// 
        /// </param>
        public static void CopyTextFile(String filename, String destination)
        {
            if (filename == null || destination == null)
            {
                return;
            }
            FileInfo srcFile = new FileInfo(filename);
            bool tmpBool;
            if (File.Exists(srcFile.FullName))
            {
                tmpBool = true;
            }
            else
            {
                tmpBool = Directory.Exists(srcFile.FullName);
            }
            if (!tmpBool)
            {
                return;
            }
            StringBuilder srcContent = ReadFileAsStringBuffer(filename);
            OverwriteFile(destination, srcContent);
        }

        /// <summary> Determine if a file exists with he given fully qualified name
        /// </summary>
        /// <param name="filename">The name of the file (fully qualified)
        /// </param>
        /// <returns> true if the file exists
        /// 
        /// </returns>
        public static bool FileExists(String filename)
        {
            FileInfo file = new FileInfo(filename);
            bool tmpBool;
            if (File.Exists(file.FullName))
            {
                tmpBool = true;
            }
            else
            {
                tmpBool = Directory.Exists(file.FullName);
            }
            return tmpBool;
        }
        /// <summary> Create a series of directories if they don't exists already in the
        /// filesystem
        /// </summary>
        /// <param name="filename">The name of the file (fully qualified)
        /// </param>
        /// <param name="isFile">true if the filename contains the name of a file, in which
        /// case the directories will be created up to one level before the file name.
        /// 
        /// </param>
        public static void CreateDirs(String filename, bool isFile)
        {
            if (filename == null)
                return;
            filename = VerifySeparator(filename);

            FileInfo file = new FileInfo(filename);
            bool tmpBool;
            if (File.Exists(file.FullName))
                tmpBool = true;
            else
                tmpBool = Directory.Exists(file.FullName);
            if (tmpBool)
                return;
            String sep = Path.DirectorySeparatorChar.ToString();
            String[] dirs = filename.Split(sep.ToCharArray());
            StringBuilder currDir = new StringBuilder();
            int lng = dirs.Length;
            if (isFile)
                lng--;
            for (int i = 0; i < lng; i++)
            {
                currDir.Append(dirs[i]);
                FileInfo dir = new FileInfo(currDir.ToString());
                bool tmpBool2;
                if (File.Exists(dir.FullName))
                    tmpBool2 = true;
                else
                    tmpBool2 = Directory.Exists(dir.FullName);
                if (!tmpBool2)
                {
                    Directory.CreateDirectory(dir.FullName);
                }
                currDir.Append(Path.DirectorySeparatorChar.ToString());
            }
        }
        /// <summary> Create a series of directories if they don't exists already in the
        /// filesystem. Calls <tt>CreateDirs(String)</tt> with isFile set to true.
        /// </summary>
        /// <param name="filename">The name of the file (fully qualified)
        /// 
        /// </param>
        public static void CreateDirs(String filename)
        {
            CreateDirs(filename, true);
        }
        /// <summary> Creates a repository directory, that is a directory whithin the given
        /// basedir that has a unique name based on sequential numbers.
        /// i.e.:
        /// If directory /home/user/a is basedir and it contains folders 1 2 4 and 5
        /// This method will create a directory named 3 under /home/user/a
        /// </summary>
        /// <param name="basedir">The name of the repository base directory (fully qualified)
        /// 
        /// </param>
        public static DirectoryInfo CreateRepoDir(String basedir)
        {
            if (basedir == null)
                return null;
            CreateDirs(basedir, false);

            DirectoryInfo baseDir = new DirectoryInfo(basedir);

            DirectoryInfo[] dirs = baseDir.GetDirectories();
            Hashtable dirNames = new Hashtable();
            for (int i = 0; i < dirs.Length; i++)
            {
                dirNames.Add(dirs[i].Name, dirs[i]);
            }
            int nextid = 0;
            String sNextid = "" + nextid;
            while (dirNames.ContainsKey(sNextid))
            {
                nextid++;
                sNextid = "" + nextid;
            }
            String newDir = baseDir.FullName + Path.DirectorySeparatorChar.ToString() + sNextid;
            Directory.CreateDirectory(newDir);
            return new DirectoryInfo(newDir);
        }

        /// <summary> Retrieves a hashtable with all the files/directory names, contained in
        /// the dirname given, as keys with File handler to each of the files/dirs
        /// </summary>
        /// <param name="dirname">The name of the directory we want to list
        /// 
        /// </param>
        public static Hashtable ListDir(String dirname)
        {
            return ListDir(dirname, -1);
        }
        /// <summary> Retrieves a hashtable with all the files/directory names, contained in
        /// the dirname given, as keys with FileInfo to each of the files/dirs
        /// </summary>
        /// <param name="dirname">The name of the directory we want to list
        /// </param>
        /// <param name="type">The type of files that we want to list either <tt>DIR</tt>
        /// for retrieving only directories, <tt>FILE</tt> for files only or any
        /// other value for both files and directories (For other types it is
        /// suggested to use <tt>ListDir(String)</tt> )
        /// 
        /// </param>
        public static Hashtable ListDir(String dirname, int type)
        {
            if (dirname == null)
                return new Hashtable();

            FileInfo baseFile = new FileInfo(dirname);
            if (File.Exists(baseFile.FullName))
            {
                dirname = baseFile.DirectoryName;
            }
            DirectoryInfo baseDir = new DirectoryInfo(dirname);
            if (!Directory.Exists(baseDir.FullName))
                return new Hashtable();

            Hashtable dirNames = new Hashtable();

            if (type == DIR)
            {
                DirectoryInfo[] dirs = baseDir.GetDirectories();
                for (int i = 0; i < dirs.Length; i++)
                {
                    dirNames.Add(dirs[i].Name, dirs[i]);
                }
            }
            else if (type == FILE)
            {
                FileInfo[] files = baseDir.GetFiles();
                for (int i = 0; i < files.Length; i++)
                {
                    dirNames.Add(files[i].Name, files[i]);
                }
            }
            else
            {
                DirectoryInfo[] dirs = baseDir.GetDirectories();
                for (int i = 0; i < dirs.Length; i++)
                {
                    dirNames.Add(dirs[i].Name, dirs[i]);
                }
                FileInfo[] files = baseDir.GetFiles();
                for (int i = 0; i < files.Length; i++)
                {
                    dirNames.Add(files[i].Name, files[i]);
                }
            }

            return dirNames;
        }
        /// <summary> Verifies that the separators in the filename are correct based on the
        /// system defined file separator
        /// </summary>
        /// <param name="filename">The name of the file (fully qualified)
        /// 
        /// </param>
        public static String VerifySeparator(String filename)
        {
            if (filename == null)
            {
                return null;
            }
            if (filename.IndexOf(FileUtility.SLASH_BACK) >= 0 && Path.DirectorySeparatorChar.ToString().Equals(FileUtility.SLASH_FORWARD))
            {
                return filename.Replace(FileUtility.SLASH_BACK, Path.DirectorySeparatorChar.ToString());
            }
            if (filename.IndexOf(FileUtility.SLASH_FORWARD) >= 0 && Path.DirectorySeparatorChar.ToString().Equals(FileUtility.SLASH_BACK))
            {
                return filename.Replace(FileUtility.SLASH_FORWARD, Path.DirectorySeparatorChar.ToString());
            }
            return filename;
        }
        /// <summary> Converts a Namespace name to a directory path
        /// </summary>
        /// <param name="pack">The name of the .NET namespace
        /// 
        /// </param>
        public static String ConvertPackageToDir(String pack)
        {
            if (pack == null || pack.Equals("null"))
                return "";
            StringTokenizer token = new StringTokenizer(pack, ".");
            StringBuilder packBuff = new StringBuilder();
            while (token.HasMoreTokens())
            {
                packBuff.Append(Path.DirectorySeparatorChar.ToString());
                packBuff.Append(token.NextToken());
            }
            return packBuff.ToString();
        }

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
            if (location.StartsWith(FileUtility.EMBEDDED_RESOURCE))
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
                            if (StringUtility.ContainsIgnoreCase(rName, name))
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
        /// <summary>
        /// Get a stream for a file that might be an embedded file or a regular file in the system
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Stream GetReadOnlyFileStream(string fileName)
        {
            Stream fis = null;
            if (fileName != null)
            {
                if (fileName.StartsWith(EMBEDDED_RESOURCE))
                {
                    fis = FileUtility.ReadEmbeddedResource(fileName);
                }
                else
                {
                    fis = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                }
            }
            return fis;
        }
        /// <summary>
        /// Reads a given text and places the contents in the given target returning the number of bytes read.
        /// </summary>
        /// <param name="sourceTextReader">The source text</param>
        /// <param name="target">Where the contents read are to be placed</param>
        /// <param name="start">Start position</param>
        /// <param name="count">Number of characters to read</param>
        /// <returns>Count of the bytes read</returns>
        public static Int32 ReadInput(TextReader sourceTextReader, ref sbyte[] target, int start, int count)
        {
            char[] charArray = new char[target.Length];
            int bytesRead = sourceTextReader.Read(charArray, start, count);

            for (int index = start; index < start + bytesRead; index++)
                target[index] = (sbyte)charArray[index];

            return bytesRead;
        }
        /// <summary>
        /// <see cref="ReadInput(TextReader,ref sbyte[],int,int)"/>
        /// </summary>
        /// <param name="sourceStream"></param>
        /// <param name="target"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Int32 ReadInput(Stream sourceStream, ref sbyte[] target, int start, int count)
        {
            byte[] receiver = new byte[target.Length];
            int bytesRead = sourceStream.Read(receiver, start, count);

            for (int i = start; i < start + bytesRead; i++)
                target[i] = (sbyte)receiver[i];

            return bytesRead;
        }
        /// <summary>
        /// Deserializes an object from a binary reader
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <returns></returns>
        public static object Deserialize(BinaryReader binaryReader)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(binaryReader.BaseStream);
        }

        /*******************************/
        /// <summary>
        /// Writes an object to the specified Stream
        /// </summary>
        /// <param name="stream">The target Stream</param>
        /// <param name="objectToSend">The object to be sent</param>
        public static void Serialize(Stream stream, object objectToSend)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, objectToSend);
        }

        /// <summary>
        /// Writes an object to the specified BinaryWriter
        /// </summary>
        /// <param name="binaryWriter">The target BinaryWriter</param>
        /// <param name="objectToSend">The object to be sent</param>
        public static void Serialize(BinaryWriter binaryWriter, object objectToSend)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(binaryWriter.BaseStream, objectToSend);
        }
        //
        // Helper classes and interfaces
        //
        /// <summary>  Interface for objects wishing to process every line in a file.
        /// </summary>
        public interface LineReader
        {
            /// <summary>  Called once for every line in the file.
            /// *
            /// </summary>
            /// <param name="s">  The line just read from the file.
            /// *
            /// </param>
            /// <returns> true to continue reading, false to abort reading.
            /// 
            /// </returns>
            bool OnRead(String s);
        }
    }
}
