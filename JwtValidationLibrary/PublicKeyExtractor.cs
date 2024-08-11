using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Nest;

namespace JwtValidationLibrary
{
    public static class PublicKeyExtractor
    {
        private const string CertResourceName = "JwtValidationLibrary.Resources.MyCertificate.crt";

        public static RSA ExtractPublicKeyFromCert(string token, string userid, string entityid)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream(CertResourceName))
                {
                    if (stream == null)
                    {
                        var message = $"Resource '{CertResourceName}' not found in assembly.";
                        Logger.LogError(message, new InvalidOperationException(message), stopwatch, token, userid, entityid);
                        throw new InvalidOperationException(message);
                    }

                    var cert = new X509Certificate2(ReadStreamFully(stream, token, userid, entityid));
                    Logger.LogInfo("Certificate loaded successfully.", stopwatch, token, userid, entityid);
                    return cert.GetRSAPublicKey();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to extract public key from certificate.", ex, stopwatch, token, userid, entityid);
                throw; // Rethrow the exception after logging
            }
            finally 
            { 
                stopwatch.Stop(); 
            }
            
        }

        private static byte[] ReadStreamFully(System.IO.Stream input, string token, string userid, string entityid)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    input.CopyTo(ms);
                    Logger.LogInfo("Stream read successfully.", stopwatch, token, userid, entityid);
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to read stream fully.", ex, stopwatch, token, userid, entityid);
                throw; // Rethrow the exception after logging
            }
            finally
            {
                stopwatch.Stop();
            }
            
        }
    }
}
