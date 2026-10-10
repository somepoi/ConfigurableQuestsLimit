# ConfigurableQuestsLimit

A MelonLoader mod for ATLYSS that lets you configure the active quest limit or
enable unlimited quests, with an expanded quest journal and paginated tracker.

## Installation

Install the Mono version of MelonLoader for ATLYSS, then extract the mod release
archive into the ATLYSS game directory. It includes `Mods/ConfigurableQuestsLimit.dll`
and `UserData/ConfigurableQuestsLimit.cfg`.

If upgrading, keep your existing config instead of replacing it with the defaults.
Set `MaxActiveQuests` (default: 10), or set `UnlimitedQuests = true` in the config.

## Releases

Download the mod on [Nexus Mods](https://www.nexusmods.com/atlyss/mods/41).
Version history is in [CHANGELOG.md](CHANGELOG.md).

Pushing a version tag such as `v1.0.1` makes GitHub Actions build the mod, create
a GitHub Release, and upload the ZIP and matching changelog to Nexus.
Compiler references come from our `Somepoi.Atlyss.GameLibs` NuGet package and
the official MelonLoader/Harmony packages, all restored from nuget.org.
GameLibs is maintained in [its own repository](https://github.com/somepoi/Atlyss.GameLibs).
No game installation or local runner is needed on GitHub.
See [the release setup guide](docs/RELEASING.md).

## Build

With .NET SDK 10 installed:

```powershell
dotnet build -c Release -p:DeployToGame=false
```

`game-libs/Versions.props` pins GameLibs, MelonLoader and Harmony versions.
To build and install into your local game instead, omit `DeployToGame=false`
and set `GameDirectory` if needed.

