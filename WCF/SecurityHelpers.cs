using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sirona.Utilities.WCF
{
    public class SecurityHelpers
    {
        public static bool TlsAllowRemoteCertificateNameMismatch { get; set; }
        public static bool TlsAllowRemoteCertificateNotAvailable { get; set; }
        public static bool TlsAllowRemoteCertificateChainErrors { get; set; }
        
        /// <summary>
        /// This method allows bypassing exceptions such as RemoteCertificateNameMismatch and RemoteCertificateChainErrors based on settings stored in a config file.
        /// It is useful when testing with self-signed certificates from partners.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        public static bool RemoteCertificateValidation(Object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            bool nameMismatchReturn = false;
            bool notAvailableReturn = false;
            bool chainErrorsReturn = false;

            System.Security.Cryptography.X509Certificates.X509Certificate2 cert2 = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificate);
            //logAtnaEvent("404", ((HttpWebRequest)sender).RequestUri.ToString(), "userid");
            //If cert2.NotAfter < DateTime.Now Then
            //End If

            // whether or not to ignore CN mismatch is an option
            // string cnMismatchOption = StaticHelpers.GetConfigSetting("TW_SERVER_CERTIFICATE_IGNORE_CNMISMATCH"); 

            /* if no errors at all */
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            /* RemoteCertificateNameMismatch - if just a name mismatch and we're allowing them */
            if ((sslPolicyErrors == System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch))
            {
                if (TlsAllowRemoteCertificateNameMismatch == true)
                {
                    nameMismatchReturn = true;
                }
                else
                {
                    nameMismatchReturn = false;
                }
            }
            else
            {
                nameMismatchReturn = true;
            }

            /* RemoteCertificateNotAvailable - if there is no available certificate */
            if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateNotAvailable) == System.Net.Security.SslPolicyErrors.RemoteCertificateNotAvailable)
            {
                //logAtnaEvent("408", ((HttpWebRequest)sender).RequestUri.ToString(), "userid");
                if (TlsAllowRemoteCertificateNotAvailable == true)
                {
                    notAvailableReturn = true;
                }
                else
                {
                    notAvailableReturn = false;
                }
            }
            else
            {
                notAvailableReturn = true;
            }

            /* RemoteCertificateChainErrors - if errors exists in the signing chain */
            if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) == System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors)
            {
                //logAtnaEvent("408", ((HttpWebRequest)sender).RequestUri.ToString(), "userid");
                if (TlsAllowRemoteCertificateChainErrors == true)
                {
                    chainErrorsReturn = true;
                }
                else
                {
                    chainErrorsReturn = false;
                }
            }
            else
            {
                chainErrorsReturn = true;
            }

            /* if any checks above are false then return false = allowances have already been accounted for in each check */
            if (nameMismatchReturn == false | notAvailableReturn == false | chainErrorsReturn == false)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
    }
}
