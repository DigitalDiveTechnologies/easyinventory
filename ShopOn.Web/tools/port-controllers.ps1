$ErrorActionPreference = "Stop"
$root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$srcDir = Join-Path $root "MYBUSINESS\Controllers"
$dstDir = Join-Path $root "ShopOn.Web\Controllers"

function Get-ControllerName([string]$fileName) {
    return [System.IO.Path]::GetFileNameWithoutExtension($fileName)
}

Get-ChildItem $srcDir -Filter "*Controller.cs" | ForEach-Object {
    $name = $_.Name
    $short = Get-ControllerName $name
    $c = Get-Content $_.FullName -Raw -Encoding UTF8

    $c = $c.Replace("`r`n", "`n")

    $c = $c -replace "namespace MYBUSINESS.Controllers", "namespace ShopOn.Web.Controllers"
    $c = $c -replace "using MYBUSINESS.Models;", "using ShopOn.Web.Models;`nusing EasyInventory.PgData.Entities;"
    $c = $c -replace "using MYBUSINESS.CustomClasses;", "using ShopOn.Web.Infrastructure;"
    $c = $c -replace "using System.Web.Mvc;", "using Microsoft.AspNetCore.Mvc;`nusing Microsoft.AspNetCore.Authorization;`nusing Microsoft.AspNetCore.Http;`nusing Microsoft.EntityFrameworkCore;`nusing EasyInventory.PgData;`nusing ShopOn.Web.Data;`nusing Microsoft.AspNetCore.Mvc.Rendering;"
    $c = $c -replace "using System.Web;`n", ""
    $c = $c -replace "using System.Web.Routing;`n", ""
    $c = $c -replace "using System.Data.Entity;`n", ""
    $c = $c -replace "using System.Data.SqlClient;`n", ""
    $c = $c -replace "using System.Drawing.Printing;`n", ""
    $c = $c -replace "using Microsoft.Reporting.WebForms;", "using Microsoft.Reporting.NETCore;"

    $ctorName = $short
    $ctorBlock = "private readonly EasyInventoryDbContext _db;`n`n        public $ctorName(EasyInventoryDbContext db) { _db = db; }"

    $c = $c -replace "private BusinessContext db = new BusinessContext\(\);", $ctorBlock
    $c = $c -replace "private readonly BusinessContext _db = new BusinessContext\(\);", $ctorBlock

    $c = $c -replace "\bdb\.", "_db."

    $c = $c -replace "public ActionResult", "public IActionResult"
    $c = $c -replace "return new HttpStatusCodeResult\(HttpStatusCode\.BadRequest\)", "return BadRequest()"
    $c = $c -replace "return HttpNotFound\(\)", "return NotFound()"

    $c = $c.Replace("`n", "`r`n")

    $out = Join-Path $dstDir $name
    Set-Content -Path $out -Value $c -Encoding utf8
    Write-Host "Wrote $name"
}
