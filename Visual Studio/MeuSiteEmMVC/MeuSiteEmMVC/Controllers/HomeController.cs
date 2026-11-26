using MeuSiteEmMVC.Data;
using MeuSiteEmMVC.Models;
using MeuSiteEmMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Diagnostics;

namespace MeuSiteEmMVC.Controllers
{
    public class HomeController : Controller
    {
        private static readonly Dictionary<string, string> TiposFoto = new(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".png"] = "image/png",
            [".webp"] = "image/webp",
        };

        private static readonly Dictionary<string, string> TiposDocumento = new(TiposFoto, StringComparer.OrdinalIgnoreCase)
        {
            [".pdf"] = "application/pdf",
        };

        private readonly AppDbContext _db;
        private readonly IFileStorage _storage;
        private readonly ILogger<HomeController> _logger;

        public HomeController(AppDbContext db, IFileStorage storage, ILogger<HomeController> logger)
        {
            _db = db;
            _storage = storage;
            _logger = logger;
        }

        public IActionResult Principal()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Cadastro()
        {
            return View(new CadastroViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(12 * 1024 * 1024)]
        public async Task<IActionResult> Cadastro(CadastroViewModel model, CancellationToken ct)
        {
            ValidarArquivo(model.Foto, nameof(model.Foto), TiposFoto, "JPG, PNG ou WEBP");
            ValidarArquivo(model.DocumentoMedico, nameof(model.DocumentoMedico), TiposDocumento, "PDF, JPG, PNG ou WEBP");

            if (model.DataNascimento is { } nascimento && nascimento > DateOnly.FromDateTime(DateTime.Today))
            {
                ModelState.AddModelError(nameof(model.DataNascimento), "A data de nascimento não pode ser no futuro.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!_storage.IsConfigured)
            {
                _logger.LogError("Cadastro recebido, mas o Supabase Storage não está configurado.");
                ModelState.AddModelError(string.Empty, "O cadastro está temporariamente indisponível. Tente novamente mais tarde.");
                return View(model);
            }

            var cpf = CadastroViewModel.SomenteDigitos(model.Cpf);
            if (await _db.Pedestres.AnyAsync(p => p.Cpf == cpf, ct))
            {
                ModelState.AddModelError(nameof(model.Cpf), "Já existe um cadastro com este CPF.");
                return View(model);
            }

            // Pasta aleatória: o nome do arquivo não expõe o CPF nem o nome do pedestre.
            var pasta = Guid.NewGuid().ToString("N");
            var fotoPath = $"{pasta}/foto{Path.GetExtension(model.Foto!.FileName).ToLowerInvariant()}";
            var docPath = $"{pasta}/documento-medico{Path.GetExtension(model.DocumentoMedico!.FileName).ToLowerInvariant()}";

            try
            {
                await _storage.UploadAsync(fotoPath, model.Foto, ct);
                await _storage.UploadAsync(docPath, model.DocumentoMedico, ct);

                _db.Pedestres.Add(new Pedestre
                {
                    Nome = model.Nome.Trim(),
                    Cpf = cpf,
                    DataNascimento = model.DataNascimento!.Value,
                    Telefone = string.IsNullOrWhiteSpace(model.Telefone) ? null : CadastroViewModel.SomenteDigitos(model.Telefone),
                    Cep = CadastroViewModel.SomenteDigitos(model.Cep),
                    Uf = model.Uf,
                    Cidade = model.Cidade.Trim(),
                    Bairro = model.Bairro.Trim(),
                    Rua = model.Rua.Trim(),
                    NumeroCasa = model.NumeroCasa?.Trim(),
                    Complemento = model.Complemento?.Trim(),
                    NomeCondicao = model.NomeCondicao.Trim(),
                    TempoCondicao = model.TempoCondicao,
                    FotoPath = fotoPath,
                    DocMedicoPath = docPath,
                });
                await _db.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao salvar o cadastro.");
                await _storage.DeleteAsync(new[] { fotoPath, docPath }, CancellationToken.None);

                var cpfDuplicado = ex is DbUpdateException { InnerException: PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } };
                ModelState.AddModelError(cpfDuplicado ? nameof(model.Cpf) : string.Empty,
                    cpfDuplicado ? "Já existe um cadastro com este CPF." : "Não foi possível salvar o cadastro agora. Tente novamente em alguns minutos.");
                return View(model);
            }

            TempData["CadastroOk"] = "Cadastro enviado com sucesso! Entraremos em contato sobre a entrega do cartão.";
            return RedirectToAction(nameof(Cadastro));
        }

        public IActionResult Simulacao()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void ValidarArquivo(IFormFile? arquivo, string campo, IReadOnlyDictionary<string, string> tipos, string descricao)
        {
            if (arquivo is null)
            {
                return; // [Required] já mostra a mensagem.
            }

            if (arquivo.Length == 0 || arquivo.Length > CadastroViewModel.TamanhoMaximoArquivo)
            {
                ModelState.AddModelError(campo, "O arquivo deve ter até 5 MB.");
                return;
            }

            var extensao = Path.GetExtension(arquivo.FileName);
            if (!tipos.TryGetValue(extensao, out var tipoEsperado)
                || !string.Equals(arquivo.ContentType, tipoEsperado, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(campo, $"Formato não aceito. Envie {descricao}.");
            }
        }
    }
}
