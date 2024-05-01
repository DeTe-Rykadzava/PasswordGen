using System;
using System.IO;
using Database.Context;
using Microsoft.EntityFrameworkCore;

namespace Database.Core;

public static class DatabaseSettings
{
    public static IDatabaseContext GetBdContext()
    {
        var builder = new DbContextOptionsBuilder<DatabaseContext>();
        builder.UseSqlite("Data Source=Passwords.db");
        return new DatabaseContext(builder.Options);
    }

    public static string GetPathToDatabase()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Passwords.db");
    }
}