using System.Threading;
using System.Threading.Tasks;
using Database.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace Database.Context;

public interface IDatabaseContext
{
    public DbSet<Password> Passwords { get; set; }
    public Task<bool> ReinitDatabaseAsync();
    public Task<bool> CheckInit();
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
    public Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken());
}