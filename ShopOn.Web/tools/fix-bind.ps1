$dir = Join-Path (Split-Path $PSScriptRoot -Parent) "Controllers"
Get-ChildItem -Path $dir -Filter *.cs | ForEach-Object {
    $c = Get-Content $_.FullName -Raw
    $c = $c -replace '\[Bind\(Include = "([^"]+)"\)\]', '[Bind("$1")]'
    $c = $c -replace '\[Bind\(Prefix = "([^"]+)", Include = "([^"]+)"\)\]', '[Bind("$2", Prefix = "$1")]'
    [System.IO.File]::WriteAllText($_.FullName, $c)
}
Write-Host "done"
