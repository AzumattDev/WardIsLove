> # Update Information (Latest listed first)
> ### v3.5.11
> - Wards now push away the mist inside their radius. (When the bubble is on and ward active)
> ### v3.5.10
> - Temp "fix" for massive FPS loss when in the Ashlands. This is temporary until I basically rewrite the bubble code to use their bubble instead of mine.
> ### v3.5.9
> - Recompiled against latest game version to fix missing field due to IG code regression.
> - Github project TestingBuild configuration and water pushout code added for future plans. Also updated .NET target.
> ### v3.5.8
> - Add piece_brazierfloor01,piece_brazierfloor02 to default configurations. By request.
> ### v3.5.7
> - Reduce logs about saving the file by moving them to debug printing only.
> ### v3.5.6
> - Fix NRE when ward loads in
> ### v3.5.5
> - Fix NRE when placing ward
> - Possible fix to ward count issue
> ### v3.5.4
> - Fix possible JIT issue
> ### v3.5.3
> - Update for Ashlands, bubble effect will likely change in the coming updates.
> ### v3.5.2
> - Update for Valheim 0.217.46 and add incompatibility with BetterNetworking as I'm kinda over hearing issues when using it and this mod at the same time.
> ### v3.5.1
> - Fix issue on server boots. (Hopefully)
> ### v3.5.0
> - Fix Ward Limit issue and also have it periodically update the file based on actual ward count in the world, just in case.
> - Fix missing script warnings
> - Fix possible issues with Ships, Carts, and Tames with saddles when they enter wards. (Hopefully)
> - Other stuff I can't remember. Next update will have some configuration value additions for ward limits and charges.
> ### v3.4.5
> - Compile against latest game version
> ### v3.4.4
> - Update to latest PieceManager to fix some issues
> ### v3.4.3
> - Forgot, needed to update ServerSync in the last update.
> ### v3.4.2
> - Increase performance when logging into a server. This is caused by the version hash check. Reverted back to just version check and letting the Anticheat handle the hashing instead.
> - Generic bug fixes (no, ward limit is not fixed yet still working on that passively)
> ### v3.4.1
> - Fix the radius issue with the ward that started in 3.3.0 (I think)
> - Fix issue with the Pickable Protection being backwards when enabled/disabled
> - Make internal changes to my code project to build cleaner
> ### v3.4.0
> - `WARNING: SIZEABLE CHANGE`
>   - The internal time for each ward has been moved to real time (based on the server's time). It's highly recommended that you destroy and rebuild your wards or charge each ward over again to get the new server times, otherwise they will auto expire or not function correctly after updating to this version.
>   - The default ward time is now 30 days. This is configurable on the server. Admin ward time never expires
> - Fix the hover text to be a bit more pretty (like it used to be)
> - Change default ward requirements to allow wards faster. (Requested)
>   - It's now 10 FineWood, 10 SurtlingCore and 1 Thunderstone
>   - Was 15 Silver, 30 SurtingCore, 1 Thunderstone
> - Check ZDO values using hashes directly instead of strings like vanilla now does for most things.
> - Change Offline Raid Checking code to be easier to read and understand.
> - Fix for the terminal commands 
> ### v3.3.1
> - Hotfix for Guilds, I was dumb and forgot to uncomment stuff.
> ### v3.3.0
> - Fix issue where ward damage amount being cleared would throw an error.
> - Add Guilds integration. If you set the permissions on your ward to Guild, only guild members will be permitted
> - Fix a missing localization key
> - Internal "steamName" that is stored on the ward is now able to store the playfab/xbox IDs into it. The key is still "steamName" to provide backwards compatibility.
> - Extend the Charge Item config to allow for multiple items to be used to charge the ward. Comma delimited list of item names and amounts. New Format is "ItemName:Amount,ItemName:Amount,ItemName:Amount"
>   - Please note: This means that the Charge Item Amount config is no longer used. It has been removed, you might need to regenerate your config file.
>     - Regenerate your configuration file by deleting the old one and restarting the game with the mod installed. I recommend you keep a backup of your old config file for reference if needed.
> ### v3.2.3
> - Update for latest Valheim changes
> ### v3.2.2
> - Update for Hildir's Request. 0.217.14
> ### v3.2.1
> - Minor fix
> ### v3.2.0
> - Updated for Valheim  0.216.9
> ### v3.1.2
> - Don't pushout/damage creatures if they are in the Player faction. This should fix an issue with Cheb's mods that add creatures to the player faction.
> ### v3.1.1
> - Fix some visual effects on the ward when placed.
> - Add a new configuration option under the UI section to change the Canvas Scalar. This should help with screens bigger than 1080p.
> ### v3.1.0
> - Various internal updates that related to RPCs so that they are a bit more unique to this mod. Mainly because I think I copied them too much into others :)
    >   - This hopefully will prevent other mods from calling the RPCs and causing repeated ward messages. If this doesn't fix it, I'll have to look into it more.
> - Hopefully fix some NREs that were happening with the ward.
> - Fix the hover text now that they have moved to TextMeshPro
> - Convert the server's wardIsLoveData file over to YAML. This should make it easier to read and edit.
  >
>  - The old file will be deleted and a new one will be created. The new file is called `Azumatt.WardIsLoveData.yml`
   >
>   - The new file should contain the same data as the old one, but it will be in a different format. You shouldn't lose any data, so not to worry there.
> - Update to my latest PieceManager code
> ### v3.0.9
> - Update to the health of the ward to fix instant bubble pop
> ### v3.0.8
> - Mistlands Update
> ### v3.0.7
> - Minor fix to PieceManager to prevent a null reference exception
> - Remove GetCenterPoint function all together. Just attempt to hit Vector3.Zero, should fix that NRE with custom creatures, but damage text will no longer show.
> ### v3.0.6
> - Update ServerSync internally again.
> ### v3.0.5
> - Update ServerSync internally
> - Make GUID of the mod be the same as my other mods (Azumatt vs azumatt as the prefix)
> - Update PieceManager code to my latest.
> - Update Admin check for crossplay
> - `NOTE:` A real update will be coming to this mod soon. This is just a quick fix to get it working again.
> ### v3.0.4
> - <strong>Performance for the bubble has been improved. It should be less intensive on your machine. Vertices have been reduced by over half!</strong>
> - Fixed the feedback gui, it wasn't sending to discord initially if you typed too much, or had some weird special characters in the inputs.
> - Fixed the inputs on the feedback panel for the GUI. They should now wrap text properly and look a bit better. Also, limit the input to the size discord accepts.
> - Attempt to fix the issue with ward entry. I have never been able to replicate the issue, but after working with multiple people who make custom creatures, I'm pretty sure this is the fix we have been waiting for.
> - Fixed the recipe changing back to the forge even when the setting was set to None.
> - Only disable vanilla wards, not other mods pieces should they have a PrivateArea component.
> ### v3.0.3
> - Don't damage tames.
> ### v3.0.2
> - Fixed where the GetCharacterPoint() function would fail, I was not ending the the coroutine properly.
> - Fixed damage timing issue where the damage interval time was triggering hits too quickly.
> - Re-added my deleted code from the previous version. I deleted too much when moving to my PieceManager. This should
    mean the original wards are removed like they should be, and other things related to the hammer.
> ### v3.0.0/v3.0.1
> - Move the ward recipe over to my PieceManager
    code. `You WILL need to regenerate your config file. There are many changes to it, that if you don't will cause issues. I recommend backing up the previous configuration for easy reference to what you had before.`
> - Attempt to optimize the bubble in Unity and a small bit in the code.
> - Move the WardHotKey (toggle ward when permitted key) over to a keyboard shortcut. Allows you to choose more than one
    key to be used for the interaction.
> - Add ZDO data buffer to provide compatibility with Networking mods
> - Armor stands are now protected
> - Ship interaction bug/toggle not working as intended has been fixed.
> - Other changes I can't remember.
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
> - Change Raid Notification flow to check if raid is on before checking offline status. Not checking them at the same
    time anymore. Might reduce spam if they keep notifications on.
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
> - Pushout changes to be more performant. Now only runs when they cross into your ward. Teleporting in will not
    activate this code anymore. Allowing players to "trap" others inside their radius. (requested)
> ### v2.3.0
> - Logout & placement exploit fix
> - Owner GUI with limited configuration options added. Found in config "Control GUI"
    section. `Must be allowed via server configurations! Off by default!`
> - Health and Stamina Boost changed to Passive Health/Stamina regeneration.
> - Auto Repair added back from Better Wards.
> - Additional optimizations to the ward bubble shader. Reduced textures to half their current size, and some lighting
    fixes to improve performance.
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
    >   - wardIsLoveData is where the ward limit information is stored. Found in BepInEx/config folder. Keyed to
    SteamIDs
> - Added ward "Charging" to deactivate ward after {x} in game days (configured on server)
    >   - The default cost of the charge is 5 Thunderstones. All wards that expire and are not charged will turn off
    until charged once more. Leaving it vulnerable to attack or takeover.
    >   - For reference. It's approximately 20 minutes for a full day cycle. Though this wiki says
    otherwise. https://valheim.fandom.com/wiki/Day_and_Night_Cycle
    >     - This means that you either have 4.16 days until initial expiration or 6.25 if the wiki is correct.
    >     - The default expiration is 300 in-game days.
> - Bubble color changes now saved and synced with clients
> - Some hover text fixes
> - Pushout Creatures/Players code changed to use collider and not code. BUBBLE MUST BE ON! (If they are already inside
    your bubble, you deal with the consequences). This change also means that if the bubble is on, it blocks arrows and
    other projectiles.
> - Default wards removed, players cannot build it anymore. Any vanilla wards will be "off" by default.
> - When the bubble causes the ward to reach half health, the bubble turns off to allow raiding of bases. (Default ward
    health will be x5 of original. It's now set to 5000hp)
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