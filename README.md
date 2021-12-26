Another dedicated ward mod brought to you by the Author of BetterWards. Based around the idea that wards should be individually configurable. Managed with a simple GUI interface for admins.

`This mod is NOT backwards compatible with BetterWards.`
> ## Features
### Everything listed below is a feature in the mod. The config values dictate the `GLOBAL` defaults for these values. Admins can change the individual options via the GUI per ward.
* Easy to use GUI interface for admins to configure wards
* Enforced Config with server (besides client custom configs)
* Built in readiness for our Guilds mod when it releases
* Server version checking, must have mod & must be same version as server
* Multiple ward models (including legacy BetterWards models)
* Add players to the permitted list no matter where they are in the world. (As long as they are online)
* Configurable entry and exit messages.
* Different levels of access to the ward.
* The hotkey can be adjusted by the configuration file.
*  Allow permitted users on the ward to toggle the ward on and off
*  PvP/PvE forced configurations
*  Ward Range configuration
* Different bubble modes
  * (Default) Enemy spawns will query the range to prevent spawning inside base
  * (No Monsters) Enemy monsters will not target players inside the ward's radius
* Health/Stamina Boosts for all players inside ward
* Show area marker for the ward
* Indestructible structures can be defined for further custom config (inside ward) (FULL LIST IN CONFIG)
* Auto Close Doors inside ward
* Reduce damage to player structures inside ward from other players, or increase default health of structure (applies to all inside ward)
* Notification of being inside a ward and who the owner is (configurable)
* Sync Admin list from server for additional configurations (auto permit on nearby wards, permit/unpermit/enable/disable/flash with command)
* Warded portal interact/teleport
* Ward pushout for creatures and players
* Unlimited fireplace fuel in warded area
* Configurable interaction in warded area
* No Weather Damage
* Visual Bubble
* Offline Raid protection for PvP servers
* Configurable ward recipe to craft
* Auto repair structures inside ward
* No food drain inside ward

> ## Client Custom Config Options

* Hotkey option for ward toggle (Default is "G") for permitted players.
* Auto Close Doors (enable/disable).

> ## Admin Only

* Hotkeys are UpArrow for enabling a ward, DownArrow for disabling a ward (must be looking at it)
* Auto permit on enabled wards nearby (client configurable, having this option off means you play as a normal player)
* Admin only chat/console(F5) commands (permit, unpermit, enable, flash, disable)


> ## Installation Instructions
***You must have BepInEx installed correctly! I can not stress this enough.***

#### Windows (Steam)
1. Locate your game folder manually or start Steam client and :
   a. Right click the Valheim game in your steam library
   b. "Go to Manage" -> "Browse local files"
   c. Steam should open your game folder
2. Extract the contents of the archive into the game folder.
3. Locate azumatt.WardIsLove.cfg under BepInEx\config and configure the mod to your needs

#### Server

`﻿Must be installed on both the client and the server for syncing to work properly.`
1. Locate your main folder manually and :
   a. Extract the contents of the archive into the main folder that contains BepInEx
   b. Launch your game at least once to generate the config file needed if you haven't already done so.
   c. Locate azumatt.WardIsLove.cfg under BepInEx\config on your machine and configure the mod to your needs
2. Reboot your server. All clients will now sync to the server's config file even if theirs differs. Config Manager mod changes will only change the client config, not what the server is enforcing.


`Feel free to reach out to me on discord if you need manual download assistance.`

> ## FAQ
> ### What if the game updates?
> 
> Game updates are unlikely to do more than partially break Better Wards at worst. In case you encounter any issues, please reach out to the me.
>
> ### What if the wards overlap?
>
> The hope is that the wards should behave correctly, even if overlapped.
>
> ### Where is the configuration file?
>
> The Config file's name is "azumatt.WardIsLove.cfg" it needs to be placed in "BepInEx\config"

> ## Known Mod Conflicts
<details> <summary></summary>

 * Anything that toggles PvE/PvP and forces the value will conflict if you have this mod toggle the values. Current known mods that do this are:

﻿World of Valheim - Zones

﻿PvP-Always-On
</details>



# Author Information

### Azumatt

`DISCORD:` Azumatt#2625

`STEAM:` https://steamcommunity.com/id/azumatt/﻿

For Questions or Comments, find me﻿ in the Odin Plus Team Discord:
[![https://i.imgur.com/XXP6HCU.png](https://i.imgur.com/XXP6HCU.png)](https://discord.gg/Pb6bVMnFb2)

***
> # Update Information (Latest listed first)
> ### v1.0.0/1.0.1
> - Initial Release
> - Features most of what BetterWards can do. Slowly adding the features back in.