KSP Add-on Version Checker
==========================

Description
-----------
On starting KSP this plugin will search your game directory for '.version' files.  It will then proceed to check each
one for version related issues.  If any issues are found, an issue monitor will be displayed notifying you of what they
are.  There are two main types of version issues, with the first being related to the add-on's version and the second
being its compatibility with your version of KSP.  Some add-ons will also support a download option in their version
check.  If you require an update of an add-on and it has a download location set, you will be given a button which will
open up your default browser and take you there.  This could link directly to the .zip file or to a page with details on
how to update.  Note that you will need to close down KSP, install the updates and then restart KSP for them to work.


Installation
------------
Copy the complete folder and contents of KSP-AVC into the GameData directory within your Kerbal Space Program installation.


Information for Developers
--------------------------
To make use of this tool, you must supply a '.version' file in with your add-on.  This file is in the standard JSON format
so it is easily portable as well as potentially human readable.

Here's an example of a '.version' file:

{
	"NAME":"KSP-AVC",
	"URL":"http://ksp-avc.cybutek.net/version.php?id=2",
	"DOWNLOAD":"",
	"VERSION":{
		"MAJOR":1,
		"MINOR":0,
		"PATCH":1,
		"BUILD":0
	},
	"KSP_VERSION":{
		"MAJOR":0,
		"MINOR":23,
		"PATCH":5
	},
	"KSP_VERSION_MIN":{
		"MAJOR":0,
		"MINOR":23,
		"PATCH":5
	},
	"KSP_VERSION_MAX":{
		"MAJOR":0,
		"MINOR":23,
		"PATCH":5
	}
}

A breakdown of the elements of this file are:

NAME			Display name of the add-on.
URL				Remote .version file used for update checking.
DOWNLOAD 		Location where the latest version can be downloaded. (Only used in the remote .version file)(Optional)
VERSION 		Version of your add-on. (MAJOR, MINOR, PATCH, BUILD)(KSP-AVC also supports a simple "VERSION":"1.0.0" string.)
KSP_VERSION 	Version of KSP which the add-on was intended to run on. (MAJOR, MINOR, PATCH)(Overrides min/max compatibility)(Optional)
KSP_VERSION_MIN	Minimum version of KSP compatible with the add-on. (MAJOR, MINOR, PATCH)(Optional)
KSP_VERSION_MAX Maximum version of KSP compatible with the add-on. (MAJOR, MINOR, PATCH)(Optional)

For simple management of your '.version' files you can use the KSP-AVC Online website at: ksp-avc.cybutek.net


License (GPLv3)
---------------
Copyright (C) 2014 CYBUTEK

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.