using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace QLicense
{
    /// <summary>
    ///     Usage Guide:
    ///     Command for creating the certificate
    ///     >> makecert -pe -ss My -sr CurrentUser -$ commercial -n "CN=
#pragma warning disable 1570
    ///     <YourCertName>
    ///         " -sky Signature
    ///         Then export the cert with private key from key store with a password
    ///         Also export another cert with only public key
    /// </summary>
#pragma warning restore 1570
    public class LicenseHandler
    {
        public static string GenerateUid(string appName)
        {
            return HardwareInfo.GenerateUid(appName);
        }

        public static string GenerateLicenseBase64String(LicenseEntity lic, byte[] certPrivateKeyData,
            SecureString certFilePwd)
        {
            //Serialize license object into XML                    
            var licenseObject = new XmlDocument();
            using (var writer = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(LicenseEntity), new[] {lic.GetType()});

                serializer.Serialize(writer, lic);

                licenseObject.LoadXml(writer.ToString());
            }

            //Get RSA key from certificate
            var cert = new X509Certificate2(certPrivateKeyData, certFilePwd);

            var rsaKey = (RSACryptoServiceProvider) cert.PrivateKey;

            //Sign the XML
            SignXml(licenseObject, rsaKey);

            //Convert the signed XML into BASE64 string            
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(licenseObject.OuterXml));
        }


        public static LicenseEntity ParseLicenseFromBase64String(Type licenseObjType, string licenseString,
            byte[] certPubKeyData, out LicenseStatus licStatus, out string validationMsg)
        {
            validationMsg = string.Empty;
            licStatus = LicenseStatus.Undefined;

            if (string.IsNullOrWhiteSpace(licenseString))
            {
                licStatus = LicenseStatus.Cracked;
                return null;
            }

            LicenseEntity lic = null;

            try
            {
                //Get RSA key from certificate
                var cert = new X509Certificate2(certPubKeyData);
                var rsaKey = (RSACryptoServiceProvider) cert.PublicKey.Key;

                var xmlDoc = new XmlDocument {PreserveWhitespace = true};

                // Load an XML file into the XmlDocument object.
                xmlDoc.LoadXml(Encoding.UTF8.GetString(Convert.FromBase64String(licenseString)));

                // Verify the signature of the signed XML.            
                if (VerifyXml(xmlDoc, rsaKey))
                {
                    var nodeList = xmlDoc.GetElementsByTagName("Signature");
                    xmlDoc.DocumentElement.RemoveChild(nodeList[0]);

                    string licXml = xmlDoc.OuterXml;

                    //Deserialize license
                    var serializer = new XmlSerializer(typeof(LicenseEntity), new[] {licenseObjType});
                    using (var reader = new StringReader(licXml))
                    {
                        lic = (LicenseEntity) serializer.Deserialize(reader);
                    }

                    licStatus = lic.DoExtraValidation(out validationMsg);
                }
                else
                {
                    licStatus = LicenseStatus.Invalid;
                }
            }
            catch
            {
                licStatus = LicenseStatus.Cracked;
            }

            return lic;
        }

        // Sign an XML file. 
        // This document cannot be verified unless the verifying 
        // code has the key with which it was signed.
        private static void SignXml(XmlDocument xmlDoc, RSA key)
        {
            // Check arguments.
            if (xmlDoc == null)
            {
                throw new ArgumentException(nameof(xmlDoc));
            }

            // Create a SignedXml object.
            var signedXml = new SignedXml(xmlDoc) {SigningKey = key ?? throw new ArgumentException(nameof(key))};

            // Add the key to the SignedXml document.

            // Create a reference to be signed.
            var reference = new Reference {Uri = ""};

            // Add an enveloped transformation to the reference.
            var env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);

            // Add the reference to the SignedXml object.
            signedXml.AddReference(reference);

            // Compute the signature.
            signedXml.ComputeSignature();

            // Get the XML representation of the signature and save
            // it to an XmlElement object.
            var xmlDigitalSignature = signedXml.GetXml();

            // Append the element to the XML document.
            xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));
        }

        // Verify the signature of an XML file against an asymmetric 
        // algorithm and return the result.
        private static bool VerifyXml(XmlDocument doc, RSA key)
        {
            // Check arguments.
            if (doc == null)
            {
                throw new ArgumentException("Doc");
            }

            if (key == null)
            {
                throw new ArgumentException("Key");
            }

            // Create a new SignedXml object and pass it
            // the XML document class.
            var signedXml = new SignedXml(doc);

            // Find the "Signature" node and create a new
            // XmlNodeList object.
            var nodeList = doc.GetElementsByTagName("Signature");

            // Throw an exception if no signature was found.
            if (nodeList.Count <= 0)
            {
                throw new CryptographicException("Verification failed: No Signature was found in the document.");
            }

            // This example only supports one signature for
            // the entire XML document.  Throw an exception 
            // if more than one signature was found.
            if (nodeList.Count >= 2)
            {
                throw new CryptographicException(
                    "Verification failed: More that one signature was found for the document.");
            }

            // Load the first <signature> node.  
            signedXml.LoadXml((XmlElement) nodeList[0]);

            // Check the signature and return the result.
            return signedXml.CheckSignature(key);
        }

        public static bool ValidateUidFormat(string uid)
        {
            return HardwareInfo.ValidateUidFormat(uid);
        }
    }
}