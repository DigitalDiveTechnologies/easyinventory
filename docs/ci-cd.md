# CI/CD for easyinventory

GitHub Actions build and package the **ASP.NET Core 8** app (`ShopOn.Web`) against **Neon PostgreSQL** (`EasyInventory.PgData`).

- **CI** validates `EasyInventory.sln` on push/PR to `dev`, `main`, or `master`.
- **CD** publishes `ShopOn.Web` and uploads `easyinventory-site.zip` as an artifact on push to those branches (and manual runs).

## Workflows

### CI

File: `.github/workflows/ci.yml`

1. Check out the repository.
2. Set up .NET 8 SDK.
3. `dotnet restore` / `dotnet build` on `EasyInventory.sln` (Release).

### CD

File: `.github/workflows/cd.yml`

1. Check out the repository.
2. Set up .NET 8 SDK.
3. `dotnet publish ShopOn.Web` to `artifacts/publish`, then zip as `artifacts/easyinventory-site.zip`.
4. Upload the zip as the `easyinventory-site-package` artifact.

For a **Windows-local** package matching CI output, run:

`powershell -ExecutionPolicy Bypass -File .\.github\scripts\package-site.ps1`

## Important repo-specific note

Inventory data lives in **Neon PostgreSQL**. Connection strings belong in `ShopOn.Web` configuration (e.g. `appsettings.Development.json` or environment variables), not committed secrets.

The deployment zip is the **framework-dependent publish output** of `ShopOn.Web` (DLLs, `wwwroot`, `web.config` for IIS if generated), suitable for `dotnet ShopOn.Web.dll` or IIS/Kestrel hosting.

**Solution:** open **`EasyInventory.sln`** at the repo root (contains `ShopOn.Web`, `EasyInventory.PgData`, `EasyInventory.DbMigrator`).

## What this gives you today

- CI build validation on Linux runners (no MSBuild/WebApplication.targets required).
- CD artifact: published Core app as a zip.

## Next deployment step

Use `easyinventory-site.zip` on a server with the **.NET 8 runtime** (or use **self-contained** publish if you change the publish profile). `deploy-droplet.yml` uploads the same artifact for SSH-based releases.
