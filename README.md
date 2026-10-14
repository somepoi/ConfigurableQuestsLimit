# ConfigurableQuestsLimit

[![Nexus Mods](https://img.shields.io/badge/Nexus%20Mods-Download-DA8E35?logo=nexusmods&logoColor=white)](https://www.nexusmods.com/atlyss/mods/41?tab=description)

## Description
Mod for ATLYSS that lets you configure the active quest limit remove the limit completely.

MelonLoader version. The BepInEx version is on [BepInEx](https://github.com/somepoi/ConfigurableQuestsLimit/tree/BepInEx).

Quests limit is tied to the client, so should be multiplayer-compatible.

## Requirements
- [MelonLoader](https://github.com/lavagang/melonloader) (v0.7.3)

## Installation
1. Install [MelonLoader](https://github.com/lavagang/melonloader)
2. Download [latest release](https://github.com/somepoi/ConfigurableQuestsLimit/releases) and extract to `Mods` folder (i.e. `"ATLYSS\Mods\ConfigurableQuestsLimit.dll"`)

## Configuration
See `UserData/ConfigurableQuestsLimit.cfg` to configure the current limit of active quest or remove the limit completely.

## Build

With .NET SDK 10 installed:

```powershell
dotnet build -c Release -p:DeployToGame=false
```

`game-libs/Versions.props` sets GameLibs, MelonLoader and Harmony versions.
To build and install into your local game, remove `DeployToGame=false`
and set `GameDirectory` if needed.

