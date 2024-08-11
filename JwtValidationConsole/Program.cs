using JwtValidationLibrary;

namespace JwtValidationConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // Defina os valores do issuer e audience esperados
            string issuer = "https://your-issuer.com";
            string audience = "your-audience";

            // JWT fictício para teste (substitua por um real)
            string token = "your-jwt-token-here";
            string partnerId = "expected-partnerId";
            string businessunitId = "expected-businessunitId";

            // Inicialize o validador de JWT
            var jwtValidator = new JwtValidator(issuer, audience);

            // Valide o token e verifique se é válido
            bool isValid = jwtValidator.IsValidToken(token, partnerId, businessunitId);

            // Exiba o resultado da validação
            if (isValid)
            {
                Console.WriteLine("Token is valid.");
            }
            else
            {
                Console.WriteLine("Token is invalid.");
            }

            // Aguarde a entrada do usuário antes de fechar o console
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
