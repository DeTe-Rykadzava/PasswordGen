using System;
using Database.Context;
using Microsoft.EntityFrameworkCore.Design;

namespace Database.Core;

public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        return (DatabaseContext)DatabaseSettings.GetBdContext();
    }
}