using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;
using System.Xml;


namespace Sirona.Utilities.WCF
{
    public class BindingHelpers
    {
        public enum BindingSecurity
        {
            None,
            TLS,
            TLSWithCertificate,
            Credentials
        }
        public enum MessageEncodingType
        {
            Text,
            Mtom
        }
        public enum MessageSecurityType
        {
            None,
            CertificateOverTransport,
            MutualCertificate,
            Sspi,
            UserNameOverTransport
        }

        public static MessageEncodingBindingElement BuildMessageEncoding(MessageEncodingType encodingType, MessageSizeQuotas quotas)
        {
            if (encodingType == MessageEncodingType.Mtom)
            {

                MtomMessageEncodingBindingElement mtomEncodingBindingElement =
                    new MtomMessageEncodingBindingElement(MessageVersion.Soap12WSAddressing10, System.Text.Encoding.UTF8);

              mtomEncodingBindingElement.MaxBufferSize = quotas.MaxBufferSize;

                return mtomEncodingBindingElement;
            }
            else
            {
                TextMessageEncodingBindingElement textMessageEncoding = 
                    new TextMessageEncodingBindingElement(MessageVersion.Soap12WSAddressing10, System.Text.UTF8Encoding.UTF8);
                textMessageEncoding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                //textMessageEncoding.MaxReadPoolSize = 16;
                //textMessageEncoding.MaxWritePoolSize = 16;
                //textMessageEncoding.ReaderQuotas.MaxDepth = int.MaxValue; //32;
                //textMessageEncoding.ReaderQuotas.MaxStringContentLength = int.MaxValue; //65536;// 8192; 
                //textMessageEncoding.ReaderQuotas.MaxArrayLength = int.MaxValue; //16384;
                //textMessageEncoding.ReaderQuotas.MaxBytesPerRead = int.MaxValue; //4096;
                textMessageEncoding.ReaderQuotas.MaxNameTableCharCount = quotas.MaxBufferSize;
                return textMessageEncoding;
            }
        }

        public static HttpTransportBindingElement BuildHttpTransport(BindingSecurity securityType)
        {
            return BuildHttpTransport(securityType, null);
        }
        public static HttpTransportBindingElement BuildHttpTransport(BindingSecurity securityType, MessageSizeQuotas quotas)
        {
            if (quotas == null)
            {
                quotas = new MessageSizeQuotas()
                {
                    MaxBufferPoolSize = 524288,
                    MaxBufferSize = 15000000,
                    MaxReceivedMessageSize = 15000000
                };
            }
            if (securityType == BindingSecurity.None)
            {
                HttpTransportBindingElement httpTransport = new HttpTransportBindingElement();
                httpTransport.ManualAddressing = false;
                httpTransport.AllowCookies = false;
                httpTransport.AuthenticationScheme = System.Net.AuthenticationSchemes.Anonymous;
                httpTransport.BypassProxyOnLocal = false;
                httpTransport.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
                httpTransport.KeepAliveEnabled = true;
                httpTransport.ProxyAuthenticationScheme = System.Net.AuthenticationSchemes.Anonymous;
                httpTransport.Realm = String.Empty;
                httpTransport.TransferMode = TransferMode.Buffered;
                httpTransport.UnsafeConnectionNtlmAuthentication = false;
                httpTransport.UseDefaultWebProxy = false;
                httpTransport.MaxBufferPoolSize = quotas.MaxBufferPoolSize;
                httpTransport.MaxReceivedMessageSize = quotas.MaxReceivedMessageSize;
                httpTransport.MaxBufferSize = quotas.MaxBufferSize;
                //httpTransport.ExtendedProtectionPolicy.PolicyEnforcement = System.Security.Authentication.ExtendedProtection.PolicyEnforcement.Never;
                return httpTransport;
            }
            else if (securityType == BindingSecurity.TLS || securityType == BindingSecurity.TLSWithCertificate || securityType == BindingSecurity.Credentials)
            {

                HttpsTransportBindingElement httpsTransportBindingElement = new HttpsTransportBindingElement();
                httpsTransportBindingElement.ManualAddressing = false;
                httpsTransportBindingElement.MaxBufferPoolSize = quotas.MaxBufferPoolSize;
                httpsTransportBindingElement.MaxReceivedMessageSize = quotas.MaxReceivedMessageSize;
                httpsTransportBindingElement.MaxBufferSize = quotas.MaxBufferSize;
                httpsTransportBindingElement.AllowCookies = false;
                httpsTransportBindingElement.BypassProxyOnLocal = false;
                httpsTransportBindingElement.Realm = string.Empty;
                httpsTransportBindingElement.KeepAliveEnabled = true;
                httpsTransportBindingElement.TransferMode = TransferMode.Buffered;
                httpsTransportBindingElement.UnsafeConnectionNtlmAuthentication = false;
                httpsTransportBindingElement.UseDefaultWebProxy = true;
                httpsTransportBindingElement.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
                if (securityType == BindingSecurity.TLSWithCertificate)
                {
                    httpsTransportBindingElement.AuthenticationScheme = System.Net.AuthenticationSchemes.Digest;
                    httpsTransportBindingElement.ProxyAuthenticationScheme = System.Net.AuthenticationSchemes.Digest;
                    httpsTransportBindingElement.RequireClientCertificate = true;
                }
                else
                {
                    httpsTransportBindingElement.AuthenticationScheme = System.Net.AuthenticationSchemes.Anonymous;
                    httpsTransportBindingElement.ProxyAuthenticationScheme = System.Net.AuthenticationSchemes.Anonymous;
                    httpsTransportBindingElement.RequireClientCertificate = false;
                }
                return httpsTransportBindingElement;
            }
            else
            {
                HttpTransportBindingElement httpTransport = new HttpTransportBindingElement();
                httpTransport.ManualAddressing = false;
                httpTransport.MaxBufferPoolSize = quotas.MaxBufferPoolSize;
                httpTransport.MaxReceivedMessageSize = quotas.MaxReceivedMessageSize;
                httpTransport.MaxBufferSize = quotas.MaxBufferSize;
                httpTransport.AllowCookies = false;
                httpTransport.AuthenticationScheme = System.Net.AuthenticationSchemes.Anonymous;
                httpTransport.BypassProxyOnLocal = false;
                httpTransport.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
                httpTransport.KeepAliveEnabled = true;
                httpTransport.ProxyAuthenticationScheme = System.Net.AuthenticationSchemes.Anonymous;
                httpTransport.Realm = String.Empty;
                httpTransport.TransferMode = TransferMode.Buffered;
                httpTransport.UnsafeConnectionNtlmAuthentication = false;
                httpTransport.UseDefaultWebProxy = false;
                //httpTransport.ExtendedProtectionPolicy.PolicyEnforcement = System.Security.Authentication.ExtendedProtection.PolicyEnforcement.Never;
                return httpTransport;
            }
        }

        public static SecurityBindingElement BuildMessageSecurity(MessageSecurityType securityType)
        {
            if (securityType == MessageSecurityType.CertificateOverTransport)
            {
                TransportSecurityBindingElement messageSecurity = SecurityBindingElement.CreateCertificateOverTransportBindingElement(
                    MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11);
                messageSecurity.IncludeTimestamp = true;
                return messageSecurity;
            }
            else if(securityType == MessageSecurityType.MutualCertificate)
            {
                SecurityBindingElement messageSecurity = SecurityBindingElement.CreateMutualCertificateBindingElement(
                    MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10, true);
                messageSecurity.DefaultAlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.Basic128Rsa15;
                messageSecurity.IncludeTimestamp = true;
                return messageSecurity;
            }
            else if (securityType == MessageSecurityType.None)
            {
                return null;
            }
            else if (securityType == MessageSecurityType.UserNameOverTransport)
            {
                TransportSecurityBindingElement messageSecurity = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
                messageSecurity.IncludeTimestamp = false;
                messageSecurity.EnableUnsecuredResponse = true;
                messageSecurity.DefaultAlgorithmSuite = System.ServiceModel.Security.Basic256SecurityAlgorithmSuite.Basic256;
                messageSecurity.SetKeyDerivation(false);
                messageSecurity.MessageSecurityVersion = MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10;
                return messageSecurity;
            }
            return SecurityBindingElement.CreateSslNegotiationBindingElement(false);
        }

        public static System.ServiceModel.Channels.Binding BuildCustomBindingSecure()
        {
            return BuildCustomBindingSecure(MessageEncodingType.Text, MessageSecurityType.None);
        }
        public static System.ServiceModel.Channels.Binding BuildCustomBindingSecure(MessageEncodingType encodingType)
        {
            return BuildCustomBindingSecure(encodingType, MessageSecurityType.None);
        }
        public static System.ServiceModel.Channels.Binding BuildCustomBindingSecure(MessageEncodingType encodingType, MessageSecurityType securityType)
        {
            return BuildCustomBindingSecure(encodingType, securityType, BindingSecurity.TLS);
        }
        public static System.ServiceModel.Channels.Binding BuildCustomBindingSecure(MessageEncodingType encodingType, MessageSecurityType securityType, BindingSecurity transportSecurity)
        {
            return BuildCustomBindingSecure(encodingType, securityType, transportSecurity, null, null);
        }
        public static System.ServiceModel.Channels.Binding BuildCustomBindingSecure(MessageEncodingType encodingType, MessageSecurityType securityType,
            BindingSecurity transportSecurity, TimeoutQuotas timeoutQuotas, MessageSizeQuotas sizeQuotas)
        {
            if (sizeQuotas == null)
            {
                sizeQuotas = new MessageSizeQuotas()
                {
                    MaxBufferPoolSize = 15000000,
                    MaxReceivedMessageSize = 15000000,
                    MaxBufferSize = 15000000
                };
            }

            if (timeoutQuotas == null)
            {
                timeoutQuotas = new TimeoutQuotas()
                {
                    CloseTimeout = 600000000, // 1 minute
                    OpenTimeout = 600000000, // 1 minute
                    ReceiveTimeout = 3000000000, // 5 minutes
                    SendTimeout = 3000000000, // 5 minutes
                };
            }
            SecurityBindingElement messageSecurity = BuildMessageSecurity(securityType);

            MessageEncodingBindingElement messageEncodingBindingElement = BuildMessageEncoding(encodingType, sizeQuotas);

            HttpTransportBindingElement transportBindingElement = BuildHttpTransport(transportSecurity, sizeQuotas);
            if (securityType == MessageSecurityType.MutualCertificate || securityType == MessageSecurityType.CertificateOverTransport)
            {
                ((HttpsTransportBindingElement)transportBindingElement).RequireClientCertificate = true;
            }
            
            BindingElementCollection bindingElementCollection = new BindingElementCollection();
            if (messageSecurity != null)
            {
                bindingElementCollection.Add(messageSecurity);
            }
            bindingElementCollection.Add(messageEncodingBindingElement);            
            bindingElementCollection.Add(transportBindingElement);

            CustomBinding cb = new CustomBinding(bindingElementCollection);
            cb.ReceiveTimeout = new TimeSpan(timeoutQuotas.ReceiveTimeout);
            cb.CloseTimeout = new TimeSpan(timeoutQuotas.CloseTimeout);
            cb.OpenTimeout = new TimeSpan(timeoutQuotas.OpenTimeout);
            cb.SendTimeout = new TimeSpan(timeoutQuotas.SendTimeout);
            cb.CreateBindingElements();
            return cb;
        }

        public static System.ServiceModel.Channels.Binding BuildHttpBinding()
        {

            MtomMessageEncodingBindingElement mtomEncodingBindingElement =
                new MtomMessageEncodingBindingElement(MessageVersion.Soap12WSAddressing10, System.Text.Encoding.UTF8);

            HttpTransportBindingElement transportBindingElement = new HttpTransportBindingElement();


            BindingElementCollection bindingElementCollection = new BindingElementCollection();
            bindingElementCollection.Add(mtomEncodingBindingElement);
            bindingElementCollection.Add(transportBindingElement);

            CustomBinding cb = new CustomBinding(bindingElementCollection);
            cb.CreateBindingElements();
            return cb;
        }

        public static BindingHelpers.BindingSecurity GetSecurityBindingType(string url)
        {
            if (url.StartsWith("https://"))
            {
                return BindingHelpers.BindingSecurity.TLS;
            }
            return BindingHelpers.BindingSecurity.None;
        }

        public static BasicHttpBinding BuildBasicHTTPBinding()
        {
            return BuildBasicHTTPBinding(BindingSecurity.None, MessageSecurityType.None, WSMessageEncoding.Text, null, null);
        }
        public static BasicHttpBinding BuildBasicHTTPBinding(BindingSecurity bindingSecurity, MessageSecurityType messageSecurity,
            WSMessageEncoding encoding, TimeoutQuotas timeoutQuotas, MessageSizeQuotas sizeQuotas)
        {
            if (sizeQuotas == null)
            {
                sizeQuotas = new MessageSizeQuotas()
                {
                    MaxBufferPoolSize = 15000000,
                    MaxReceivedMessageSize = 15000000,
                    MaxBufferSize = 15000000
                };
            }

            if (timeoutQuotas == null)
            {
                timeoutQuotas = new TimeoutQuotas()
                {
                    CloseTimeout = 600000000, // 1 minute
                    OpenTimeout = 600000000, // 1 minute
                    ReceiveTimeout = 3000000000, // 5 minutes
                    SendTimeout = 3000000000, // 5 minutes
                };
            }

            BasicHttpBinding binding = new BasicHttpBinding();
            binding.Name = "BasicHttpBinding_IPrimeSuiteService";
            binding.CloseTimeout = new TimeSpan(timeoutQuotas.CloseTimeout);
            binding.OpenTimeout = new TimeSpan(timeoutQuotas.OpenTimeout);
            binding.ReceiveTimeout = new TimeSpan(timeoutQuotas.ReceiveTimeout);
            binding.SendTimeout = new TimeSpan(timeoutQuotas.SendTimeout);
            binding.AllowCookies = false;
            binding.BypassProxyOnLocal = false;
            binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            binding.MaxBufferSize = sizeQuotas.MaxBufferSize;
            binding.MaxBufferPoolSize = sizeQuotas.MaxBufferPoolSize;
            binding.MaxReceivedMessageSize = sizeQuotas.MaxReceivedMessageSize;
            binding.MessageEncoding = WSMessageEncoding.Text;
            binding.TextEncoding = System.Text.Encoding.UTF8;
            binding.TransferMode = TransferMode.Buffered;
            binding.UseDefaultWebProxy = true;
            binding.ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas();
            binding.ReaderQuotas.MaxDepth = 32;
            binding.ReaderQuotas.MaxStringContentLength = 8192;
            binding.ReaderQuotas.MaxArrayLength = 16384;
            binding.ReaderQuotas.MaxBytesPerRead = 4096;
            binding.ReaderQuotas.MaxNameTableCharCount = 1638400;
            binding.Security = new System.ServiceModel.BasicHttpSecurity();
            binding.Security.Transport = new HttpTransportSecurity();

            switch (bindingSecurity)
            {
                case BindingSecurity.None:
                    binding.Security.Mode = BasicHttpSecurityMode.None;
                    binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                    binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
                    binding.Security.Transport.Realm = String.Empty;
                    //binding_Text.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
                    //binding_Text.Security.Message.NegotiateServiceCredential = true;
                    //binding_Text.Security.Message.EstablishSecurityContext = true;
                    break;
                case BindingSecurity.TLSWithCertificate:
                    binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
                    binding.Security.Mode = BasicHttpSecurityMode.Transport;
                    binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.Basic;
                    binding.Security.Transport.Realm = String.Empty;
                    break;
                case BindingSecurity.TLS:
                    binding.Security.Mode = BasicHttpSecurityMode.Transport;
                    binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.Basic;
                    binding.Security.Transport.Realm = String.Empty;
                    break;
                default:
                    break;
            }

            switch (messageSecurity)
            {
                case MessageSecurityType.None:
                    binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                    binding.Security.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.Default;
                    break;
                case MessageSecurityType.CertificateOverTransport:
                    binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.Certificate;
                    binding.Security.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.Default;
                    break;
                case MessageSecurityType.MutualCertificate:
                    binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.Certificate;
                    binding.Security.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.Default;
                    break;
                default:
                    break;
            }
            return binding;
        }

        public static WSHttpBinding BuildWSHttpBinding(BindingSecurity bindingSecurity, MessageSecurityType messageSecurity, 
            WSMessageEncoding encoding, TimeoutQuotas timeoutQuotas, MessageSizeQuotas quotas)
        {
            if (quotas == null)
            {
                quotas = new MessageSizeQuotas()
                {
                    MaxBufferPoolSize = 15000000,
                    MaxReceivedMessageSize = 15000000
                };
            }

            if (timeoutQuotas == null)
            {
                timeoutQuotas = new TimeoutQuotas()
                {
                    CloseTimeout = 600000000, // 1 minute
                    OpenTimeout = 600000000, // 1 minute
                    ReceiveTimeout = 3000000000, // 5 minutes
                    SendTimeout = 3000000000, // 5 minutes
                };
            }

            WSHttpBinding binding_Text = new WSHttpBinding();
            binding_Text.Name = "WSHttpBinding_" + encoding.ToString() + "_" + bindingSecurity.ToString();
            binding_Text.CloseTimeout = new TimeSpan(timeoutQuotas.CloseTimeout);
            binding_Text.OpenTimeout = new TimeSpan(timeoutQuotas.OpenTimeout);
            binding_Text.ReceiveTimeout = new TimeSpan(timeoutQuotas.ReceiveTimeout);
            binding_Text.SendTimeout = new TimeSpan(timeoutQuotas.SendTimeout);
            binding_Text.BypassProxyOnLocal = false;
            binding_Text.TransactionFlow = false;
            binding_Text.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            binding_Text.MaxBufferPoolSize = quotas.MaxBufferPoolSize;
            binding_Text.MaxReceivedMessageSize = quotas.MaxReceivedMessageSize;
            binding_Text.MessageEncoding = encoding;
            binding_Text.TextEncoding = System.Text.Encoding.UTF8;
            binding_Text.UseDefaultWebProxy = true;
            binding_Text.AllowCookies = false;
            binding_Text.ReaderQuotas = new XmlDictionaryReaderQuotas();
            binding_Text.ReaderQuotas.MaxDepth = 32;
            binding_Text.ReaderQuotas.MaxStringContentLength = 81920;
            binding_Text.ReaderQuotas.MaxArrayLength = 16384;
            binding_Text.ReaderQuotas.MaxBytesPerRead = 32768;
            binding_Text.ReaderQuotas.MaxNameTableCharCount = 16384000;
            System.ServiceModel.Channels.ReliableSessionBindingElement reliableSessionBindingElement = new System.ServiceModel.Channels.ReliableSessionBindingElement(true);
            reliableSessionBindingElement.InactivityTimeout = new TimeSpan(6000000000); //10 minutes
            //binding_Text.ReliableSession = new OptionalReliableSession(reliableSessionBindingElement);
            switch (bindingSecurity)
            {
                case BindingSecurity.None:
                    binding_Text.Security.Mode = SecurityMode.None;
                    binding_Text.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                    binding_Text.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
                    binding_Text.Security.Transport.Realm = String.Empty;
                    //binding_Text.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
                    //binding_Text.Security.Message.NegotiateServiceCredential = true;
                    //binding_Text.Security.Message.EstablishSecurityContext = true;
                    break;
                case BindingSecurity.TLSWithCertificate:
                    binding_Text.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
                    binding_Text.Security.Mode = SecurityMode.Transport;
                    binding_Text.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.Basic;
                    binding_Text.Security.Transport.Realm = String.Empty;
                    break;
                case BindingSecurity.TLS:
                    binding_Text.Security.Mode = SecurityMode.Transport;
                    binding_Text.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.Basic;
                    binding_Text.Security.Transport.Realm = String.Empty;
                    break;
                case BindingSecurity.Credentials:
                    binding_Text.Security.Mode = SecurityMode.TransportWithMessageCredential;
                    binding_Text.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
                    break;
                default:
                    break;
            }
            switch (messageSecurity)
            {
                case MessageSecurityType.None:
                    break;
                case MessageSecurityType.CertificateOverTransport:
                    binding_Text.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;
                    binding_Text.Security.Message.NegotiateServiceCredential = true;
                    binding_Text.Security.Message.EstablishSecurityContext = true;
                    break;
                case MessageSecurityType.MutualCertificate:
                    binding_Text.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;
                    binding_Text.Security.Message.NegotiateServiceCredential = true;
                    binding_Text.Security.Message.EstablishSecurityContext = true;
                    break;
                default:
                    break;
            }
            return binding_Text;
        }

        public static string UriEncodeIpAddress(string ip)
        {
            System.Net.IPAddress ipAddress = System.Net.IPAddress.Parse(ip);
            if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                ip = "[" + ip + "]";
            }
            return ip;
        }

        public static string TranslateIpv6ToIpv4(string ipv6)
        {
            System.Net.IPAddress ipAddress = System.Net.IPAddress.Parse(ipv6);
            try
            {
                //This next line will call out to the DNS server via sockets. I have opted for not using this method due
                //to errors from doing this operation but leaving this method here for reference. 
                //Using UriEncodeIpAddress seems to do the job without the need to call out to a DNS server
                System.Net.IPHostEntry ipHostEntry = System.Net.Dns.GetHostEntry(ipAddress);
                foreach (System.Net.IPAddress address in ipHostEntry.AddressList)
                {
                    if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        return address.ToString();
                }
            }
            catch
            {
            }
            return ipv6;
        }
    }
}
