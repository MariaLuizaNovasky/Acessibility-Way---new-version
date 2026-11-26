using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace MeuSiteEmMVC.Services
{
    public class SupabaseStorageOptions
    {
        public const string DefaultBucket = "documentos-pedestres";

        public string? Url { get; set; }
        public string? ServiceRoleKey { get; set; }
        public string Bucket { get; set; } = DefaultBucket;

        public bool IsConfigured => !string.IsNullOrWhiteSpace(Url) && !string.IsNullOrWhiteSpace(ServiceRoleKey);
    }

    public interface IFileStorage
    {
        bool IsConfigured { get; }
        Task UploadAsync(string path, IFormFile file, CancellationToken ct = default);
        Task DeleteAsync(IEnumerable<string> paths, CancellationToken ct = default);
    }

    /// <summary>
    /// Envia arquivos para um bucket privado do Supabase Storage pela API REST.
    /// A chave service_role fica só no servidor; nunca vai para o navegador.
    /// </summary>
    public class SupabaseStorageService : IFileStorage
    {
        private readonly HttpClient _http;
        private readonly SupabaseStorageOptions _options;

        public SupabaseStorageService(HttpClient http, IOptions<SupabaseStorageOptions> options)
        {
            _http = http;
            _options = options.Value;

            if (_options.IsConfigured)
            {
                _http.BaseAddress = new Uri(_options.Url!.TrimEnd('/') + "/storage/v1/");
                _http.DefaultRequestHeaders.Add("apikey", _options.ServiceRoleKey);
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.ServiceRoleKey);
            }
        }

        public bool IsConfigured => _options.IsConfigured;

        public async Task UploadAsync(string path, IFormFile file, CancellationToken ct = default)
        {
            EnsureConfigured();

            await using var stream = file.OpenReadStream();
            using var content = new StreamContent(stream);
            content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            using var request = new HttpRequestMessage(HttpMethod.Post, $"object/{_options.Bucket}/{path}") { Content = content };
            request.Headers.Add("x-upsert", "false");

            using var response = await _http.SendAsync(request, ct);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                throw new InvalidOperationException($"Supabase Storage recusou o upload ({(int)response.StatusCode}): {body}");
            }
        }

        public async Task DeleteAsync(IEnumerable<string> paths, CancellationToken ct = default)
        {
            if (!IsConfigured)
            {
                return;
            }

            using var request = new HttpRequestMessage(HttpMethod.Delete, $"object/{_options.Bucket}")
            {
                Content = JsonContent.Create(new { prefixes = paths.ToArray() }),
            };
            using var response = await _http.SendAsync(request, ct);
        }

        private void EnsureConfigured()
        {
            if (!IsConfigured)
            {
                throw new InvalidOperationException("Supabase Storage não configurado: defina SUPABASE_URL e SUPABASE_SERVICE_ROLE_KEY.");
            }
        }
    }
}
