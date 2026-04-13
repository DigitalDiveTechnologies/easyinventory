param(
    [string]$ProjectRoot = "MYBUSINESS",
    [string]$OutputRoot = "artifacts"
)

$ErrorActionPreference = "Stop"

$projectPath = Join-Path (Get-Location) $ProjectRoot
$outputPath = Join-Path (Get-Location) $OutputRoot
$stagingPath = Join-Path $outputPath "site"
$zipPath = Join-Path $outputPath "easyinventory-site.zip"

if (-not (Test-Path $projectPath)) {
    throw "Project path '$projectPath' was not found."
}

Remove-Item -Recurse -Force $stagingPath -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force -Path $stagingPath | Out-Null

$robocopyArgs = @(
    $projectPath
    $stagingPath
    "/E"
    "/XD", "obj", ".vs", "documentation", "sqllocaldb_portable", "Properties"
    "/XF", "*.csproj", "*.csproj.user", "*.user", "*.cs", "*.vb", "*.pdb", "*.tt", "*.ttinclude", "*.edmx.diagram", "*.md", "*.url", "*.log", "*.msi", "bower.json", "composer.json", "package.json", "package-lock.json", "yarn.lock", ".npmignore", "ConnectToDB.exe", "Project_Readme.html"
)

robocopy @robocopyArgs | Out-Host

if ($LASTEXITCODE -gt 7) {
    throw "robocopy failed with exit code $LASTEXITCODE."
}

if (Test-Path $zipPath) {
    Remove-Item -Force $zipPath
}

Compress-Archive -Path (Join-Path $stagingPath "*") -DestinationPath $zipPath
Write-Host "Created deployment package: $zipPath"
