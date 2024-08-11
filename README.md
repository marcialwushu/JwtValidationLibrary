
# JwtValidationLibrary

## Descrição

**JwtValidationLibrary** é uma biblioteca em .NET projetada para validar JSON Web Tokens (JWT) de maneira segura e eficiente. A biblioteca oferece uma série de funcionalidades que permitem a validação de assinaturas, emissores, público-alvo, e claims de tokens JWT utilizando chaves públicas extraídas de certificados incorporados no assembly. Além disso, a biblioteca inclui capacidades de logging estruturado, registrando informações detalhadas sobre o processo de validação e erros, com suporte para envio de logs ao ElasticSearch.

## Funcionalidades

- **Validação de JWT**: Verifica a assinatura, emissor, público-alvo, e claims.
- **Extração de Chave Pública**: Obtém chaves públicas de certificados incorporados.
- **Logging Estruturado**: Registra logs detalhados com informações como tempo de execução, erros, e contexto da validação.
- **Suporte ao ElasticSearch**: Envia logs estruturados para o ElasticSearch para análise e monitoramento.

## Requisitos

- .NET 6.0 ou superior
- ElasticSearch (opcional, para registro de logs)

## Instalação

Para instalar e usar a **JwtValidationLibrary**, siga os passos abaixo:

1. Clone o repositório para sua máquina local:

   ```bash
   git clone https://github.com/seu-usuario/JwtValidationLibrary.git
   ```

2. Navegue até o diretório do projeto:

   ```bash
   cd JwtValidationLibrary
   ```

3. Compile o projeto:

   ```bash
   dotnet build
   ```

4. Adicione a referência à biblioteca em seu projeto .NET:

   ```bash
   dotnet add reference ../JwtValidationLibrary/JwtValidationLibrary.csproj
   ```

## Uso

Aqui está um exemplo básico de como usar a `JwtValidator` para validar um token JWT:

```csharp
using JwtValidationLibrary;

class Program
{
    static void Main(string[] args)
    {
        // Configurações do JWT
        string issuer = "https://your-issuer.com";
        string audience = "your-audience";
        string token = "your-jwt-token";
        string userid = "user123";
        string entityid = "entity456";

        // Inicializa o validador de JWT
        var jwtValidator = new JwtValidator(issuer, audience, token, userid, entityid);

        // Valida o token
        bool isValid = jwtValidator.IsValidToken(token, userid, entityid);

        // Resultado da validação
        Console.WriteLine(isValid ? "Token is valid." : "Token is invalid.");
    }
}
```

## Configuração

### Configuração do ElasticSearch

Por padrão, os logs são enviados ao ElasticSearch usando as configurações definidas na classe `Logger`. Para alterar a URL do ElasticSearch ou o índice padrão, modifique o construtor estático na classe `Logger.cs`:

```csharp
static Logger()
{
    var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
        .DefaultIndex("jwt-logs");

    _client = new ElasticClient(settings);
}
```

## Estrutura de Logs

Os logs gerados pela biblioteca incluem informações detalhadas, como:

- **Timestamp**: Hora e data do log.
- **Level**: Nível de severidade do log (por exemplo, "Info" ou "Error").
- **Message**: Mensagem descritiva do evento.
- **Exception**: Detalhes da exceção (em caso de erro).
- **StackTrace**: Stack trace do erro.
- **RequestDuration**: Tempo de execução da operação.
- **MachineName**: Nome da máquina onde o log foi gerado.
- **Token**: O token JWT associado à operação.
- **UserId**: O identificador do usuário relacionado.
- **EntityId**: O identificador da entidade relacionada.

## Autor

**[CLEILSON DE SOUSA PEREIRA]**

Desenvolvedor de software com foco em segurança e validação de tokens JWT. Para mais informações ou consultas, entre em contato através do [contato@email.com](mailto:seu-email@example.com).

## Licença

Este projeto é licenciado sob os termos da [Licença MIT](LICENSE). Consulte o arquivo LICENSE para mais detalhes.

---

**Nota**: Este README serve como um guia básico para configurar e utilizar a `JwtValidationLibrary`. Para mais detalhes sobre as funcionalidades avançadas, consulte a documentação completa ou entre em contato com o autor.


### Explicação dos Tópicos no `README.md`

1. **Descrição**: Fornece uma visão geral da biblioteca, suas funcionalidades principais e propósitos.

2. **Funcionalidades**: Lista as principais capacidades da biblioteca, como validação de JWT, extração de chave pública, e logging estruturado.

3. **Requisitos**: Especifica as dependências e requisitos necessários para usar a biblioteca.

4. **Instalação**: Instruções passo a passo para clonar, compilar e adicionar a biblioteca a um projeto .NET.

5. **Uso**: Exemplo de código mostrando como utilizar a biblioteca em um aplicativo.

6. **Configuração**: Explica como configurar o ElasticSearch para o registro de logs.

7. **Estrutura de Logs**: Detalha os campos incluídos nos logs gerados pela biblioteca.

8. **Autor**: Informações sobre o desenvolvedor responsável pela biblioteca.

9. **Licença**: Detalhes sobre a licença sob a qual a biblioteca é distribuída.

Este `README.md` é projetado para ser informativo e acessível, permitindo que os usuários configurem e utilizem a biblioteca com facilidade, enquanto também oferece informações sobre o desenvolvimento e suporte.
