using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EasyInventory.PgData;

/// <summary>Used by dotnet ef. Set env <c>NEON_CONNECTION_STRING</c> (Npgsql format) before running migrations.</summary>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<EasyInventoryDbContext>
{
    public EasyInventoryDbContext CreateDbContext(string[] args)
    {
        var cs = Environment.GetEnvironmentVariable("NEON_CONNECTION_STRING")
            ?? "Host=127.0.0.1;Port=5432;Database=easyinventory;Username=local;Password=local;SSL Mode=Disable";
        var options = new DbContextOptionsBuilder<EasyInventoryDbContext>()
            .UseNpgsql(cs)
            .Options;
        return new EasyInventoryDbContext(options);
    }
}
