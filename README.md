Another dedicated ward mod brought to you by the Author of BetterWards. Based around the idea that wards should be individually configurable. Managed with a simple GUI interface for admins.

`This mod is NOT backwards compatible with BetterWards.`

`This mod uses ServerSync, if installed on the server and the config is set to be forced, it will sync all configs to clients.`

`This mod uses a FileWatcher. If the file on the server is changed, upon saving the file, the configurations are sent to all clients.`

> ## Features
### Everything listed below is a feature in the mod. The config values dictate the `GLOBAL` defaults for these values. Admins can change the individual options via the GUI per ward.
* Easy to use GUI interface for admins to configure wards (and owners, in a limited GUI interface, if server allows via config)
* Enforced Config with server (besides client custom configs)
* Built in readiness for our Guilds mod when it releases
* Server version checking, must have mod & must be same version as server
* Default wards removed, players cannot build it anymore. Any vanilla wards will be "off" by default.
* Multiple ward models (including legacy BetterWards models)
* Add players to the permitted list no matter where they are in the world. (As long as they are online)
* Ward Limiting with VIP option for those *special* players that you want to allow more wards for (configured on the server and found in General section, thanks KG!)
    - wardIsLoveData is where the ward limit information is stored. Found in BepInEx/config folder. Keyed to SteamIDs
    - Ward "Charging" to deactivate ward after {x} in game days (configured on server)
        - The default cost of the charge is 5 Thunderstones. All wards that expire and are not charged will turn off until charged once more. Leaving it vulnerable to attack or takeover.
        - For reference. It's approximately 24 minutes for a full day cycle. Though this wiki says otherwise. https://valheim.fandom.com/wiki/Day_and_Night_Cycle
            - This means that you either have ~5 days until initial expiration or 6.25 if the wiki is correct.
            - The default expiration is 300 in-game days.
* Configurable entry and exit messages.
* Different levels of access to the ward.
* The hotkey can be adjusted by the configuration file.
*  Allow permitted users on the ward to toggle the ward on and off
*  PvP/PvE forced configurations
*  Ward Range configuration
* Different bubble modes
    * (Default) Enemy spawns will query the range to prevent spawning inside base
    * (No Monsters) Enemy monsters will not target players inside the ward's radius
* Configurable bubble colors!
* Pushout Creatures/Players. BUBBLE MUST BE ON! (If they are already inside your bubble, you deal with the consequences). This change also means that if the bubble is on, it blocks arrows and other projectiles.
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
  ... and much more! Give the mod a try!

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
   1. Right click the Valheim game in your steam library
   2. "Go to Manage" -> "Browse local files"
   3. Steam should open your game folder
2. Extract the contents of the archive. Put the DLL into BepInEx\plugins the other files are needed for the thunderstore upload and can be ignored.
3. Locate azumatt.WardIsLove.cfg under BepInEx\config and configure the mod to your needs

#### Server

`﻿Must be installed on both the client and the server for syncing to work properly.`
1. Locate your main folder manually and :
   1. Extract the contents of the archive into BepInEx\plugins.
   2. Launch your game at least once to generate the config file needed if you haven't already done so.
   3. Locate azumatt.WardIsLove.cfg under BepInEx\config on your machine and configure the mod to your needs
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
> ### How long are the days in Valheim for the ward?
>
> Approximately 20 minutes for a full day cycle. Though this wiki says otherwise. https://valheim.fandom.com/wiki/Day_and_Night_Cycle
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

## Ward Model Author (Models were commissioned for this mod and used with permissions)

### deBARBA
`Sketchfab:` https://sketchfab.com/dillonbarba

***
> # Update Information (Latest listed first)
> ### v2.3.8/2.3.9
> - Fix ongoing/random dropdown population issue.
> - Fix issue with carts, ships, tames with saddles, etc not making it into bubbles.
> - Fix Weather Damage Patch
> - Fix some logs from testing, moved to debug (v2.3.9)
> ### v2.3.7
> - Streamer special request. Prevent steam information from showing for admins. Client config option.
> ### v2.3.6
> - Gratak special request. Prevent GUI opening and option showing on hover even in singleplayer.
> ### v2.3.5
> - Accepted PR from WackyMole to add config option for Raid Notification
> - Change Raid Notification flow to check if raid is on before checking offline status. Not checking them at the same time anymore. Might reduce spam if they keep notifications on.
> ### v2.3.4
> - Compile against latest game version
> ### v2.3.3
> - PressurePlate compat
> - Update ServerSync
> - Internal removal of unused directives
> ### v2.3.2
> - Placement exploit fixed.
> - Wards from non-permitted players can no longer overlap a ward they aren't permitted on.
    >   - This also means they can not build above a ward they aren't permitted on.
> - Yuleklapps (gift containers) are now ignored by the ward, as they are in vanilla
> - Creature damage increase slider fixed.
> ### v2.3.1
> - FPS Fixes
> - Remove Autorepair until FPS performance can be increased.
> - Pushout changes to be more performant. Now only runs when they cross into your ward. Teleporting in will not activate this code anymore. Allowing players to "trap" others inside their radius. (requested)
> ### v2.3.0
> - Logout & placement exploit fix
> - Owner GUI with limited configuration options added. Found in config "Control GUI" section. `Must be allowed via server configurations! Off by default!`
> - Health and Stamina Boost changed to Passive Health/Stamina regeneration.
> - Auto Repair added back from Better Wards.
> - Additional optimizations to the ward bubble shader. Reduced textures to half their current size, and some lighting fixes to improve performance.
> ### v2.2.0
> - Original pushout code re-added to prevent clipping through the bubble using doors and other things
> - Added terrain checking
> - Optional cost to charging. (If you set the cost value to 0, the item is not needed and can charge without cost.)
> - Fix some issues with opting-in/out of the ward.
> - Remove redundancies in code.
> ### v2.0.1
> Update ServerSync version to fix some syncing issues.
> ### v2.0.0
> `It is recommended that you delete your config files and localization file when upgrading to this version`
> - Localization and default values fixed to match GUI properly.
> - Fixed Food drain bug
> - Added new ward model (Hel)
> - Added Ward Limit (configured on the server and found in General section, thanks KG!)
    >   - wardIsLoveData is where the ward limit information is stored. Found in BepInEx/config folder. Keyed to SteamIDs
> - Added ward "Charging" to deactivate ward after {x} in game days (configured on server)
    >   - The default cost of the charge is 5 Thunderstones. All wards that expire and are not charged will turn off until charged once more. Leaving it vulnerable to attack or takeover.
    >   - For reference. It's approximately 20 minutes for a full day cycle. Though this wiki says otherwise. https://valheim.fandom.com/wiki/Day_and_Night_Cycle
          >     - This means that you either have 4.16 days until initial expiration or 6.25 if the wiki is correct.
          >     - The default expiration is 300 in-game days.
> - Bubble color changes now saved and synced with clients
> - Some hover text fixes
> - Pushout Creatures/Players code changed to use collider and not code. BUBBLE MUST BE ON! (If they are already inside your bubble, you deal with the consequences). This change also means that if the bubble is on, it blocks arrows and other projectiles.
> - Default wards removed, players cannot build it anymore. Any vanilla wards will be "off" by default.
> - When the bubble causes the ward to reach half health, the bubble turns off to allow raiding of bases. (Default ward health will be x5 of original. It's now set to 5000hp)
> - Admin wards have a really long expire date (50k)
> - Creature Damage Increase option added back from BetterWards!
> - Recharging your ward now costs you. (Configurable)
> - Added workbench to ward requirements
> - Fix NRE when in Singleplayer
> - Fix ward model order to match icon/hover text. Back to the way it should be.
> - Fix ward emissions to toggle on and off when the ward does.
> - Audio for "Ward is Love?" option is now instant.
> - Add the new torches to the unlimited fire sources list
> - Generic bug fixes
> ### v1.0.2
> - Ward interaction fixes
> - Issue with saying a new version exists fixed
> - Populate player list dropdown more often (On Access tab click as well as ward GUI show)
> - Fix recipe issue. Forgot to update the defaults after moving to using prefab names
> ### v1.0.0/1.0.1
> - Initial Release
> - Features most of what BetterWards can do. Slowly adding the features back in.