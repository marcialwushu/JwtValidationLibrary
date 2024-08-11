# ADR 001: Arquitetura e Design da JwtValidationLibrary

## Status

Decisão Tomada

## Contexto

A **JwtValidationLibrary** foi criada para atender à necessidade de validação segura e eficiente de JSON Web Tokens (JWT) em aplicações .NET. O projeto busca oferecer uma solução robusta que permita a verificação de assinaturas, emissores, público-alvo, e claims de tokens JWT, além de registrar logs detalhados para monitoramento e depuração. A necessidade de uma validação de tokens segura e auditável foi o principal motivador para o desenvolvimento desta biblioteca.

## Decisão

### Estrutura da Biblioteca

1. **Validação de JWT**:
   - A biblioteca utiliza o `JwtSecurityTokenHandler` do .NET para realizar a validação de tokens JWT.
   - São validadas a assinatura do token, o emissor (`iss`), o público-alvo (`aud`), e a validade temporal (`exp`).
   - A assinatura é verificada utilizando uma chave pública RSA, extraída de um certificado incorporado ao assembly.

2. **Extração de Chave Pública**:
   - A classe `PublicKeyExtractor` é responsável por extrair a chave pública de um certificado X.509 incorporado no assembly da aplicação.
   - O método `ExtractPublicKeyFromCert` é utilizado para carregar o certificado e obter a chave pública RSA, que é então usada para a validação da assinatura do token.

3. **Logging Estruturado**:
   - A biblioteca inclui uma classe `Logger`, que registra logs estruturados em formato JSON.
   - Os logs são enviados para um servidor ElasticSearch, facilitando a indexação e análise.
   - Os logs incluem informações detalhadas como tempo de execução, stack trace, nome da máquina, e contexto do token (por exemplo, `userid` e `entityid`).

### Detalhes Técnicos

- **Tecnologias Utilizadas**:
  - .NET 8.0
  - ElasticSearch para armazenamento e consulta de logs
  - `System.IdentityModel.Tokens.Jwt` para manipulação e validação de tokens JWT
  - `System.Security.Cryptography` para operações criptográficas

- **Componentes Principais**:
  - **JwtValidator**: Classe principal para validação de tokens JWT.
  - **PublicKeyExtractor**: Classe para extração da chave pública do certificado.
  - **Logger**: Classe para registrar logs estruturados e enviá-los ao ElasticSearch.

- **Segurança**:
  - A chave pública RSA é carregada de um certificado incorporado ao assembly, o que impede a necessidade de armazená-la ou recuperá-la de locais inseguros.
  - A validade dos tokens é rigorosamente verificada, garantindo que tokens expirados ou malformados sejam rejeitados.

### Melhoria de Performance

- **ClockSkew**:
  - A configuração `ClockSkew` foi ajustada para `TimeSpan.Zero`, eliminando qualquer margem de erro na validação do tempo de vida do token, garantindo a precisão da validade temporal.

### Auditoria e Monitoramento

- **ElasticSearch**:
  - Logs detalhados são enviados para o ElasticSearch, permitindo que os eventos de validação sejam facilmente auditados e monitorados usando ferramentas como Kibana.

## Consequências

### Vantagens

- **Segurança**: A utilização de chaves públicas RSA e a validação rigorosa de tokens garantem um alto nível de segurança.
- **Auditabilidade**: A capacidade de enviar logs estruturados para o ElasticSearch permite auditoria detalhada e monitoramento contínuo.
- **Facilidade de Integração**: A biblioteca pode ser facilmente integrada em qualquer aplicação .NET que precise de validação de JWT.

### Desvantagens

- **Complexidade**: A necessidade de configurar e manter um servidor ElasticSearch para o armazenamento de logs pode adicionar complexidade ao ambiente de produção.
- **Performance**: A validação de tokens, especialmente com a verificação da assinatura RSA, pode introduzir alguma latência, especialmente em ambientes de alta carga.

## Melhorias Futuras

1. **Suporte a Outros Algoritmos de Assinatura**:
   - Expandir a biblioteca para suportar outros algoritmos de assinatura além do RSA (por exemplo, ECDSA).

2. **Cache de Certificados**:
   - Implementar um mecanismo de cache para certificados carregados, reduzindo a necessidade de ler o certificado do assembly a cada validação de token.

3. **Suporte a Outros Destinos de Logs**:
   - Adicionar suporte para outros destinos de logs além do ElasticSearch, como bancos de dados SQL, serviços de log baseados em nuvem (por exemplo, AWS CloudWatch), ou sistemas de arquivos.

4. **Melhoria na Configuração**:
   - Facilitar a configuração da biblioteca através de arquivos de configuração externos, como `appsettings.json`, para tornar a personalização mais acessível sem necessidade de alterar o código.

5. **Documentação e Exemplos Avançados**:
   - Expandir a documentação com exemplos avançados, incluindo integração com outras bibliotecas e frameworks, além de cenários de uso específicos.

## Registro de Decisões

A decisão de seguir com a arquitetura atual foi tomada após considerar a necessidade de segurança, auditabilidade, e facilidade de integração com aplicações existentes. As melhorias futuras foram identificadas com base em feedback e considerações de desempenho e usabilidade.

---

## Autor

**[CLEILSON DE SOUSA PEREIRA]**

Desenvolvedor de software especializado em segurança e autenticação de tokens JWT. Entre em contato para consultas ou contribuições através de [contato@email.com](mailto:seu-email@example.com).

---

Este documento serve como um registro das principais decisões arquiteturais para a **JwtValidationLibrary** e orienta futuras iterações e melhorias no projeto.
