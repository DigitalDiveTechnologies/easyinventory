using System.Data.Entity.Migrations;
using MYBUSINESS.Models;
using Npgsql;

namespace MYBUSINESS.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = false;
            SetSqlGenerator("Npgsql", new NpgsqlMigrationSqlGenerator());
        }
    }
}
