# CI/CD for easyinventory

This repository now includes two GitHub Actions workflows for the ASP.NET MVC 5 / .NET Framework 4.8 application:

- `CI` validates the solution on every push and pull request targeting `dev`, `main`, or `master`.
- `CD` builds a deployable site package zip and uploads it as a workflow artifact on every push to `dev`, `main`, or `master`, and on manual runs.

## Workflows

### CI

File: `.github/workflows/ci.yml`

Steps:

1. Check out the repository.
2. Restore NuGet packages with MSBuild.
3. Build `MYBUSINESS.sln` in `Release` configuration.

### CD

File: `.github/workflows/cd.yml`

Steps:

1. Check out the repository.
2. Restore NuGet packages with MSBuild.
3. Build the solution in `Release` configuration.
4. Stage the runtime site files with `.github/scripts/package-site.ps1`.
5. Upload the package as the `easyinventory-site-package` artifact.

The artifact folder contains the generated deployment zip at `artifacts/easyinventory-site.zip`.

## Important repo-specific note

The project was referencing LocalDB files as if they already lived inside `MYBUSINESS/App_Data`, while the tracked database files are actually stored at the repository root:

- `bmsFresh.mdf`
- `bmsFresh_log.ldf`

The project file now links those root files into `App_Data` during packaging so local publish scenarios stay aligned with the repository layout.

The CD workflow itself packages the existing runtime files from disk instead of relying on the project file's full content manifest, because the repository currently contains many stale content entries that do not exist on disk and would otherwise break packaging.

## What this gives you today

- Continuous integration for restore and build validation.
- Continuous delivery of a deployment-ready package artifact from GitHub Actions.

## Next deployment step

This setup intentionally stops at a portable package artifact because the target hosting platform was not defined yet.

You can take the generated `easyinventory-site.zip` and deploy it to:

- IIS by extracting the site package to the target application directory
- Azure App Service
- Octopus Deploy
- Any release process that accepts a site-content zip artifact

If you want, the next step can be wiring this artifact into automatic deployment for your exact target environment.
