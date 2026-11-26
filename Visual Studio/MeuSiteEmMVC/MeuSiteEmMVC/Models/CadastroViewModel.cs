using System.ComponentModel.DataAnnotations;

namespace MeuSiteEmMVC.Models
{
    /// <summary>
    /// Dados enviados pelo formulário de cadastro de pedestres.
    /// </summary>
    public class CadastroViewModel
    {
        public const long TamanhoMaximoArquivo = 5 * 1024 * 1024; // 5 MB

        [Required(ErrorMessage = "Informe o nome completo.")]
        [StringLength(120, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 120 caracteres.")]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; } = "";

        [Required(ErrorMessage = "Informe o CPF.")]
        [Cpf]
        [Display(Name = "CPF")]
        public string Cpf { get; set; } = "";

        [Required(ErrorMessage = "Informe a data de nascimento.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateOnly? DataNascimento { get; set; }

        [RegularExpression(@"^\(?\d{2}\)?\s?\d{4,5}-?\d{4}$", ErrorMessage = "Telefone inválido.")]
        [Display(Name = "Telefone")]
        public string? Telefone { get; set; }

        [Required(ErrorMessage = "Informe o CEP.")]
        [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "CEP inválido.")]
        [Display(Name = "CEP")]
        public string Cep { get; set; } = "";

        [Required(ErrorMessage = "Selecione a UF.")]
        [RegularExpression("^(AC|AL|AP|AM|BA|CE|DF|ES|GO|MA|MT|MS|MG|PA|PB|PR|PE|PI|RJ|RN|RS|RO|RR|SC|SP|SE|TO)$", ErrorMessage = "Selecione a UF.")]
        [Display(Name = "UF")]
        public string Uf { get; set; } = "";

        [Required(ErrorMessage = "Informe a cidade.")]
        [StringLength(80)]
        [Display(Name = "Cidade")]
        public string Cidade { get; set; } = "";

        [Required(ErrorMessage = "Informe o bairro.")]
        [StringLength(80)]
        [Display(Name = "Bairro")]
        public string Bairro { get; set; } = "";

        [Required(ErrorMessage = "Informe a rua.")]
        [StringLength(120)]
        [Display(Name = "Rua")]
        public string Rua { get; set; } = "";

        [StringLength(10)]
        [Display(Name = "Nº")]
        public string? NumeroCasa { get; set; }

        [StringLength(60)]
        [Display(Name = "Complemento")]
        public string? Complemento { get; set; }

        [Required(ErrorMessage = "Informe sua condição.")]
        [StringLength(120)]
        [Display(Name = "Condição")]
        public string NomeCondicao { get; set; } = "";

        [Required(ErrorMessage = "Selecione o tempo da condição.")]
        [RegularExpression("^(permanente|temporaria)$", ErrorMessage = "Selecione o tempo da condição.")]
        [Display(Name = "Tempo da Condição")]
        public string TempoCondicao { get; set; } = "";

        [Required(ErrorMessage = "Envie o documento médico.")]
        [Display(Name = "Documento Médico")]
        public IFormFile? DocumentoMedico { get; set; }

        [Required(ErrorMessage = "Envie uma foto.")]
        [Display(Name = "Foto")]
        public IFormFile? Foto { get; set; }

        public static string SomenteDigitos(string? valor) =>
            new((valor ?? "").Where(char.IsDigit).ToArray());
    }

    /// <summary>
    /// Valida o CPF pelos dígitos verificadores.
    /// </summary>
    public sealed class CpfAttribute : ValidationAttribute
    {
        public CpfAttribute() : base("CPF inválido.")
        {
        }

        public override bool IsValid(object? value)
        {
            if (value is not string texto || string.IsNullOrWhiteSpace(texto))
            {
                return true; // [Required] cuida do vazio.
            }

            var cpf = CadastroViewModel.SomenteDigitos(texto);
            if (cpf.Length != 11 || cpf.Distinct().Count() == 1)
            {
                return false;
            }

            int Digito(int tamanho)
            {
                var soma = 0;
                for (var i = 0; i < tamanho; i++)
                {
                    soma += (cpf[i] - '0') * (tamanho + 1 - i);
                }
                var resto = soma % 11;
                return resto < 2 ? 0 : 11 - resto;
            }

            return Digito(9) == cpf[9] - '0' && Digito(10) == cpf[10] - '0';
        }
    }
}
