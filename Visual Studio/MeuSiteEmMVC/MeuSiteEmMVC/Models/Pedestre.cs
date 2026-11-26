namespace MeuSiteEmMVC.Models
{
    /// <summary>
    /// Pedestre cadastrado para receber o cartão do semáforo (tabela public.pedestre).
    /// </summary>
    public class Pedestre
    {
        public long Id { get; set; }
        public string Nome { get; set; } = "";
        /// <summary>Somente os 11 dígitos, sem pontuação.</summary>
        public string Cpf { get; set; } = "";
        public DateOnly DataNascimento { get; set; }
        public string? Telefone { get; set; }
        /// <summary>Somente os 8 dígitos, sem hífen.</summary>
        public string Cep { get; set; } = "";
        public string Uf { get; set; } = "";
        public string Cidade { get; set; } = "";
        public string Bairro { get; set; } = "";
        public string Rua { get; set; } = "";
        public string? NumeroCasa { get; set; }
        public string? Complemento { get; set; }
        public string NomeCondicao { get; set; } = "";
        /// <summary>"permanente" ou "temporaria".</summary>
        public string TempoCondicao { get; set; } = "";
        /// <summary>Caminho do arquivo no bucket privado do Supabase Storage.</summary>
        public string FotoPath { get; set; } = "";
        public string DocMedicoPath { get; set; } = "";
        public DateTimeOffset CriadoEm { get; set; }
    }
}
