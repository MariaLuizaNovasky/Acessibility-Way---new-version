using Microsoft.EntityFrameworkCore;

namespace MeuSiteEmMVC.Data
{
    /// <summary>
    /// Cria as tabelas no Supabase na primeira execução, usando Data/schema.sql.
    /// </summary>
    public static class DatabaseInitializer
    {
        public static async Task EnsureSchemaAsync(IServiceProvider services, ILogger logger)
        {
            try
            {
                var assembly = typeof(DatabaseInitializer).Assembly;
                var resource = assembly.GetManifestResourceNames().Single(n => n.EndsWith("schema.sql", StringComparison.Ordinal));
                using var stream = assembly.GetManifestResourceStream(resource)!;
                using var reader = new StreamReader(stream);
                var sql = await reader.ReadToEndAsync();

                using var scope = services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await db.Database.ExecuteSqlRawAsync(sql);
                logger.LogInformation("Estrutura do banco verificada.");
            }
            catch (Exception ex)
            {
                // Não derruba o site: as páginas informativas continuam funcionando.
                logger.LogError(ex, "Não foi possível preparar o banco. Confira a DATABASE_URL.");
            }
        }
    }
}
