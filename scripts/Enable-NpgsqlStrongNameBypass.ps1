# Prefer MYBUSINESS\Lib\EntityFramework6.Npgsql (built from tmp-EntityFramework6.Npgsql with repo Npgsql.snk):
# fully signed and loads without sn -Vr.
#
# Fallback: NuGet EntityFramework6.Npgsql delay-signed builds may need verification skip for Npgsql's key
# (one-time per machine, requires Administrator).
#
# Right-click PowerShell -> Run as administrator, then:
#   powershell -ExecutionPolicy Bypass -File .\scripts\Enable-NpgsqlStrongNameBypass.ps1

$sn = @(
    "${env:ProgramFiles(x86)}\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\x64\sn.exe",
    "${env:ProgramFiles(x86)}\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools\x64\sn.exe"
) | Where-Object { Test-Path $_ } | Select-Object -First 1

if (-not $sn) {
    Write-Error "sn.exe not found. Install .NET Framework SDK or Visual Studio Build Tools."
    exit 1
}

& $sn -Vr "*,5d8b90d52f46fda7"
if ($LASTEXITCODE -ne 0) {
    Write-Error "sn -Vr failed (need Administrator). Public key token 5d8b90d52f46fda7 = Npgsql packages."
    exit $LASTEXITCODE
}

Write-Host "OK: strong-name verification skip registered for Npgsql key. Restart IIS Express / Visual Studio."
