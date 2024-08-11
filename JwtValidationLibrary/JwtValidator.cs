using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JwtValidationLibrary
{
    public class JwtValidator
    {
        private readonly string _issuer;
        private readonly string _audience;
        private readonly RSA _rsa;

        public JwtValidator(string issuer, string audience, string token, string userid, string entityid)
        {
            _issuer = issuer;
            _audience = audience;
            _rsa = PublicKeyExtractor.ExtractPublicKeyFromCert(token, userid, entityid);
        }


        public bool IsValidToken(string token, string userid, string entityid)
        {

            var stopwatch = Stopwatch.StartNew();
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,

                    ValidateAudience = true,
                    ValidAudience = _audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new RsaSecurityKey(_rsa),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

            
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                if (validatedToken is JwtSecurityToken jwtToken)
                {
                    if (jwtToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha256, StringComparison.InvariantCultureIgnoreCase))
                    {
                        bool isValid = ValidateClaims(principal, "userid", userid) &&
                                       ValidateClaims(principal, "entityid", entityid);

                        Logger.LogInfo("Token validation successful.", stopwatch, token, userid, entityid);
                        return isValid;
                    }
                }
                Logger.LogInfo("Token validation failed: Invalid token structure or algorithm.", stopwatch, token, userid, entityid);
                return false;
            }
            catch(Exception ex)
            {
                Logger.LogError("Token validation failed.", ex, stopwatch, token, userid, entityid);
                return false;
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        private bool ValidateClaims(ClaimsPrincipal principal, string claimType, string expectedValue)
        {
            var claim = principal.FindFirst(claimType);
            return claim != null && claim.Value.Equals(expectedValue, StringComparison.OrdinalIgnoreCase);
        }

    }
}
