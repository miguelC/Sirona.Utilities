using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Configuration;
using System.Xml.Serialization;

namespace Sirona.Utilities.Encryption
{
    public enum EncryptionAlgorithm
    {
        Rijndael, DES, TripleDES, RC2
    }

    public class EncryptionUtility
    {
        //public static BinaryKeyVectorPair GetEncryptionKeyAndInitializationVector()
        //{

        //    System.Configuration.Configuration c = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming);

        //    EncryptionConfiguration conf = null;
        //    if (c != null)
        //    {

        //        conf = (EncryptionConfiguration)
        //            c.GetSection(
        //                EncryptionConfigurationConstants.SECTION_NAME);
        //    }
        //    conf = CheckConfiguration(conf);

        //    ICryptoTransform encryptor = null;
        //    BinaryKeyVectorPair kvpair = GetValidKeyVectorPair(conf);
        //}

        public static BinaryKeyVectorPair GetValidKeyVectorPair(byte[] key, byte[] vector, EncryptionAlgorithm algorithm)
        {
            BinaryKeyVectorPair pair = new BinaryKeyVectorPair();
            switch (algorithm)
            {
                case EncryptionAlgorithm.DES:
                    //key is 8 bytes
                    DESCryptoServiceProvider prov = new DESCryptoServiceProvider();
                    pair.Key = LimitBytes(key, prov.LegalKeySizes);
                    pair.Vector = LimitBytes(vector, prov.LegalBlockSizes);
                    break;
                case EncryptionAlgorithm.TripleDES:
                    TripleDESCryptoServiceProvider prov1 = new TripleDESCryptoServiceProvider();
                    pair.Key = LimitBytes(key, prov1.LegalKeySizes);
                    pair.Vector = LimitBytes(vector, prov1.LegalBlockSizes);
                    break;
                case EncryptionAlgorithm.RC2:
                    RC2CryptoServiceProvider prov2 = new RC2CryptoServiceProvider();
                    pair.Key = LimitBytes(key, prov2.LegalKeySizes);
                    pair.Vector = LimitBytes(vector, prov2.LegalBlockSizes);
                    break;
                case EncryptionAlgorithm.Rijndael:
                    RijndaelManaged prov3 = new RijndaelManaged();
                    pair.Key = LimitBytes(key, prov3.LegalKeySizes);
                    pair.Vector = LimitBytes(vector, prov3.LegalBlockSizes);
                    break;

            }
            //shouldn't get here
            return pair;
        }
        private static byte[] LimitBytes(byte[] source, KeySizes[] sizes)
        {
            KeySizes asize = sizes[0];
            int byteSize = 8;
            int max = (asize.MaxSize / byteSize);
            if (source.Length < max)
            {
                int exceeds = source.Length % (asize.SkipSize / byteSize);
                if (exceeds != 0)
                {
                    int size = source.Length - exceeds;
                    byte[] copy = new byte[size];
                    for (int i = 0; i < size; i++)
                    {
                        copy[i] = source[i];
                    }
                    return copy;
                }
            }
            else
            {
                byte[] copy = new byte[max];
                for (int i = 0; i < max; i++)
                {
                    copy[i] = source[i];
                }
                return copy;
            }
            return source;
        }
        public static ICryptoTransform GetEncryptor(EncryptionAlgorithm algorithm, byte[] encryptionKey, byte[] initializationVector)
        {
            return GetEncryptorOrDecryptor(algorithm, encryptionKey, initializationVector, true);
        }
        public static ICryptoTransform GetDecryptor(EncryptionAlgorithm algorithm, byte[] encryptionKey, byte[] initializationVector)
        {
            return GetEncryptorOrDecryptor(algorithm, encryptionKey, initializationVector, false);
        }

        public static ICryptoTransform GetEncryptorOrDecryptor(EncryptionAlgorithm algorithm, byte[] encryptionKey, byte[] initializationVector, bool encr)
        {
            BinaryKeyVectorPair kvp = GetValidKeyVectorPair(encryptionKey, initializationVector, algorithm);
            switch (algorithm)
            {
                case EncryptionAlgorithm.DES:
                    DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                    //DES Keys are 8 bytes long only
                    if (encr)
                    {
                        return des.CreateEncryptor(kvp.Key, kvp.Vector);
                    }
                    else
                    {
                        return des.CreateDecryptor(kvp.Key, kvp.Vector);
                    }
                case EncryptionAlgorithm.RC2:
                    RC2CryptoServiceProvider rc2 = new RC2CryptoServiceProvider();
                    if (encr)
                    {
                        return rc2.CreateEncryptor(kvp.Key, kvp.Vector);
                    }
                    else
                    {
                        return rc2.CreateDecryptor(kvp.Key, kvp.Vector);
                    }
                case EncryptionAlgorithm.TripleDES:
                    TripleDESCryptoServiceProvider trid = new TripleDESCryptoServiceProvider();
                    //3DES keys are 24 bytes long only
                    if (encr)
                    {
                        return trid.CreateEncryptor(kvp.Key, kvp.Vector);
                    }
                    else
                    {
                        return trid.CreateDecryptor(kvp.Key, kvp.Vector);
                    }
                case EncryptionAlgorithm.Rijndael:
                    RijndaelManaged rm = new RijndaelManaged();
                    if (encr)
                    {
                        return rm.CreateEncryptor(kvp.Key, kvp.Vector);
                    }
                    else
                    {
                        return rm.CreateDecryptor(kvp.Key, kvp.Vector);
                    }
            }
            return null;
        }

        public static string Encrypt(string cipherText, byte[] encryptionKey, byte[] initializationVector, EncryptionAlgorithm algorithm)
        {
            return Encrypt(cipherText, encryptionKey, initializationVector, algorithm, Encoding.ASCII);
        }
        public static string Encrypt(string cipherText, byte[] encryptionKey, byte[] initializationVector, EncryptionAlgorithm algorithm, Encoding encoding)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                ICryptoTransform encryptor = GetEncryptor(algorithm, encryptionKey, initializationVector);
                CryptoStream crptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

                byte[] data = encoding.GetBytes(cipherText);

                crptoStream.Write(data, 0, data.Length);
                crptoStream.FlushFinalBlock();
                crptoStream.Close();
                memoryStream.Close();
                return Convert.ToBase64String(memoryStream.ToArray());

            }
        }
        public static string Encrypt(
            string plainText, 
            string passPhrase,
            string saltValue, 
            string hashAlgorithm,
            int passwordIterations, 
            string initVector,
            int keySize)
        {
            return Encrypt(plainText, passPhrase, saltValue, hashAlgorithm, passwordIterations, initVector, keySize, EncryptionAlgorithm.Rijndael);
        }
        public static string Encrypt(
            string plainText, 
            string passPhrase,
            string saltValue, 
            string hashAlgorithm,
            int passwordIterations, 
            string initVector,
            int keySize,
            EncryptionAlgorithm algorithm)
        {
            return Encrypt(plainText, passPhrase, saltValue, hashAlgorithm, passwordIterations, initVector, keySize, algorithm, Encoding.ASCII, Encoding.UTF8);
        }
        public static string Encrypt(
            string plainText, 
            string passPhrase,
            string saltValue, 
            string hashAlgorithm,
            int passwordIterations, 
            string initVector,
            int keySize,
            EncryptionAlgorithm algorithm,
            Encoding initKeysEncoding,
            Encoding resultStringEncoding)
        {

            // Convert strings into byte arrays.
            // Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8 
            // encoding.
            byte[] initVectorBytes = null;
            initVectorBytes = initKeysEncoding.GetBytes(initVector);

            byte[] saltValueBytes = null;
            saltValueBytes = initKeysEncoding.GetBytes(saltValue);
            
            // First, we must create a password, from which the key will be derived.
            // This password will be generated from the specified passphrase and 
            // salt value. The password will be created using the specified hash 
            // algorithm. Password creation can be done in several iterations.
            PasswordDeriveBytes password = null;
            password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = null;
            keyBytes = password.GetBytes(keySize / 8);

            //// Create uninitialized Rijndael encryption object.
            //RijndaelManaged symmetricKey = null;
            //symmetricKey = new RijndaelManaged();

            //// It is reasonable to set encryption mode to Cipher Block Chaining
            //// (CBC). Use default options for other symmetric key parameters.
            //symmetricKey.Mode = CipherMode.CBC;

            // Generate encryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key 
            // bytes.

            return Encrypt(plainText, keyBytes, initVectorBytes, algorithm, resultStringEncoding);
        }

        #region Decryption

        public static string Decrypt(string cipherText, byte[] encryptionKey, byte[] initializationVector, EncryptionAlgorithm algorithm)
        {
            return Decrypt(cipherText, encryptionKey, initializationVector, algorithm, Encoding.ASCII);
        }
        public static string Decrypt(string cipherText, byte[] encryptionKey, byte[] initializationVector, EncryptionAlgorithm algorithm, Encoding encoding)
        {
            byte[] cipher = Convert.FromBase64String(cipherText);
            ICryptoTransform encryptor = GetDecryptor(algorithm, encryptionKey, initializationVector);
            using (MemoryStream memoryStream = new MemoryStream(cipher))
            {
                CryptoStream crptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Read);
                byte[] data = new byte[cipher.Length];
                int dataLength = crptoStream.Read(data, 0, data.Length);
                memoryStream.Close();
                crptoStream.Close();
                return encoding.GetString(data, 0, dataLength);
            }
        }

        public static string Decrypt(
            string cipherText, 
            string passPhrase, 
            string saltValue, 
            string hashAlgorithm, 
            int passwordIterations, 
            string initVector, 
            int keySize)
        {
            //Default algorith is Rinjdael
            return Decrypt(cipherText, passPhrase, saltValue, hashAlgorithm, passwordIterations, initVector, keySize, EncryptionAlgorithm.Rijndael);        
        }
        public static string Decrypt(
            string cipherText, 
            string passPhrase, 
            string saltValue, 
            string hashAlgorithm, 
            int passwordIterations, 
            string initVector, 
            int keySize, 
            EncryptionAlgorithm algorithm)
        {
            //Default key encoding is ascii and default string encoding is UTF-8
            return Decrypt(cipherText, passPhrase, saltValue, hashAlgorithm, passwordIterations, initVector, keySize, algorithm, Encoding.ASCII, Encoding.UTF8);
        }
        public static string Decrypt(
            string cipherText, 
            string passPhrase, 
            string saltValue, 
            string hashAlgorithm, 
            int passwordIterations, 
            string initVector, 
            int keySize, 
            EncryptionAlgorithm algorithm,
            Encoding initKeysEncoding,
            Encoding resultStringEncoding)
        {

            // Convert strings defining encryption key characteristics into byte
            // arrays. Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8
            // encoding.
            byte[] initVectorBytes = null;
            initVectorBytes = initKeysEncoding.GetBytes(initVector);

            byte[] saltValueBytes = null;
            saltValueBytes = initKeysEncoding.GetBytes(saltValue);

            // Convert our ciphertext into a byte array.
            byte[] cipherTextBytes = null;
            cipherTextBytes = Convert.FromBase64String(cipherText);

            // First, we must create a password, from which the key will be 
            // derived. This password will be generated from the specified 
            // passphrase and salt value. The password will be created using
            // the specified hash algorithm. Password creation can be done in
            // several iterations.
            PasswordDeriveBytes password = null;
            password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = null;
            keyBytes = password.GetBytes(keySize / 8);

            //// Create uninitialized Rijndael encryption object.
            //RijndaelManaged symmetricKey = null;
            //symmetricKey = new RijndaelManaged();

            //// It is reasonable to set encryption mode to Cipher Block Chaining
            //// (CBC). Use default options for other symmetric key parameters.
            //symmetricKey.Mode = CipherMode.CBC;
            
            return Decrypt(cipherText, keyBytes, initVectorBytes, algorithm, resultStringEncoding);
        }
        
        #endregion



        #region Encrypt/Decrypt a file

        public static void EncryptFile(string source, string target, byte[] encryptionKey, byte[] initializationVector, EncryptionAlgorithm algorithm)
        {
            using (FileStream fsFileOut = File.Open(target, FileMode.OpenOrCreate, FileAccess.Write))
            {
                ICryptoTransform encryptor = GetEncryptor(algorithm, encryptionKey, initializationVector);
                CryptoStream crptoStream = new CryptoStream(fsFileOut, encryptor, CryptoStreamMode.Write);

                WriteFileUsingCryptoStream(source, crptoStream);
                crptoStream.FlushFinalBlock();
                crptoStream.Close();
                fsFileOut.Close();
            }
        }
        protected static void WriteFileUsingCryptoStream(string source, CryptoStream crptoStream)
        {
            using (FileStream fsIn = new FileStream(source, FileMode.Open, FileAccess.Read))
            {
                int bufferLen = 4096;
                byte[] buffer = new byte[bufferLen];
                int bytesRead;
                int offset = 0;
                do
                {
                    bytesRead = fsIn.Read(buffer, offset, bufferLen);
                    // encrypt it 
                    crptoStream.Write(buffer, offset, bytesRead);
                } while (bytesRead == bufferLen);

                fsIn.Close();
            }
        }
        public static void DecryptFile(string source, string target, byte[] encryptionKey, byte[] initializationVector, EncryptionAlgorithm algorithm)
        {
            using (FileStream fsFileOut = File.Open(target, FileMode.OpenOrCreate, FileAccess.Write))
            {
                ICryptoTransform decryptor = GetDecryptor(algorithm, encryptionKey, initializationVector);
                CryptoStream crptoStream = new CryptoStream(fsFileOut, decryptor, CryptoStreamMode.Write);
                WriteFileUsingCryptoStream(source, crptoStream);

                crptoStream.FlushFinalBlock();
                crptoStream.Close();
                fsFileOut.Close();
            }
        }
        #endregion
    }
}
