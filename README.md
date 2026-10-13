# ConfigurableQuestsLimit

[![Nexus Mods](https://img.shields.io/badge/Nexus%20Mods-Download-DA8E35?logo=nexusmods&logoColor=white)](https://www.nexusmods.com/atlyss/mods/41?tab=description)

## Description
Mod for ATLYSS that lets you configure the active quest limit or remove the limit completely.

BepInEx version. The MelonLoader version is on [main](https://github.com/somepoi/ConfigurableQuestsLimit/tree/main).

Quests limit is tied to the client, so should be multiplayer-compatible.

## Requirements
- [BepInEx 5](https://github.com/BepInEx/BepInEx/releases) (x64, v5.4.21 or newer)

## Installation
1. Install [BepInEx 5](https://github.com/BepInEx/BepInEx/releases) x64 into ATLYSS.
2. Download the BepInEx ZIP from [releases](https://github.com/somepoi/ConfigurableQuestsLimit/releases) and extract to `BepInEx/plugins` (i.e. `"ATLYSS\BepInEx\plugins\ConfigurableQuestsLimit.dll"`).
3. Start the game once to generate the config.

Use only one version of the mod at a time.

## Configuration
See `BepInEx/config/somepoi.ConfigurableQuestsLimit.cfg` to configure the active quest limit or remove it completely.
Restart the game after changing the config.

```ini
[ConfigurableQuestsLimit]
MaxActiveQuests = 10
UnlimitedQuests = false
```

`MaxActiveQuests` must be at least 1. `UnlimitedQuests` ignores that limit when enabled.

## Build

With .NET SDK 10 installed:

```powershell
dotnet build -c Release -p:DeployToGame=false
```

`game-libs/Versions.props` sets GameLibs, BepInEx and Harmony versions.
To build and install into your local game, remove `DeployToGame=false`
and set `GameDirectory` if needed.

## Releases

Actions builds the BepInEx ZIP on branch pushes and pull requests.
Push a tag such as `bepinex-1.0.1` to publish a GitHub release.
The tag must match the project version and its changelog section.

Nexus uploads use the existing `NEXUSMODS_API_KEY` secret and a separate
`NEXUSMODS_BEPINEX_FILE_ID` repository variable pointing to the BepInEx file.
Without that variable, Actions publishes only to GitHub.

