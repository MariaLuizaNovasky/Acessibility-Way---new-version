using MeuSiteEmMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace MeuSiteEmMVC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Pedestre> Pedestres => Set<Pedestre>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mapeamento igual ao script Data/schema.sql.
            modelBuilder.Entity<Pedestre>(e =>
            {
                e.ToTable("pedestre", "public");
                e.HasKey(p => p.Id);
                e.Property(p => p.Id).HasColumnName("id").UseIdentityAlwaysColumn();
                e.Property(p => p.Nome).HasColumnName("nome").HasMaxLength(120);
                e.Property(p => p.Cpf).HasColumnName("cpf").HasMaxLength(11).IsFixedLength();
                e.HasIndex(p => p.Cpf).IsUnique();
                e.Property(p => p.DataNascimento).HasColumnName("data_nascimento");
                e.Property(p => p.Telefone).HasColumnName("telefone").HasMaxLength(20);
                e.Property(p => p.Cep).HasColumnName("cep").HasMaxLength(8).IsFixedLength();
                e.Property(p => p.Uf).HasColumnName("uf").HasMaxLength(2).IsFixedLength();
                e.Property(p => p.Cidade).HasColumnName("cidade").HasMaxLength(80);
                e.Property(p => p.Bairro).HasColumnName("bairro").HasMaxLength(80);
                e.Property(p => p.Rua).HasColumnName("rua").HasMaxLength(120);
                e.Property(p => p.NumeroCasa).HasColumnName("numero_casa").HasMaxLength(10);
                e.Property(p => p.Complemento).HasColumnName("complemento").HasMaxLength(60);
                e.Property(p => p.NomeCondicao).HasColumnName("nome_condicao").HasMaxLength(120);
                e.Property(p => p.TempoCondicao).HasColumnName("tempo_condicao").HasMaxLength(12);
                e.Property(p => p.FotoPath).HasColumnName("foto_path");
                e.Property(p => p.DocMedicoPath).HasColumnName("doc_medico_path");
                e.Property(p => p.CriadoEm).HasColumnName("criado_em").HasDefaultValueSql("now()").ValueGeneratedOnAdd();
            });
        }
    }
}
