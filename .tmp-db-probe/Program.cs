using Npgsql;
using System.Security.Cryptography;
using System.Text;

static string Encrypt(string plain, string key)
{
    using var tripleDes = TripleDES.Create();
    using var md5 = MD5.Create();
    tripleDes.Key = md5.ComputeHash(Encoding.ASCII.GetBytes(key));
    tripleDes.Mode = CipherMode.ECB;
    using var encryptor = tripleDes.CreateEncryptor();
    var bytes = Encoding.ASCII.GetBytes(plain);
    return Convert.ToBase64String(encryptor.TransformFinalBlock(bytes, 0, bytes.Length));
}

var cs = Environment.GetEnvironmentVariable("NEON_CONNECTION_STRING")
    ?? "Host=localhost;Port=5432;Database=easyinventory;Username=postgres;Password=your_password";

await using var conn = new NpgsqlConnection(cs);
try
{
    await conn.OpenAsync();

    await using (var cleanup = conn.CreateCommand())
    {
        cleanup.CommandText = """
            DELETE FROM "Employee" WHERE "Login" = 'testadmin';
            DELETE FROM "Department" WHERE "Name" = 'Seed';
            """;
        await cleanup.ExecuteNonQueryAsync();
    }

    int deptId;
    await using (var deptCmd = conn.CreateCommand())
    {
        deptCmd.CommandText = """
            INSERT INTO "Department" ("Name", "Remarks", "CreateDate", "UpdateDate", "bizId")
            VALUES ('Seed', NULL, NOW(), NOW(), NULL)
            RETURNING "Id";
            """;
        deptId = Convert.ToInt32(await deptCmd.ExecuteScalarAsync());
    }

    var encryptedPassword = Encrypt("Test@123", "d3A#");
    await using (var empCmd = conn.CreateCommand())
    {
        empCmd.CommandText = """
            INSERT INTO "Employee"
                ("FirstName", "LastName", "Gender", "Login", "Password", "Email",
                 "EmployeeTypeId", "RightId", "RankId", "DepartmentId", "Designation",
                 "Probation", "RegistrationDate", "Casual", "Earned", "IsActive",
                 "CreateDate", "UpdateDate", "bizId")
            VALUES
                ('Test', 'User', NULL, 'testadmin', @password, 'testadmin@stockpulse.com',
                 1, NULL, NULL, @deptId, 'Admin',
                 NULL, NOW(), NULL, NULL, 1,
                 NOW(), NOW(), NULL);
            """;
        empCmd.Parameters.AddWithValue("password", encryptedPassword);
        empCmd.Parameters.AddWithValue("deptId", deptId);
        await empCmd.ExecuteNonQueryAsync();
    }

    await using var check = conn.CreateCommand();
    check.CommandText = """
        SELECT
            (SELECT count(*) FROM "Employee") AS employee_count,
            (SELECT count(*) FROM "Employee" WHERE "Login" = 'testadmin') AS testadmin_count,
            (SELECT count(*) FROM "Department") AS department_count;
        """;
    await using var reader = await check.ExecuteReaderAsync();
    if (await reader.ReadAsync())
    {
        Console.WriteLine($"employees={reader.GetInt64(0)}, testadmin={reader.GetInt64(1)}, departments={reader.GetInt64(2)}");
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.GetType().FullName);
    Console.WriteLine(ex.Message);
    return;
}
