/// <summary>
/// <para><b>Biblioteca:</b> JwtValidationLibrary</para>
/// <para><b>Versão:</b> 1.0.0</para>
/// <para><b>Autor:</b> [CLEILSON DE SOUSA PEREIRA]</para>
/// <para><b>Ano:</b> 2024</para>
/// <para><b>Descrição:</b></para>
/// <para>
/// A <c>JwtValidationLibrary</c> é uma biblioteca desenvolvida para realizar a validação
/// de JSON Web Tokens (JWT) em aplicações .NET. Ela fornece funcionalidades para validar
/// a assinatura, emissor, público-alvo, e claims de tokens JWT, utilizando chaves públicas
/// extraídas de certificados incorporados ao assembly.
/// </para>
/// <para>
/// A biblioteca também inclui capacidades de logging estruturado, registrando informações
/// detalhadas sobre o processo de validação, erros ocorridos, e o tempo de execução das
/// operações. Os logs são enviados ao ElasticSearch para facilitar a análise e o monitoramento.
/// </para>
/// <para><b>Licença:</b> [MIT]</para>
/// <para>
/// <b>Nota:</b> Esta biblioteca foi desenvolvida para uso interno em projetos que requerem
/// validação de tokens JWT com alto nível de segurança e rastreabilidade. Para mais informações,
/// consulte a documentação ou entre em contato com o autor.
/// </para>
/// </summary>

using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace JwtValidationLibrary
{
    /// <summary>
    /// A classe <c>JwtValidator</c> valida JSON Web Tokens (JWT) com base em um emissor, público-alvo e uma chave pública RSA extraída de um certificado.
    /// Esta classe também registra logs detalhados do processo de validação, incluindo informações de tempo de execução, erros e resultados.
    /// </summary>
    public class JwtValidator
    {
        private readonly string _issuer;
        private readonly string _audience;
        private readonly RSA _rsa;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="JwtValidator"/> com os parâmetros de configuração fornecidos.
        /// </summary>
        /// <param name="issuer">O emissor esperado do token JWT (claim "iss").</param>
        /// <param name="audience">O público-alvo esperado do token JWT (claim "aud").</param>
        /// <param name="token">O token JWT a ser validado.</param>
        /// <param name="userid">O identificador do usuário associado ao token JWT.</param>
        /// <param name="entityid">O identificador da entidade associada ao token JWT.</param>
        /// <remarks>
        /// O construtor extrai a chave pública RSA de um certificado incorporado ao assembly,
        /// utilizando as informações do token, userid e entityid para fins de log e contexto.
        /// </remarks>
        public JwtValidator(string issuer, string audience, string token, string userid, string entityid)
        {
            _issuer = issuer;
            _audience = audience;
            _rsa = PublicKeyExtractor.ExtractPublicKeyFromCert(token, userid, entityid);
        }

        /// <summary>
        /// Valida um token JWT e verifica se ele contém as claims necessárias, como userid e entityid.
        /// </summary>
        /// <param name="token">O token JWT a ser validado.</param>
        /// <param name="userid">O identificador do usuário a ser validado no token JWT.</param>
        /// <param name="entityid">O identificador da entidade a ser validado no token JWT.</param>
        /// <returns>Retorna <c>true</c> se o token for válido; caso contrário, <c>false</c>.</returns>
        /// <remarks>
        /// Este método valida a assinatura, emissor, público-alvo e o tempo de vida do token JWT.
        /// Além disso, ele verifica se as claims "userid" e "entityid" correspondem aos valores esperados.
        /// Logs detalhados são registrados durante o processo, incluindo sucesso ou falha na validação.
        /// </remarks>
        public bool IsValidToken(string token, string userid, string entityid)
        {
            // Inicia a medição do tempo de execução
            var stopwatch = Stopwatch.StartNew();
            try
            {
                // Cria um manipulador de tokens para processar o JWT
                var tokenHandler = new JwtSecurityTokenHandler();
                // Define os parâmetros de validação do token
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _issuer, // Verifica se o emissor do token corresponde ao esperado

                    ValidateAudience = true,
                    ValidAudience = _audience, // Verifica se o público-alvo do token corresponde ao esperado

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new RsaSecurityKey(_rsa), // Verifica a assinatura do token usando a chave pública RSA

                    ValidateLifetime = true, // Verifica se o token ainda está dentro do seu período de validade
                    ClockSkew = TimeSpan.Zero // Não permite nenhuma margem de erro no tempo de expiração
                };

                // Valida o token e extrai o principal (claims)
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                // Verifica se o token validado é do tipo JWT e se usa o algoritmo esperado
                if (validatedToken is JwtSecurityToken jwtToken)
                {
                    // Verifica se as claims userid e entityid correspondem aos valores esperados
                    if (jwtToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha256, StringComparison.InvariantCultureIgnoreCase))
                    {
                        bool isValid = ValidateClaims(principal, "userid", userid) &&
                                       ValidateClaims(principal, "entityid", entityid);

                        // Log de sucesso na validação do token
                        Logger.LogInfo("Token validation successful.", stopwatch, token, userid, entityid);
                        return isValid;
                    }
                }
                // Log de falha na validação devido a estrutura ou algoritmo inválido
                Logger.LogInfo("Token validation failed: Invalid token structure or algorithm.", stopwatch, token, userid, entityid);
                return false;
            }
            catch (Exception ex)
            {
                // Captura e registra qualquer exceção ocorrida durante a validação do token
                Logger.LogError("Token validation failed.", ex, stopwatch, token, userid, entityid);
                return false;
            }
            finally
            {
                // Para o cronômetro de medição do tempo de execução
                stopwatch.Stop();
            }
        }

        /// <summary>
        /// Verifica se uma claim específica existe no principal e se o seu valor corresponde ao esperado.
        /// </summary>
        /// <param name="principal">O principal que contém as claims do token JWT.</param>
        /// <param name="claimType">O tipo da claim a ser validada (por exemplo, "userid" ou "entityid").</param>
        /// <param name="expectedValue">O valor esperado da claim.</param>
        /// <returns>Retorna <c>true</c> se a claim existir e corresponder ao valor esperado; caso contrário, <c>false</c>.</returns>
        /// <remarks>
        /// Este método é utilizado para validar se as claims necessárias (como "userid" e "entityid")
        /// estão presentes no token JWT e se correspondem aos valores fornecidos.
        /// </remarks>
        private bool ValidateClaims(ClaimsPrincipal principal, string claimType, string expectedValue)
        {
            // Busca a claim pelo tipo especificado
            var claim = principal.FindFirst(claimType);
            // Retorna true se a claim existir e seu valor corresponder ao esperado
            return claim != null && claim.Value.Equals(expectedValue, StringComparison.OrdinalIgnoreCase);
        }

    }
}
