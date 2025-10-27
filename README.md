# Make Float Great Again
Re-add float override input (down + jump, after player has Faydown Cloak) with extensive customization options.

## Features

* Customizable Input: Configure whether holding specific inputs (Down, Up, Dream Nail, or Quick Map) to trigger floating (default: Hold Down).
* Invert Input Conditions: Invert the input condition logic. When enabled, at least one of the specified inputs (Down, Up, Dream Nail, or Quick Map) must be held to allow floating, instead of requiring none to be held (default: false).
* Horizontal Input Support: Allow triggering floating while holding horizontal input (default: false).

## Configuration
We recommend using the [BepInEx.ConfigurationManager](https://thunderstore.io/c/hollow-knight-silksong/p/jakobhellermann/BepInExConfigurationManager/) for easy in-game customization of settings. If you have it installed (press F1 in-game to open the menu), navigate to the "Make Float Great Again" section to adjust options directly—no file editing required.

Alternatively, edit the configuration file located at `BepInEx\config\com.demojameson.makefloatgreatagain.cfg`

## To install

### Thunderstore
It should all be handled for you auto-magically.

### Manual
First install BepInEx to your Silksong folder,
(note: this will break how thunderstore does things)

You can find it at
https://github.com/BepInEx/BepInEx/releases
latest stable is currently 5.4.23.4

After unzipping, run the game once, so that the BepInEx folder structure generates
(ie: there's folders in there apart from just `core`)

Then pull this DLL, or folder including the dll in to
`Hollow Knight Silksong\BepInEx\plugins`

## Source
GitHub: [https://github.com/DemoJameson/Silksong.MakeFloatGreatAgain](https://github.com/DemoJameson/Silksong.MakeFloatGreatAgain)