# Ares Chronicles Dumper

A tool for extracting and converting the Unity AssetBundles used by the game Ares Chronicles.

Unlike most other games, this one uses a customised AssetBundle structure that is natively supported by a modified Unity Engine opposed to being deciphered by in-game scripts before being loaded. This custom structure replaces the standard Unity header with a elaborately bit twiddled version which contains just the important Unity header fields. Additionally, these bundles have an XOR cipher applied to their Block Info section.

This tool both extracts and converts each bundle back to the standard format which is then readable by AssetStudio (and possibly Unity).

#### Usage

Place `AresChroniclesDumper.exe` in the game's `client` folder next to `zskrpc.exe` and run.

This will create a new folder called `Dump` containing two sub-directories called `InnerPackages` and `OuterPackages`. `InnerPackages` will contain all the files from the base game's archives and `OuterPackages` will contain hotfix files that download with each update.