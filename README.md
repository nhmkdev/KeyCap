# KeyCap

## Description
*Windows only* application for capture keyboard input and remapping it to:
* another key or sequence of keys
* mouse input
* nothing at all
* on/off toggles for keys and inputs (this allows a single keystroke to initiate a hold and a second keystroke to end the hold)

## Example Uses
* (original inspiration) Shortcutting a sequence of shortcuts. I originally created the application long ago to help with a sequence of keyboard shortcuts necessary to perform file merges when integrating a branch with [Perforce](https://www.perforce.com/) + [Araxis Merge](https://www.araxis.com/)
* [Starbound](http://playstarbound.com/) Keyboard shortcuts to toggle off/on mouse buttons. This makes extensive mining in the game a lot less painful. [Mass Effect 2](http://masseffect.bioware.com/me2/) planet scanning also can be a lot easier...
* [Dreamfall Chapters](http://redthreadgames.com/games/chapters/) has a run button though it can be nice to have a button that toggles whether you are running or not instead of holding one down.

## TODO:
* Add a new release (currently available here: [Download Key2Key](https://www.nhmk.com/tools.php)
** This release is the older version and does not support the toggle action.
* Code comments
* Code cleanup
* Proper description for how it all works! I have forgotten some of the details...

## Warning
This program does perform keyboard capture and is essentially the first component of a key logger. I humbly request you NOT use the information included in the source code to create a malicious application.

## Visual Studio Notes
* If you attempt to run from Visual Studio you will need to make sure the C# project is configured with *Enable Native Code Debugging*. If this is not enabled the dll will not create the necessary keyboard hook (works fine outside of Visual Studio). This was observed in every Visual Studio version up-to and including 2015.

## Technical Documentation

### File Format

The file is a repeated sequence of the following information.

Byte array representation of each input/output pair:

| Flags | Value | Outputs Count | Flags | Value | (repeats) Flag and value for all outputs |
| --- | --- | --- | --- | --- | --- |

#### Flags

The flags indicate information that modifies the value (alt/shift/ctrl and special functionality when mapping to another output)

#### Output Count

This is limited to a byte, so 255 maximum.

## History

This is an old project I used many years ago for remapping input. I still find it useful sometimes.
