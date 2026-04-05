using System.Security.Cryptography;
using System.Text;
using EasyInventory.PgData;
using EasyInventory.PgData.Entities;
using Microsoft.EntityFrameworkCore;

static string EncryptPassword(string plain, string key)
{
    using var des = TripleDES.Create();
    using var md5 = MD5.Create();
    des.Key = md5.ComputeHash(Encoding.ASCII.GetBytes(key));
    des.Mode = CipherMode.ECB;
    var buf = Encoding.ASCII.GetBytes(plain);
    return Convert.ToBase64String(des.CreateEncryptor().TransformFinalBlock(buf, 0, buf.Length));
}

var withTestLogin = args.Contains("--with-test-login", StringComparer.OrdinalIgnoreCase);

var cs = Environment.GetEnvironmentVariable("NEON_CONNECTION_STRING");
if (string.IsNullOrWhiteSpace(cs))
{
    Console.Error.WriteLine("Set NEON_CONNECTION_STRING to your Neon Npgsql connection string.");
    Console.Error.WriteLine("Example (PowerShell): $env:NEON_CONNECTION_STRING = '<from neonctl>'");
    return 1;
}

await using var db = new EasyInventoryDbContext(
    new DbContextOptionsBuilder<EasyInventoryDbContext>().UseNpgsql(cs).Options);

await db.Database.MigrateAsync();
Console.WriteLine("Database migrated.");

if (!withTestLogin)
    return 0;

const string login = "testadmin";
const string plainPassword = "Test@123";
const string encryptKey = "d3A#";

if (await db.Employees.AnyAsync(e => e.Login == login))
{
    Console.WriteLine($"Employee {login} already exists. Login: {login} / Password: {plainPassword}");
    return 0;
}

var dept = await db.Departments.OrderBy(d => d.Id).FirstOrDefaultAsync();
if (dept == null)
{
    dept = new Department { Name = "Seed" };
    db.Departments.Add(dept);
    await db.SaveChangesAsync();
}

var enc = EncryptPassword(plainPassword, encryptKey);
db.Employees.Add(new Employee
{
    FirstName = "Test",
    LastName = "User",
    Login = login,
    Password = enc,
    EmployeeTypeId = 1,
    DepartmentId = dept.Id,
    IsActive = 1
});
await db.SaveChangesAsync();
Console.WriteLine($"Created {login}. Password: {plainPassword} (same TripleDES + key as MYBUSINESS UserManagement).");
return 0;
