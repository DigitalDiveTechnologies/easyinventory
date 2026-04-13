$views = Join-Path (Split-Path $PSScriptRoot -Parent) "Views"
Get-ChildItem $views -Recurse -Filter *.cshtml | ForEach-Object {
    $c = Get-Content $_.FullName -Raw -Encoding UTF8
    $orig = $c
    $c = [regex]::Replace($c, '@Scripts\.Render\([^)]*\)', '')
    $c = [regex]::Replace($c, '@Styles\.Render\([^)]*\)', '')
    if ($c -ne $orig) {
        [System.IO.File]::WriteAllText($_.FullName, $c)
        Write-Host $_.FullName
    }
}
