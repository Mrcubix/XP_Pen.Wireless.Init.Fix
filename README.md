# XP-Pen Wireless Init Fix

A plugin for OpenTabletDriver that attempts to fix nitialization issue caused by the wireless dongle, sometimes used with XP-Pen Deco LW, L & M models.
This works by checking for a specific set a data, indicating the tablet has switched from the off state to the on state, and then sending the necessary output data to switch it between Plug & Play to Vendor Mode.

## Installation

- Open OpenTabletDriver's UX,
- Go to `Plugins > Open Plugin Manager...` in the menu bar,
- Click on the File menu, then `Install Plugin...`,
- Select either the downloaded .zip file, or the contained .dll file,

## Usage

Go the the Filter tab, a new Filter should have appeared.
Click on it, and enable it, apply & / or save.

## Dependencies (building)

- .NET 8 SDK

The above dependency is available in the Ubuntu backport package repo.
Otherwise, refer to Microsoft's documentation for more information.

## Building

Take a look at the content of the `build.sh` script.
If everything inside is known safe to use, you can then add execution permissions to it, and run it with `./build.sh` from the root of this repository.