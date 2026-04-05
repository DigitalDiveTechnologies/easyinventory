# Creates Department (if none) and Employee testadmin / Test@123 (same encryption as UserManagementController).
# Uses EasyInventory.DbMigrator (.NET 8) to avoid Npgsql dependency issues in Windows PowerShell.
# Requires: neonctl auth
$ErrorActionPreference = "Stop"
$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$neonRoot = Join-Path $repoRoot "EasyInventory.NeonDb"
$raw = (neonctl connection-string main 2>&1 | Select-Object -First 1).ToString().Trim()
if (-not $raw.StartsWith("postgresql://")) { throw "Unexpected neonctl output: $raw" }
$u = [Uri]$raw
$ui = $u.UserInfo
$i = $ui.IndexOf(':')
$env:NEON_CONNECTION_STRING = "Host=$($u.Host);Port=5432;Database=$($u.AbsolutePath.TrimStart('/'));Username=$($ui.Substring(0,$i));Password=$($ui.Substring($i+1));SSL Mode=Require;Trust Server Certificate=true"
Push-Location $neonRoot
try {
    dotnet run --project "src\EasyInventory.DbMigrator\EasyInventory.DbMigrator.csproj" -- --with-test-login
} finally {
    Pop-Location
}
