using Nest;
using System.Diagnostics;

namespace JwtValidationLibrary
{
    /// <summary>
    /// A classe <c>Logger</c> fornece funcionalidades para registrar logs estruturados e enviá-los ao ElasticSearch.
    /// Esta classe suporta o registro de logs de erros e logs informativos, incluindo detalhes como tempo de execução,
    /// stack trace, nome da máquina, e informações associadas ao token JWT.
    /// </summary>
    public static class Logger
    {
        private static readonly ElasticClient _client;

        /// <summary>
        /// Bloco estático que inicializa a configuração do cliente ElasticSearch com o índice padrão "jwt-logs".
        /// Substitua a URL do ElasticSearch conforme necessário.
        /// </summary>
        static Logger()
        {
            var settings = new ConnectionSettings(new Uri("http://localhost:9200")) // Substitua pela URL do seu ElasticSearch
                .DefaultIndex("jwt-logs");

            _client = new ElasticClient(settings);
        }

        /// <summary>
        /// Registra um log de erro no ElasticSearch.
        /// </summary>
        /// <param name="message">A mensagem descritiva do erro.</param>
        /// <param name="ex">A exceção que foi lançada durante a execução.</param>
        /// <param name="stopwatch">O <see cref="Stopwatch"/> que mede o tempo de execução da operação.</param>
        /// <param name="token">O token JWT associado à operação.</param>
        /// <param name="userid">O identificador do usuário associado à operação.</param>
        /// <param name="entityid">O identificador da entidade associado à operação.</param>
        /// <remarks>
        /// Este método cria um objeto log contendo informações detalhadas sobre a exceção,
        /// incluindo a mensagem de erro, stack trace, duração da requisição, nome da máquina onde o erro ocorreu,
        /// e informações contextuais como token, userid, e entityid. O log é então enviado ao ElasticSearch.
        /// </remarks>
        public static void LogError(string message, Exception ex, Stopwatch stopwatch, string token, string userid, string entityid)
        {
            // Nota: A estrutura de log abaixo foi projetada para capturar informações essenciais
            // durante a validação do JWT. Cada campo no log fornece detalhes importantes
            // que ajudam na rastreabilidade e na análise de erros.
            var logEntry = new
            {
                // Timestamp: Captura a data e hora atuais em UTC no momento em que o log é criado.
                // Isso é crucial para ordenar e correlacionar eventos em sistemas distribuídos.
                Timestamp = DateTime.UtcNow,

                // Level: Indica o nível de severidade do log. Neste caso, "Error" é usado para 
                // destacar que ocorreu uma falha durante a execução.
                Level = "Error",

                // Message: Contém uma descrição textual do erro ou evento, fornecendo 
                // contexto sobre o que deu errado.
                Message = message,

                // Exception: Serializa a exceção lançada, incluindo a mensagem de erro 
                // e qualquer informação adicional relacionada à falha.
                Exception = ex.ToString(),

                // StackTrace: Armazena o stack trace associado à exceção, permitindo
                // uma análise detalhada de onde e por que o erro ocorreu no código.
                StackTrace = ex.StackTrace,

                // RequestDuration: Mede a duração da operação, em milissegundos, desde
                // o início até o momento do erro. Isso é útil para identificar 
                // gargalos de desempenho e operações que falham após longas execuções.
                RequestDuration = stopwatch.ElapsedMilliseconds,

                // MachineName: Captura o nome da máquina onde o código foi executado,
                // o que é vital em ambientes distribuídos para rastrear onde o problema ocorreu.
                MachineName = Environment.MachineName,

                // Token: Armazena o token JWT relacionado à operação. Isso é útil para 
                // auditoria e para correlacionar o log com eventos específicos de autenticação.
                Token = token,

                // UserId: Captura o identificador do usuário relacionado ao token JWT,
                // permitindo a identificação de qual usuário estava associado ao erro.
                UserId = userid,

                // EntityId: Registra o identificador da entidade relacionada ao token JWT,
                // ajudando a rastrear o contexto exato da operação que falhou.
                EntityId = entityid
            };

            // Envia o log estruturado para o ElasticSearch. Isso permite a indexação e
            // consulta eficiente dos logs, facilitando a análise posterior através de 
            // ferramentas como Kibana.
            _client.IndexDocument(logEntry);
        }

        /// <summary>
        /// Registra um log informativo no ElasticSearch.
        /// </summary>
        /// <param name="message">A mensagem descritiva do evento informativo.</param>
        /// <param name="stopwatch">O <see cref="Stopwatch"/> que mede o tempo de execução da operação.</param>
        /// <param name="token">O token JWT associado à operação.</param>
        /// <param name="userid">O identificador do usuário associado à operação.</param>
        /// <param name="entityid">O identificador da entidade associado à operação.</param>
        /// <remarks>
        /// Este método cria um objeto log contendo informações sobre a operação bem-sucedida,
        /// incluindo a mensagem informativa, duração da requisição, nome da máquina onde a operação ocorreu,
        /// e informações contextuais como token, userid, e entityid. O log é então enviado ao ElasticSearch.
        /// </remarks>
        public static void LogInfo(string message, Stopwatch stopwatch, string token, string userid, string entityid)
        {
            var logEntry = new
            {
                Timestamp = DateTime.UtcNow,
                Level = "Info",
                Message = message,
                RequestDuration = stopwatch.ElapsedMilliseconds,
                MachineName = Environment.MachineName,
                Token = token,
                UserId = userid,
                EntityId = entityid
            };

            _client.IndexDocument(logEntry);
        }
    }
}
