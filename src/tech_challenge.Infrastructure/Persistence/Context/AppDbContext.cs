using Microsoft.EntityFrameworkCore;
using tech_challenge.Application.Common.Interfaces;
using tech_challenge.Domain.Aggregates.Clientes;
using tech_challenge.Domain.Aggregates.OrdemServicos;
using tech_challenge.Domain.Aggregates.PecaInsumos;
using tech_challenge.Domain.Aggregates.Servicos;
using tech_challenge.Domain.Aggregates.Usuarios;
using tech_challenge.Domain.Aggregates.Veiculos;
using tech_challenge.Domain.Common.Entities;
using tech_challenge.Infrastructure.Persistence.Configuration;

namespace tech_challenge.Infrastructure.Persistence.Context
{
    public class AppDbContext : DbContext
    {
        private readonly IUsuarioLogadoService _usuarioLogadoService;
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<OrdemServico> OrdemServicos { get; set; }
        public DbSet<OrdemServicoItemPecaInsumo> OrdemServicoItemPecaInsumos { get; set; }
        public DbSet<OrdemServicoItemServico> OrdemServicoItemServicos { get; set; }
        public DbSet<PecaInsumo> PecasInsumos { get; set; }
        public DbSet<Servico> Servicos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options, IUsuarioLogadoService usuarioLogadoService)
            : base(options)
        {
            _usuarioLogadoService = usuarioLogadoService;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ClienteConfiguration());
            modelBuilder.ApplyConfiguration(new OrdemServicoConfiguration());
            modelBuilder.ApplyConfiguration(new OrdemServicoItemPecaInsumoConfiguration());
            modelBuilder.ApplyConfiguration(new OrdemServicoItemServicoConfiguration());
            modelBuilder.ApplyConfiguration(new PecaInsumoConfiguration());
            modelBuilder.ApplyConfiguration(new ServicoConfiguration());
            modelBuilder.ApplyConfiguration(new UsuarioConfiguration());
            modelBuilder.ApplyConfiguration(new VeiculoConfiguration());
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userId = _usuarioLogadoService.UniqueCode ?? Guid.Empty;

            foreach (var entry in ChangeTracker.Entries<Audit>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.IsActive = true;
                    entry.Entity.CreatedBy = userId;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = userId;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
