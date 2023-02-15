# KeyCap

Download the [Latest Release](https://github.com/nhmkdev/KeyCap/releases/latest)

## Description
*Windows only* application for capturing keyboard input and remapping it to:
* another key or sequence of keys
* mouse input
* nothing at all
* on/off toggles for keys and inputs (this allows a single keystroke to initiate a hold and a second keystroke to end the hold)
* Repetition (with custom delay)

## Example Uses
* (original inspiration) Shortcutting a sequence of shortcuts. I originally created the application long ago to help with a sequence of keyboard shortcuts necessary to perform file merges when integrating a branch with [Perforce](https://www.perforce.com/) + [Araxis Merge](https://www.araxis.com/)
* [Starbound](http://playstarbound.com/) Keyboard shortcuts to toggle off/on mouse buttons. This makes extensive mining in the game a lot less painful. [Mass Effect 2](http://masseffect.bioware.com/me2/) planet scanning also can be a lot easier...
* [Dreamfall Chapters](http://redthreadgames.com/games/chapters/) has a run button though it can be nice to have a button that toggles whether you are running or not instead of holding one down.
* [Citadel: Forged With Fire](https://www.citadelthegame.com/) has a number of long mouse hold actions. Toggling this off/on instead of holding the button for long periods of time is extremely nice.

## Run on Startup

With Registry Editor, add a new StringValue to `HKCU\Software\Microsoft\Windows\CurrentVersion\Run` with the full path to `KeyCap.exe` followed by the full path to your `.kfg` file.  KeyCap will load the config file, start it and then minimize to the tray.

Alternatively, copy the following to a text file named `KeyCap.reg` and then edit it to match your setup.  Save it, right click-it and selct **Merge**.  Follow the prompts and then the next time you reboot, KeyCap will start automatically.

```regedit
Windows Registry Editor Version 5.00

[HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run]
"KeyCap"="D:\\Util\\KeyCap\\KeyCap.exe -f D:\\Util\\KeyCap\\Config.kfg"
```

You can also include the `-autostart` argument if you wish to immediately start capturing keys with the loaded config.

## Warning
This program does perform keyboard capture and is essentially the first component of a key logger. I humbly request you NOT use the information included in the source code to create a malicious application.

## Visual Studio Notes
* If you attempt to run from Visual Studio you will need to make sure the C# project is configured with *Enable Native Code Debugging*. If this is not enabled the dll will not create the necessary keyboard hook (works fine outside of Visual Studio). This was observed in every Visual Studio version up-to and including 2015.

## Technical Documentation

### .kfg File Format

The file is prefixed with two 32-bit ints:

| File Data Prefix | File Format Version |
| --- | --- |
| 0x0E0CA000 | 0x1

The remainder of the file is a repeated sequence of the following information. One input may be associated with numerous outputs.

Byte array representation of each input/output(s) pair:

`(#)` is the number of bytes the given information is alloted.

**Input Bytes**
| Input Flags (4) | Input VirtualKey (1) | Padding (3) | Parameter (4) | Outputs Count (1) | Padding (3) | (all outputs for input) |
| --- | --- | --- | --- | --- | --- | --- |

**Output Bytes**
| Output Flags (4) | Output VirtualKey (1) | Padding (3) | Output Parameter (4) |
| --- | --- | --- | --- |

There is no count for the number of inputs. Instead the file is read until no further input/output(s) pairings can be read.

#### Flags

The flags indicate information that modifies the value (alt/shift/ctrl and special functionality when mapping to another output)

#### VirtualKey

A [VirtualKey](https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes) value.

#### Parameter

A special value associated with the input/output (as necessary based on the function).

#### Output Count

This is limited to a single byte despite the use of an int, so 255 maximum.

## History

Used to be called Key2Key but then I added mouse support so that name needed to change.

## Disclaimer

I am providing code in the repository to you under an open source license. Because this is my personal repository, the license you receive to my code is from me and not my employer.
