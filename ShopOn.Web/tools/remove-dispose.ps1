$dir = Join-Path (Split-Path $PSScriptRoot -Parent) "Controllers"
Get-ChildItem $dir -Filter *.cs | ForEach-Object {
    $c = Get-Content $_.FullName -Raw
    if ($c -match '(?s)\s*protected override void Dispose\(bool disposing\)\s*\{[^}]*\}') {
        $c = $c -replace '(?s)\s*protected override void Dispose\(bool disposing\)\s*\{[^}]*\}', ''
        [System.IO.File]::WriteAllText($_.FullName, $c)
        Write-Host "removed Dispose from $($_.Name)"
    }
}
