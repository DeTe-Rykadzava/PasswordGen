using System.Threading;
using System.Threading.Tasks;
using Database.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace Database.Context;

public partial class DatabaseContext : DbContext, IDatabaseContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
    
    public virtual DbSet<Password> Passwords { get; set; }

    public Task<bool> ReinitDatabaseAsync()
    {
        Database.EnsureDeleted();
        Database.EnsureCreated();
        return Task.FromResult(true);
    }

    public Task<bool> CheckInit()
    {
        Database.EnsureCreated();
        return Task.FromResult(true);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
    {
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Password>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.EncryptedPassword).HasColumnName("encrypted_password");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.Iv).HasColumnName("iv");
        });

        OnModelCreatingPartial(modelBuilder);
        
        base.OnModelCreating(modelBuilder);
    }
    
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}