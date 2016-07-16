# MiniAVC Add-on Version Checker

## Developer

Cybutek


## KSP-AVC Ready

KSP Add-on Version Checker is a standardised system for versioning mods. You can get more information on the
[forum thread](http://forum.kerbalspaceprogram.com/threads/79745)


## Description

This plugin checks inside its current directory and all contained directories for version files.  It's use is for mods to make use of the KSP-AVC system without requiring the player to have the full KSP-AVC Plugin installed.  The MiniAVC.dll plugin is safe to be bundled with mods and will negotiate between all other instances of MiniAVC to run the latest version.  If an installation of KSP-AVC Plugin is found, the MiniAVC system will be disabled to let KSP-AVC take over.  MiniAVC can check for both updates and game version compatibility, the same way as KSP-AVC Plugin does.  The biggest difference is that because MiniAVC is a bundleable plugin, it will ask the player on the first run whether to allow update checking.  If the user does not wish your add-on to check for updates, the remote checking functionality will be disabled.  This will not completely turn off MiniAVC as it can still run in local mode to notify the player of any game version compatibility issues.


## Installation

 - Bundle the MiniAVC.dll file into your packaged add-on directory along with your version file.
 - If your add-on contains multiple version files, place it at the lowest directory level which will cover all the version files, but do not place it in GameData.			</ul>


## Community Add-on Rule 5.5

The rule states that, "Add-ons that contact another network or computer system must tell users exactly what it's sending or receiving in a clear and obvious way in all locations it is offered for download." This means that if you bundle MiniAVC with your add-on, you must clearly notify players before downloading that it contains MiniAVC and how it works.		

### BB Code
```
This mod includes version checking using [URL=http://forum.kerbalspaceprogram.com/threads/79745]MiniAVC[/URL]. If you opt-in, it will use the internet to check whether there is a new version available. Data is only read from the internet and no personal information is sent. For a more comprehensive version checking experience, please download the [URL=http://forum.kerbalspaceprogram.com/threads/79745]KSP-AVC Plugin[/URL].
```
### HTML
```html
This mod includes version checking using <a href="http://forum.kerbalspaceprogram.com/threads/79745">MiniAVC</a>. If you opt-in, it will use the internet to check whether there is a new version available. Data is only read from the internet and no personal information is sent. For a more comprehensive version checking experience, please download the <a href="http://forum.kerbalspaceprogram.com/threads/79745">KSP-AVC Plugin</a>.
```

## Version File Breakdown

 - **NAME** - Required

   The display name for the add-on.

 - **URL** - Optional

    Location of a remote version file for update checking.

 - **DOWNLOAD** - Optional

    Web address where the latest version can be downloaded.
    *This is only used from the remote version file.*

 - **CHANGE_LOG** - Optional

   The complete or incremental change log for the add-on.
   This is only used from the remote version file.

 - **CHANGE_LOG_URL** - Optional

   Populates the CHANGE_LOG field using the file at this url.
   *This is only used from the remote version file.*

 - **GITHUB** - Optional

   Allows KSP-AVC to do release checks with GitHub including setting a download location if one is not specified.
   If the latest release version is not equal to the version in the file, an update notification will not appear.
   *This is only used from the remote version file.*

   - **USERNAME** - Required

     Your GitHub username.

   - **REPOSITORY** - Required

     The name of the source repository.

   - **ALLOW_PRE_RELEASE** - Optional

     Include pre-releases in the latest release search.

     *The default value is false.*

 - **VERSION** - Required

    The version of the add-on.
- **KSP_VERSION** - Optional, Required for MIN/MAX

    Version of KSP that the add-on was made to support.
- **KSP_VERSION_MIN** - Optional

    Minimum version of KSP that the add-on supports.

    *Requires KSP_VERSION field to work.*

- **KSP_VERSION_MAX** - Optional

    Maximum version of KSP that the add-on supports.
    *Requires KSP_VERSION field to work.*
    
For simple management of your version files you can use the KSP-AVC Online website at: [ksp-avc.cybutek.net](http://ksp-avc.cybutek.net/)

## Version File Example

```json
{
    "NAME":"KSP-AVC",
    "URL":"http://ksp-avc.cybutek.net/version.php?id=2",
    "DOWNLOAD":"http://kerbal.curseforge.com/ksp-mods/220462-ksp-avc-add-on-version-checker",
    "GITHUB":
    {
        "USERNAME":"YourGitHubUserName",
        "REPOSITORY":"YourGitHubRepository",
        "ALLOW_PRE_RELEASE":false,
    },
    "VERSION":
    {
        "MAJOR":1,
        "MINOR":1,
        "PATCH":0,
        "BUILD":0
    },
    "KSP_VERSION":
    {
        "MAJOR":0,
        "MINOR":24,
        "PATCH":2
    },
    "KSP_VERSION_MIN":
    {
        "MAJOR":0,
        "MINOR":24,
        "PATCH":0
    },
    "KSP_VERSION_MAX":
    {
        "MAJOR":0,
        "MINOR":24,
        "PATCH":2
    }
}
```

## Changelog

See [CHANGES.txt](CHANGES.txt)

## Software License

Licensed under the [GNU General Public License v3](LICENSE.txt).
