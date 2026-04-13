# Run from repo root: powershell -File .\scripts\Apply-NeonToWebConfig.ps1
# Writes Neon connection string into ShopOn.Web\appsettings.Development.json (local only; do not commit secrets).
$ErrorActionPreference = "Stop"
$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$cfgPath = Join-Path $repoRoot "ShopOn.Web\appsettings.Development.json"
$raw = (neonctl connection-string main 2>&1 | Select-Object -First 1).ToString().Trim()
if (-not $raw.StartsWith("postgresql://")) { throw "Unexpected neonctl output: $raw" }
$u = [Uri]$raw
$userInfo = $u.UserInfo
$idx = $userInfo.IndexOf(':')
$user = $userInfo.Substring(0, $idx)
$pass = $userInfo.Substring($idx + 1)
$db = $u.AbsolutePath.TrimStart('/')
$nh = $u.Host
$cs = "Host=$nh;Port=5432;Database=$db;Username=$user;Password=$pass;SSL Mode=Require;Trust Server Certificate=true"

$json = @{
    ConnectionStrings = @{
        DefaultConnection = $cs
    }
    Logging           = @{
        LogLevel = @{
            Default               = "Information"
            "Microsoft.AspNetCore" = "Warning"
        }
    }
}
$json | ConvertTo-Json -Depth 5 | Set-Content -Path $cfgPath -Encoding UTF8
Write-Host "Updated ConnectionStrings:DefaultConnection in $cfgPath"
