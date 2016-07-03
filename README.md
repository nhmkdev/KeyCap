# KeyCap

This is an old project I used many years ago for remapping input. I still find it useful sometimes.

## Description
*Windows only* application for capture keyboard input and remapping it to:
* another key or sequence of keys
* mouse input
* nothing at all

## TODO:
* Add a new release (currently available here: [Download Key2Key](https://www.nhmk.com/tools.php)
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

