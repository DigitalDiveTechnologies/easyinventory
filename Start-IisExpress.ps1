# Run from repo root:
#   powershell -ExecutionPolicy Bypass -File .\Start-IisExpress.ps1
#
# Copies the IIS Express template, points the site at MYBUSINESS, listens on 11795 on the
# loopback interfaces only (no http://*:11795/ so URL ACL admin rights are not required).
# Use http://localhost:11795/ or http://127.0.0.1:11795/ - both should work.
$ErrorActionPreference = "Stop"
$repoRoot = $PSScriptRoot
$sitePath = Join-Path $repoRoot "MYBUSINESS"
if (-not (Test-Path $sitePath)) {
    throw "MYBUSINESS folder not found at $sitePath"
}

$template = Join-Path $env:ProgramFiles "IIS Express\config\templates\PersonalWebServer\applicationhost.config"
if (-not (Test-Path $template)) {
    $template = Join-Path ${env:ProgramFiles(x86)} "IIS Express\config\templates\PersonalWebServer\applicationhost.config"
}
if (-not (Test-Path $template)) {
    throw "IIS Express applicationhost template not found under Program Files."
}

$pathAttr = [System.Security.SecurityElement]::Escape($sitePath)
$cfgPath = Join-Path $env:TEMP "easyinventory-iisexpress.applicationhost.config"
Copy-Item -Path $template -Destination $cfgPath -Force
$content = [System.IO.File]::ReadAllText($cfgPath)
$content = $content.Replace(
    'physicalPath="%IIS_SITES_HOME%\WebSite1"',
    ('physicalPath="{0}"' -f $pathAttr)
)
$content = $content.Replace(
    @'
                <bindings>
                    <binding protocol="http" bindingInformation=":8080:localhost" />
                </bindings>
'@,
    @'
                <bindings>
                    <binding protocol="http" bindingInformation="127.0.0.1:11795:" />
                    <binding protocol="http" bindingInformation="[::1]:11795:" />
                </bindings>
'@
)

[System.IO.File]::WriteAllText($cfgPath, $content, [System.Text.UTF8Encoding]::new($false))

$iis = Join-Path $env:ProgramFiles "IIS Express\iisexpress.exe"
if (-not (Test-Path $iis)) {
    $iis = Join-Path ${env:ProgramFiles(x86)} "IIS Express\iisexpress.exe"
}
if (-not (Test-Path $iis)) {
    throw "iisexpress.exe not found."
}

Write-Host 'Starting MYBUSINESS at http://localhost:11795/ (and http://127.0.0.1:11795/)'
Write-Host "Config: $cfgPath"
& $iis "/config:$cfgPath" "/site:WebSite1"
