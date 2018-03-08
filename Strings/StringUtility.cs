using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sirona.Utilities.IO;

namespace Sirona.Utilities.Strings
{
    public static class StringUtility
    {
        /// <summary>
        /// Check whether or not the provided string is null or an empty string ("").
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static bool NotEmpty(string src)
        {
            return !string.IsNullOrEmpty(src);
        }
        /// <summary>  Returns a non null value of a String.. i.e. empty string if value is null.
        /// </summary>
        /// <param name="s">  The string to extract a non null value from.
        /// </param>
        /// <returns> the original string or an empty string if null.
        /// </returns>
        public static string NonNullValue(string s)
        {
            if (s == null)
                return "";
            return s;
        }
        /// <summary>
        /// Get a null value if the provided string is empty ("").
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetValueEmptyIsNull(string s)
        {
            if (string.IsNullOrEmpty(s))
                return null;
            return s;
        }
        /// <summary> 
        /// Splits a string based on a given regular expression
        /// </summary>
        /// <param name="orig">The original String to split
        /// </param>
        /// <param name="regex"> The regular expression to use as a splitter.
        /// </param>
        /// <returns> an array of strings. 
        /// </returns>
        public static List<string> SplitToListRegex(string orig, string regex)
        {
            List<string> rv = new List<string>();
            if (string.IsNullOrEmpty(orig))
            {
                return rv;
            }
            else if (string.IsNullOrEmpty(regex))
            {
                rv.Add(orig);
                return rv;
            }
            Regex r = new Regex(regex);
            string[] splet = r.Split(orig);
            for (int i = 0; i < splet.Length; i++)
            {
                rv.Add(splet[i]);
            }
            return rv;
        }
        /// <summary>
        /// Splits based on a given substring
        /// </summary>
        /// <param name="orig">The string to split</param>
        /// <param name="spl">The substring to use for splitting the string</param>
        /// <returns></returns>
        public static List<string> SplitToList(string orig, string spl)
        {
            List<string> rv = new List<string>();
            if (string.IsNullOrEmpty(orig))
            {
                return rv;
            }
            if (string.IsNullOrEmpty(spl) && !string.IsNullOrEmpty(orig))
            {
                rv.Add(orig);
                return rv;
            }
            string[] splet = orig.Split(spl.ToCharArray());
            for (int i = 0; i < splet.Length; i++)
            {
                rv.Add(splet[i]);
            }
            return rv;
        }

        /// <summary> Count the occurrences of the substring in string s
        /// </summary>
        /// <param name="s">string to search in. Returns 0 if this is null
        /// </param>
        /// <param name="sub">string to search for. Return 0 if this is null.
        /// </param>
        /// <returns>number of times sub is present in s</returns>
        public static int CountOccurrencesOf(string s, string sub)
        {
            if (s == null || string.IsNullOrEmpty(sub))
                return 0;
            Regex r = new Regex(sub);
            MatchCollection mc = r.Matches(s);
            return mc.Count;
        }
        ///<summary>
        ///Delete all occurrences of a pattern in a string
        ///</summary>	
        ///<param name="inString">String to perform delete action on</param>
        ///<param name="pattern">pattern to delete all occurrences of 
        ///</param>
        ///<returns>The string with all occurrences of pattern removed</returns>
        public static string Delete(string inString, string pattern)
        {
            return inString.Replace(pattern, string.Empty);
        }
        /// <summary>
        /// Removes the first occurrence of the given pattern from the source string
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string DeleteFirst(string source, string pattern)
        {
            if (!Contains(source, pattern))
            {
                return source;
            }
            if (source.IndexOf(pattern) == 0)
            {
                return source.Substring(pattern.Length);
            }
            string result = source.Substring(0, source.IndexOf(pattern));
            result += source.Substring(source.IndexOf(pattern) + pattern.Length);
            return result;
        }

        ///<summary>
        ///Delete any occurrences of one or more of a given list of characters
        ///</summary>
        ///<param name="inString">String to perform delete action on</param>
        ///<param name="chars">characters to delete e.g. az\n will delete as, zs and new lines 
        ///</param>
        ///<returns>The string with any occurrences of chars removed</returns>
        public static string DeleteAny(string inString, string chars)
        {
            if (inString == null || chars == null)
                return inString;
            StringBuilder out_Renamed = new StringBuilder();
            for (int i = 0; i < inString.Length; i++)
            {
                char c = inString[i];
                if (chars.IndexOf((Char)c) == -1)
                {
                    out_Renamed.Append(c);
                }
            }
            return out_Renamed.ToString();
        }

        /// <summary>  Compares two strings without regard to case, null-safe.
        /// </summary>
        /// <param name="s1"> a string to compare against <tt>s2</tt>
        /// </param>
        /// <param name="s2"> a string to compare against <tt>s1</tt>
        /// </param>
        /// <returns> Zero if both strings are "identical", negative if
        /// <tt>s1</tt> is "less than" <tt>s2</tt>, or
        /// positive if <tt>s1</tt> is "greater than" <tt>s2</tt>.
        /// 
        /// </returns>
        public static int CompareToIgnoreCase(string s1, string s2)
        {
            return string.Compare(s1, s2, true);
        }
        /// <summary>
        /// Determine if 2 strings are equals regardless of case
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns>true if strings are the same without considering the case</returns>
        public static bool EqualsIgnoreCase(string s1, string s2)
        {
            return CompareToIgnoreCase(s1, s2) == 0;
        }
        /// <summary>
        /// Determine if a string contains another ignoring case
        /// </summary>
        /// <param name="orig">the original string</param>
        /// <param name="s1">the string to look for</param>
        /// <returns></returns>
        public static bool ContainsIgnoreCase(string orig, string s1)
        {
            if (orig == null || s1 == null)
            {
                return false;
            }
            return orig.ToLower().IndexOf(s1.ToLower()) >= 0;
        }
        /// <summary>
        /// Determine if a string contains another 
        /// </summary>
        /// <param name="orig">the original string</param>
        /// <param name="s1">the string to look for</param>
        /// <returns></returns>
        public static bool Contains(string orig, string s1)
        {
            if (orig == null || s1 == null)
            {
                return false;
            }
            return orig.IndexOf(s1) >= 0;
        }
        /// <summary>
        /// Deletes all occurrences of the given string in a case insensitive manner
        /// </summary>
        /// <param name="orig">the original string</param>
        /// <param name="s1">the string to delete</param>
        /// <returns></returns>
        public static string DeleteIgnoreCase(string orig, string s1)
        {
            return ReplaceIgnoreCase(orig, s1, null);
        }
        /// <summary>
        /// Replaces a string ignoring case
        /// </summary>
        /// <param name="orig">The original string</param>
        /// <param name="s1">the string to search for</param>
        /// <param name="s2">the string to replcae all</param>
        /// <returns></returns>
        public static string ReplaceIgnoreCase(string orig, string s1, string s2)
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
        /// 
        /// </summary>
        public static string LineBreakWindows
        {
            get
            {
                return "\r\n";
            }
        }

        /// <summary>
        /// Convert byte array into string of 3 digit integers.
        /// For example the byte array (11010100,00010111,01110000)
        /// wioll be returned as "212023112"
        /// </summary>
        /// <param name="bin">byte array to convert</param>
        /// <returns>string representation of byte array</returns>
        public static string ByteArrayToIntString(byte[] bin)
        {
            string val = null;
            foreach (byte b in bin)
            {
                string temp = b.ToString();
                val += temp.PadLeft(3, '0');
            }

            return val;
        }

        /// <summary>
        /// Convert int string into byte array.
        /// For example "212023112" will be converted to the byte array 
        /// (11010100,00010111,01110000).  Expects string to be evenly 
        /// divisible by 3. 
        /// </summary>
        /// <param name="s">String to convert</param>
        /// <returns>byte array</returns>
        public static byte[] IntStringToByteArray(string s)
        {
            if ((s.Length % 3) != 0)
                throw new ApplicationException("String length must be evenly divisible by 3."
                  + "  " + s + " is not.");

            byte[] b = new byte[s.Length / 3];

            for (int i = 0; i < b.Length; i++)
            {
                b[i] = Convert.ToByte(s.Substring(i * 3, 3));
            }

            return b;
        }
        /// <summary>
        /// Converts a namespace into a directory structure
        /// </summary>
        /// <param name="nspace">Something like A.B.C</param>
        /// <returns>something like A\B\C</returns>
        public static string NamespaceToDir(string nspace)
        {
            if (nspace == null) return string.Empty;
            if (nspace.IndexOf(".") < 0) return nspace;
            return nspace.Replace(".", System.IO.Path.DirectorySeparatorChar.ToString());
        }
        /// <summary>
        /// Converts a slash separated structure of strings into back-slash separated structure and viceversa
        /// </summary>
        /// <param name="pValue">Something like A/B/C</param>
        /// <returns>something like A\B\C</returns>
        public static string ForwardToBackSlash(string pValue)
        {
            return ToSlashStructure(pValue, true);
        }
        /// <summary>
        /// Converts a slash separated structure of strings into back-slash separated structure and viceversa
        /// </summary>
        /// <param name="pValue">Something like A\B\C</param>
        /// <returns>something like A/B/C</returns>
        public static string BackToForwardSlash(string pValue)
        {
            return ToSlashStructure(pValue, false);
        }
        /// <summary>
        /// Converts a slash separated structure of strings into back-slash separated structure and viceversa
        /// </summary>
        /// <param name="pValue">Something like A\B\C or A/B/C</param>
        /// <param name="forward">/ to \</param>
        /// <returns>something like A/B/C or A\B\C</returns>
        public static string ToSlashStructure(string pValue, bool forward)
        {
            string fSlash = FileUtility.SLASH_FORWARD;
            string bSlash = FileUtility.SLASH_BACK;
            if (pValue == null) return string.Empty;
            if (!forward)
            {
                fSlash = FileUtility.SLASH_BACK;
                bSlash = FileUtility.SLASH_FORWARD;
            }

            if (pValue.IndexOf(bSlash) < 0) return pValue;
            return pValue.Replace(bSlash, fSlash);
        }
        /// <summary>
        /// Converts xxxx into Xxxx
        /// </summary>
        /// <param name="pValue">value to convert</param>
        /// <returns>result</returns>
        public static string Capitalize(string pValue)
        {
            return pValue.Substring(0, 1).ToUpper() + pValue.Substring(1, pValue.Length - 1);
        }
        /// <summary>
        /// Extracts the string contained within the first occurreces of startDelim and endDelim in source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startDelim"></param>
        /// <param name="endDelim"></param>
        /// <returns></returns>
        public static string ExtractFromDelimiters(string source, string startDelim, string endDelim)
        {
            if (startDelim == null || !source.Contains(startDelim) || !source.Contains(endDelim))
            {
                return source;
            }
            int strt = source.IndexOf(startDelim) + startDelim.Length;
            string sourceNew = source.Substring(strt);
            int indxOfEnd = sourceNew.Contains(endDelim) ? sourceNew.IndexOf(endDelim) + strt : (source.Length - 1);
            int lnght = source.Length - strt - (source.Length - indxOfEnd);
            return source.Substring(strt, lnght);
        }
        /// <summary>
        /// Determines if the delimiters are present and in the correct order
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startDelim"></param>
        /// <param name="endDelim"></param>
        /// <returns></returns>
        public static bool ContainsDelimiters(string source, string startDelim, string endDelim)
        {
            if(source == null || endDelim == null || startDelim == null)
            {
                //They can't be null
                return false;
            }
            bool hasDels = false;
            int startIndx = source.IndexOf(startDelim);
            if (startIndx >= 0)
            {
                int endIndx = source.Substring(startIndx + startDelim.Length).IndexOf(endDelim);
                if (endIndx >= 0)
                {
                    hasDels = true;
                }
            }
            return hasDels;
        }
        /// <summary>
        /// Cleans up the new line characters and the tab characters and limits the string to a length
        /// </summary>
        /// <param name="source">the source string</param>
        /// <param name="limit">the limit or maximum length of the desired string</param>
        /// <returns></returns>
        public static string CleanAndLimitString(string source, int limit)
        {
            source = Delete(Delete(source.Trim(), Environment.NewLine), "\t");
            return LimitString(source, limit);
        }
        /// <summary>
        /// Cuts the string to a specified length if its length exceeds the given limit
        /// </summary>
        /// <param name="source">the source string</param>
        /// <param name="limit">the limit or maximum length of the desired string</param>
        /// <returns></returns>
        public static string LimitString(string source, int limit)
        {
            if (source.Length > limit)
            {
                source = source.Substring(0, limit) + " ....";
            }
            return source;
        }
        /// <summary>
        /// Replace a regular expression with a string value
        /// example : to replace pasword=annie to password=xxx use,
        /// string result = StringUtils.ReplaceRegex("password=annie", @"password=\w+", "password=xxx");
        /// </summary>
        /// <param name="original"></param>
        /// <param name="sourceRegex"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string ReplaceRegex(string original, string sourceRegex, string replacement)
        {
            Regex r = new Regex(sourceRegex);
            return r.Replace(original, replacement);
        }
        /// <summary>
        /// A method to call the clone method on a string if the string is not null.
        /// returns a null string if the source string is null.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string NullSafeClone(string source)
        {
            if (source != null)
            {
                return source.Clone() as string;
            }
            return null;
        }

        /// <summary>
        /// Turn a string into a bitwise int for things like bit flag operations.
        /// For example :
        /// int flags = StringUtility.ToBitWiseInt("01001");
        /// should set flags to 7.
        /// </summary>
        /// <param name="bitsString"></param>
        /// <returns></returns>
        public static int ToBitWiseInt(string bitsString)
        {
            int value = 0;
            int bitRep = 1;
            char[] bitarray = bitsString.ToCharArray();
            for (int pos = bitarray.Length - 1; pos > 0; pos--)
            {
              if (bitarray[pos] == '1')
              {
                value = value + bitRep;
              }
              bitRep = 2 * bitRep;
            }
            return value;
        }
    }
}
