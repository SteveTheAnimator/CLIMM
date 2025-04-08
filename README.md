# CLIMM

CLIMM is a command-line tool designed for the installation and management of mods for Unity games.

## Features

- **Install BepInEx**: Quickly set up the BepInEx modding framework for supported Unity games.
- **Install Mods from GitHub**: Seamlessly add mods directly from GitHub repositories to your Unity games.

## Prerequisites

- **.NET SDK**: CLIMM is built using the .NET SDK (8.0). Ensure it's installed on your system. Download it from the [.NET official website](https://dotnet.microsoft.com/download).

## Usage

CLIMM provides two main commands:

1. **Install BepInEx**:
   - To install BepInEx for a specific game:
     ```bash
     CLIMM.exe install-bepinex --game-path "<path-to-game-directory>"
     ```
     Replace `<path-to-game-directory>` with your game's root directory path.

2. **Install a Mod from GitHub**:
   - To install a mod:
     ```bash
     CLIMM.exe install-mod --game-path "<path-to-game-directory>" --mod-url "<mod-github-url>"
     ```
     - `<path-to-game-directory>`: Your game's root directory path.
     - `<mod-github-url>`: Direct URL to the mod's GitHub repository or release.

## License

CLIMM is open-source software licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

By following this guide, you can effectively install, use, and uninstall CLIMM to manage mods for your Unity games.
