using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace JwtValidationLibrary
{
    /// <summary>
    /// A classe <c>PublicKeyExtractor</c> é responsável por extrair a chave pública de um certificado incorporado ao assembly.
    /// </summary>
    public static class PublicKeyExtractor
    {
        private const string CertResourceName = "JwtValidationLibrary.Resources.MyCertificate.crt";

        /// <summary>
        /// Extrai a chave pública de um certificado X.509 incorporado ao assembly.
        /// </summary>
        /// <param name="token">O token JWT associado à operação de extração.</param>
        /// <param name="userid">O identificador do usuário associado à operação.</param>
        /// <param name="entityid">O identificador da entidade associado à operação.</param>
        /// <returns>Retorna a chave pública extraída do certificado.</returns>
        /// <exception cref="InvalidOperationException">Lançada quando o recurso do certificado não é encontrado no assembly.</exception>
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

        /// <summary>
        /// Lê completamente um fluxo de dados (stream) e converte-o em um array de bytes.
        /// </summary>
        /// <param name="input">O fluxo de dados a ser lido.</param>
        /// <param name="token">O token JWT associado à operação de leitura.</param>
        /// <param name="userid">O identificador do usuário associado à operação.</param>
        /// <param name="entityid">O identificador da entidade associado à operação.</param>
        /// <returns>Retorna o conteúdo do fluxo de dados como um array de bytes.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro durante a leitura do fluxo de dados.</exception>
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
