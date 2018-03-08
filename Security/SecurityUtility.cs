using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace Sirona.Utilities.Security
{
    public class SecurityUtility
    {

        public static X509Certificate2 FindFirstCertificateByThumbprintInLocalMachinePersonalStore(string findValue)
        {
            return FindFirstCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindByThumbprint, findValue);
        }
        public static X509Certificate2 FindFirstCertificateByThumbprintInPersonalStore(StoreLocation location, string findValue)
        {
            return FindFirstCertificate(location, StoreName.My, X509FindType.FindByThumbprint, findValue);
        }
        public static X509Certificate2 FindFirstCertificateByThumbprint(StoreLocation location, StoreName name, string findValue)
        {
            return FindFirstCertificate(location, name, X509FindType.FindByThumbprint, findValue);
        }

        public static X509Certificate2 FindFirstCertificate(StoreLocation location, StoreName name, X509FindType findType, string findValue)
        {
            X509Store store = new X509Store(name, location);
            try
            {
                // create and open store for read-only access
                store.Open(OpenFlags.ReadOnly);

                // search store
                X509Certificate2Collection col = store.Certificates.Find(findType, findValue, true);

                // return first certificate found
                return col[0];
            }
            // always close the store
            finally { store.Close(); }
        }


        public static X509Certificate2Collection FindCertificatesByThumbprintInLocalMachinePersonalStore(string findValue)
        {
            return FindCertificates(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindByThumbprint, findValue);
        }
        public static X509Certificate2Collection FindCertificatesByThumbprintInPersonalStore(StoreLocation location, string findValue)
        {
            return FindCertificates(location, StoreName.My, X509FindType.FindByThumbprint, findValue);
        }
        public static X509Certificate2Collection FindCertificatesByThumbprint(StoreLocation location, StoreName name, string findValue)
        {
            return FindCertificates(location, name, X509FindType.FindByThumbprint, findValue);
        }
        public static X509Certificate2Collection FindCertificates(StoreLocation location, StoreName name, X509FindType findType, string findValue)
        {
            X509Store store = new X509Store(name, location);
            try
            {
                // create and open store for read-only access
                store.Open(OpenFlags.ReadOnly);

                // search store
                return store.Certificates.Find(findType, findValue, true);
            }
            // always close the store
            finally { store.Close(); }
        }
    }
}
