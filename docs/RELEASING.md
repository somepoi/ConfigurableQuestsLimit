# Releasing to GitHub and Nexus

GitHub Actions builds the mod from NuGet references. A matching stable version
tag creates a GitHub Release and uploads the same ZIP and version changelog to
the existing [Nexus mod page](https://www.nexusmods.com/atlyss/mods/41).

## One-time setup

1. Push this repository to GitHub and enable Actions.
2. In **Settings → Secrets and variables → Actions**, configure:

   | Type | Name | Value |
   | --- | --- | --- |
   | Secret | `NEXUSMODS_API_KEY` | Your [Nexus API key](https://www.nexusmods.com/settings/api-keys) for the account that can update this mod |
   | Variable | `NEXUSMODS_FILE_ID` | Persistent File ID of the existing main file |

Find the File ID through **Advanced / API Info** in the Nexus
[Files tab](https://www.nexusmods.com/atlyss/mods/41?tab=files), or the file edit
menu in Manage Files. This is the file's update-chain ID, not the page number
`41` or a download/version ID. See the [official upload action](https://github.com/Nexus-Mods/upload-action).
The workflow resolves the global Mod ID from `atlyss/mods/41` automatically.

The mod repository consumes published GameLibs from nuget.org. It needs no
NuGet publishing credentials, local DLL feed, Steam credentials or self-hosted
runner. GameLibs publication is configured independently in
[Atlyss.GameLibs](https://github.com/somepoi/Atlyss.GameLibs) using Trusted Publishing.

## Release a version

1. Set `<Version>` in `ConfigurableQuestsLimit.csproj`, e.g. `1.0.1`.
   AssemblyVersion, FileVersion and MelonLoader's mod version follow it.
2. Add a matching section at the top of `CHANGELOG.md`:

   ```markdown
   # 1.0.1

   ## Fixes

   - Describe the change.
   ```

3. Commit and push the sources. Wait for the branch build to succeed.
4. Push the matching tag:

   ```powershell
   git tag v1.0.1
   git push origin v1.0.1
   ```

| Event | Result |
| --- | --- |
| Branch push or pull request | Build and downloadable `mod-package` artifact |
| Stable `vX.Y.Z` tag push | Build, GitHub Release and Nexus upload |
| Manual run with `publish` unchecked | Build and artifact only |
| Manual run on a version tag with `publish` checked | Build and publish to both services |

Only stable `vX.Y.Z` tags are supported. The tag must match the project version;
missing, empty or duplicate changelog sections stop the release. Artifact
downloads are retained for seven days; GitHub Release assets remain available.
No local publication command is needed. Your PC can be off after the push.

The installable ZIP contains exactly:

```text
Mods/ConfigurableQuestsLimit.dll
UserData/ConfigurableQuestsLimit.cfg
README.md
CHANGELOG.md
LICENSE
```

GameLibs, MelonLoader and Harmony references are excluded. Nexus receives only
the selected version's changelog as the file description and mod changelog.
The previous file version is archived and the displayed mod version is updated.

## Build locally and update dependencies

Requirements: .NET SDK 10; Python 3 for release packaging.

```powershell
dotnet build ConfigurableQuestsLimit.csproj -c Release -p:DeployToGame=false
python scripts/release.py metadata --tag v1.0.1
python scripts/release.py package --tag v1.0.1
```

All builds restore `Somepoi.Atlyss.GameLibs` and the official
`LavaGang.MelonLoader`/`HarmonyX` packages from nuget.org. Version pins are in
`game-libs/Versions.props`. NuGet needs no authentication to download these
public packages. Compiler references are not copied into the game or release ZIP.

Without `DeployToGame=false`, a successful local build installs the mod into
`GameDirectory`, which defaults to `D:\GAMES\SteamLibrary\steamapps\common\ATLYSS`.
An existing player configuration is preserved.

GameLibs snapshots are updated and published in their own repository after
Steam updates the maintainer's installed game. To adopt a published snapshot,
change `GameLibsVersion` in `Versions.props` and rebuild. Update the loader and
Harmony pins there when needed. Existing releases retain their original pins.

## Failures and retries

A missing Nexus setting or failed build stops publication. If Nexus fails after
GitHub Release creation, that release and the workflow artifact retain the ZIP.

Nexus uploads and changelog writes are not transactional. Publishing jobs reject
reruns to prevent duplicate uploads. Check Nexus Files and Changelogs first.
If nothing was uploaded, start a **new manual run**, select the same version tag
and check `publish`. If only the changelog is missing, add it on Nexus without
uploading the file again.
