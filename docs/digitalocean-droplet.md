# DigitalOcean Droplet Delivery

This repository now includes a GitHub Actions workflow that can upload the packaged site artifact to a DigitalOcean Droplet over SSH:

- `.github/workflows/deploy-droplet.yml`

## What the workflow does

1. Restores and builds the .NET Framework solution on `windows-latest`.
2. Creates the deployable site zip with `.github/scripts/package-site.ps1`.
3. Downloads that artifact on `ubuntu-latest`.
4. Connects to the droplet over SSH.
5. Uploads the zip to a release folder.
6. Extracts it under a release directory and updates a `current` symlink.

Default target path on the server:

- `/opt/smartpos/easyinventory`

Release layout on the server:

- `/opt/smartpos/easyinventory/releases/<git-sha>/easyinventory-site.zip`
- `/opt/smartpos/easyinventory/releases/<git-sha>/site`
- `/opt/smartpos/easyinventory/current`

## Required GitHub repository secrets

Add these secrets in GitHub before running the workflow:

- `DROPLET_HOST`
- `DROPLET_USER`
- `DROPLET_SSH_KEY`
- `DROPLET_TARGET_PATH`

`DROPLET_TARGET_PATH` is optional. If not set, the workflow uses `/opt/smartpos/easyinventory`.

## Important platform note

This workflow connects the repository to the droplet for artifact delivery only.

The application itself still targets ASP.NET MVC 5 on .NET Framework 4.8, which normally requires Windows + IIS to run. DigitalOcean's documentation says standard Droplets do not support Windows images or Windows custom images, so a normal Linux Droplet is not a native runtime target for this app.

That means this workflow can successfully upload the package to the droplet, but the application will still need one of these next steps to actually run in production:

- move deployment to a Windows host
- migrate the application to ASP.NET Core / Linux
- introduce a nonstandard compatibility layer and accept the operational risk

## Validation note

I verified SSH access to `165.22.221.180` using the local key at `C:\Users\itsei\.ssh\smartpos_droplet` and confirmed the droplet is running Ubuntu 24.04.3 LTS. I also confirmed `python3` is installed on the server, so the workflow extracts the package with Python instead of relying on `unzip`.
