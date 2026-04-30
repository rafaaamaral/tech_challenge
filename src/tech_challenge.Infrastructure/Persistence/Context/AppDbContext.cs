using Microsoft.EntityFrameworkCore;
using tech_challenge.Application.Common.Interfaces;
using tech_challenge.Domain.Aggregates.Clientes;
using tech_challenge.Domain.Aggregates.Usuarios;
using tech_challenge.Domain.Common.Entities;
using tech_challenge.Infrastructure.Persistence.Configuration;

namespace tech_challenge.Infrastructure.Persistence.Context
{
    public class AppDbContext : DbContext
    {
        private readonly IUsuarioLogadoService _usuarioLogadoService;
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options, IUsuarioLogadoService usuarioLogadoService)
            : base(options)
        {
            _usuarioLogadoService = usuarioLogadoService;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ClienteConfiguration());
            modelBuilder.ApplyConfiguration(new UsuarioConfiguration());
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
