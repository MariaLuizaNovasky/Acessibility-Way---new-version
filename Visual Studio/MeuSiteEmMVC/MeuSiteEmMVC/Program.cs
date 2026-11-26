using MeuSiteEmMVC.Data;
using MeuSiteEmMVC.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// O Render (e outros hosts) informam a porta pela variável PORT.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

builder.Services.AddControllersWithViews();

// Banco de dados: PostgreSQL do Supabase.
// Aceita DATABASE_URL (formato URI do Supabase) ou ConnectionStrings:DefaultConnection.
var connectionString = DatabaseUrl.Resolve(builder.Configuration);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString ?? DatabaseUrl.Placeholder));

// Armazenamento dos arquivos (foto e documento médico) no Supabase Storage.
builder.Services.Configure<SupabaseStorageOptions>(options =>
{
    options.Url = builder.Configuration["SUPABASE_URL"] ?? builder.Configuration["Supabase:Url"];
    options.ServiceRoleKey = builder.Configuration["SUPABASE_SERVICE_ROLE_KEY"] ?? builder.Configuration["Supabase:ServiceRoleKey"];
    options.Bucket = builder.Configuration["SUPABASE_BUCKET"] ?? builder.Configuration["Supabase:Bucket"] ?? SupabaseStorageOptions.DefaultBucket;
});
builder.Services.AddHttpClient<IFileStorage, SupabaseStorageService>();

var app = builder.Build();

if (connectionString is null)
{
    app.Logger.LogWarning("Banco não configurado: defina DATABASE_URL. As páginas abrem, mas o cadastro não será salvo.");
}
else if (!string.Equals(app.Configuration["DATABASE_AUTO_MIGRATE"], "false", StringComparison.OrdinalIgnoreCase))
{
    await DatabaseInitializer.EnsureSchemaAsync(app.Services, app.Logger);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Principal}/{id?}");

app.Run();
