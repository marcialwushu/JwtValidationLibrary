using Nest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwtValidationLibrary
{
    public static class Logger
    {
        private static readonly ElasticClient _client;

        static Logger()
        {
            var settings = new ConnectionSettings(new Uri("http://localhost:9200")) // Substitua pela URL do seu ElasticSearch
                .DefaultIndex("jwt-logs");

            _client = new ElasticClient(settings);
        }

        public static void LogError(string message, Exception ex, Stopwatch stopwatch, string token, string userid, string entityid)
        {
            var logEntry = new
            {
                Timestamp = DateTime.UtcNow,
                Level = "Error",
                Message = message,
                Exception = ex.ToString(),
                StackTrace = ex.StackTrace,
                RequestDuration = stopwatch.ElapsedMilliseconds,
                MachineName = Environment.MachineName,
                Token = token,
                UserId = userid,
                EntityId = entityid
            };

            _client.IndexDocument(logEntry);
        }

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
