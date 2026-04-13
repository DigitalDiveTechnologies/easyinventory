# Publishes ShopOn.Web (ASP.NET Core 8) and zips the output for deployment.
# Run from repo root: powershell -ExecutionPolicy Bypass -File .\.github\scripts\package-site.ps1
param(
    [string]$OutputRoot = "artifacts"
)

$ErrorActionPreference = "Stop"
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
$publishDir = Join-Path (Join-Path $repoRoot $OutputRoot) "publish"
$zipPath = Join-Path (Join-Path $repoRoot $OutputRoot) "easyinventory-site.zip"

Push-Location $repoRoot
try {
    if (Test-Path $publishDir) {
        Remove-Item -Recurse -Force $publishDir
    }
    New-Item -ItemType Directory -Force -Path (Split-Path $publishDir) | Out-Null

    dotnet publish (Join-Path $repoRoot "ShopOn.Web\ShopOn.Web.csproj") `
        -c Release `
        -o $publishDir `
        --verbosity minimal

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet publish failed with exit code $LASTEXITCODE."
    }

    if (Test-Path $zipPath) {
        Remove-Item -Force $zipPath
    }

    Compress-Archive -Path (Join-Path $publishDir "*") -DestinationPath $zipPath -Force
    Write-Host "Created deployment package: $zipPath"
} finally {
    Pop-Location
}
