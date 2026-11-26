using Npgsql;

namespace MeuSiteEmMVC.Services
{
    /// <summary>
    /// Converte a connection string do Supabase para o formato do Npgsql.
    /// O Supabase entrega no formato URI: postgresql://usuario:senha@host:5432/postgres
    /// </summary>
    public static class DatabaseUrl
    {
        // Usada só para o app subir quando o banco ainda não foi configurado.
        public const string Placeholder = "Host=localhost;Database=postgres;Username=postgres";

        public static string? Resolve(IConfiguration configuration)
        {
            var raw = configuration["DATABASE_URL"] ?? configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(raw))
            {
                return null;
            }

            raw = raw.Trim();
            return raw.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)
                || raw.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase)
                ? FromUri(raw)
                : raw;
        }

        private static string FromUri(string url)
        {
            var uri = new Uri(url);
            var userInfo = uri.UserInfo.Split(':', 2);

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = uri.Host,
                Port = uri.Port > 0 ? uri.Port : 5432,
                Database = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/')) is { Length: > 0 } db ? db : "postgres",
                Username = Uri.UnescapeDataString(userInfo[0]),
                Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : null,
                SslMode = SslMode.Require,
            };

            // Porta 6543 = pooler do Supabase em modo transação: não suporta reset de sessão.
            if (builder.Port == 6543)
            {
                builder.NoResetOnClose = true;
            }

            // Parâmetros extras da URI (ex.: ?sslmode=require) sobrescrevem os padrões.
            var query = uri.Query.TrimStart('?');
            if (query.Length > 0)
            {
                foreach (var pair in query.Split('&', StringSplitOptions.RemoveEmptyEntries))
                {
                    var kv = pair.Split('=', 2);
                    var key = Uri.UnescapeDataString(kv[0]);
                    var value = kv.Length > 1 ? Uri.UnescapeDataString(kv[1]) : string.Empty;
                    try
                    {
                        builder[key] = value;
                    }
                    catch (ArgumentException)
                    {
                        // Ignora parâmetros que o Npgsql não conhece (ex.: pgbouncer=true).
                    }
                }
            }

            return builder.ConnectionString;
        }
    }
}
