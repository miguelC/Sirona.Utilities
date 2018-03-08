using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Sirona.Utilities.WCF.Message.Encoders.GZipEncoder
{
    class Encoding
    {
        private const string gKey = "aj&^fk:f!~`@#$%^()HF^f&g'd:P:s%F";

        public static byte[] EncodePackage(byte[] arrByte)
        {
            string sMethodFullName = Convert.ToString(typeof(Encoding)) + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;

            try
            {
                RijndaelManaged myRijndael = new RijndaelManaged();
                myRijndael.Padding = PaddingMode.Zeros;
                byte[] key = System.Text.Encoding.ASCII.GetBytes(gKey);
                byte[] IV = System.Text.Encoding.ASCII.GetBytes(gKey.Substring(1, 16));
                ICryptoTransform encryptor = myRijndael.CreateEncryptor(key, IV);
                MemoryStream msEncrypt = new MemoryStream();
                CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                csEncrypt.Write(arrByte, 0, arrByte.Length);
                csEncrypt.FlushFinalBlock();
                return msEncrypt.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in " + sMethodFullName + Environment.NewLine + ex.Message);
            }

        }

        public static byte[] DecodePackage(byte[] arrByte)
        {
            string sMethodFullName = Convert.ToString(typeof(Encoding)) + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;

            try
            {
                RijndaelManaged myRijndael = new RijndaelManaged();
                myRijndael.Padding = PaddingMode.Zeros;
                byte[] key = System.Text.Encoding.ASCII.GetBytes(gKey);
                byte[] IV = System.Text.Encoding.ASCII.GetBytes(gKey.Substring(1, 16));
                ICryptoTransform decryptor = myRijndael.CreateDecryptor(key, IV);
                byte[] fromEncrypt = null;
                int iCount = 0;
                MemoryStream msDecrypt = new MemoryStream(arrByte);
                CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                fromEncrypt = new byte[arrByte.Length + 1];
                iCount = csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);
                byte[] newBytes = new byte[iCount];
                Array.Copy(fromEncrypt, newBytes, iCount);
                return newBytes;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in " + sMethodFullName + Environment.NewLine + ex.Message);
            }

        }
    }
}
