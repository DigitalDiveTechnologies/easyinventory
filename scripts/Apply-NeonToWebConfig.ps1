# Run from repo root: powershell -File .\scripts\Apply-NeonToWebConfig.ps1
# Fills MYBUSINESS\Web.config from `neonctl connection-string main`. Do not commit secrets.
$ErrorActionPreference = "Stop"
$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$wcPath = Join-Path $repoRoot "MYBUSINESS\Web.config"
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
[xml]$x = Get-Content $wcPath
foreach ($n in $x.configuration.connectionStrings.add) {
    if ($n.name -eq 'DefaultConnection' -or $n.name -eq 'BusinessContext') {
        $n.connectionString = $cs
        $n.providerName = 'Npgsql'
    }
}
$x.Save($wcPath)
Write-Host "Updated connection strings in $wcPath"
