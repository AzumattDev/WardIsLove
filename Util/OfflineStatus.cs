using System.Collections.Generic;
using System.Linq;
using WardIsLove.Extensions;

namespace WardIsLove.Util
{
    public class OfflineStatus
    {
        internal static bool CheckOfflineStatus(WardMonoscript ward)
        {
            bool flag = false;
            string message = "";
            if (ward.IsPermitted(Player.m_localPlayer.GetPlayerID()))
            {
                flag = true;
                return flag;
            }
            else
            {
                List<KeyValuePair<long, string>> permittedList = ward.GetPermittedPlayers();
                List<string> stringList = ZNet.instance.GetPlayerList().Select(player => player.m_name).ToList();
                int raidProtectionPlayerNeeded = ward.GetRaidProtectionPlayerNeeded();
                bool protectionOn = ward.GetRaidProtectionOn();
                int permittedOnline =
                    permittedList.Count(permittedPlayer => stringList.Contains(permittedPlayer.Value));

                switch (permittedList.Count)
                {
                    /* If the permitted list is empty, but the owner is found in the PlayerList, compare raidable player count to determine raidable status*/
                    case 0 when stringList.Contains(ward.GetCreatorName()):
                    {
                        flag = raidProtectionPlayerNeeded <= 1;
                        return flag;
                    }
                    
                    /* If owner is online and people from the permitted list are online, compare raidable count */
                    case > 0 when stringList.Contains(ward.GetCreatorName()):
                    {
                        if (raidProtectionPlayerNeeded >= 1 && (permittedOnline + 1 >= raidProtectionPlayerNeeded))
                        {
                            flag = true;
                            return flag;
                        }

                        break;
                    }
                    /* If owner is not online and people from the permitted list are online, compare raidable count */
                    case > 0 when !stringList.Contains(ward.GetCreatorName()):
                    {
                        if (raidProtectionPlayerNeeded >= 1 && (permittedOnline >= raidProtectionPlayerNeeded))
                        {
                            flag = true;
                            return flag;
                        }

                        break;
                    }
                }

                /* Display the appropriate message to the player */
                switch (flag)
                {
                    case false:
                        if (stringList.Contains(ward.GetCreatorName()))
                        {
                            Chat.m_instance.AddString("[WardIsLove]",
                                $"<color=\"red\">Not enough players on this ward are online; ONLINE: {permittedOnline + 1}, NEEDED: {raidProtectionPlayerNeeded}</color>",
                                Talker.Type.Normal);
                            Chat.m_instance.AddString("[WardIsLove]",
                                "<color=\"red\">All structures inside the ward are indestructible</color>",
                                Talker.Type.Normal);
                        }
                        else
                        {
                            Chat.m_instance.AddString("[WardIsLove]",
                                $"<color=\"red\">Not enough players on this ward are online; ONLINE: {permittedOnline}, NEEDED: {raidProtectionPlayerNeeded}</color>",
                                Talker.Type.Normal);
                            Chat.m_instance.AddString("[WardIsLove]",
                                "<color=\"red\">All structures inside the ward are indestructible</color>",
                                Talker.Type.Normal);
                        }

                        break;
                    case true:
                        Chat.m_instance.AddString("[WardIsLove]", "<color=\"green\">Raidable</color>",
                            Talker.Type.Normal);
                        break;
                }

                WardIsLovePlugin.WILLogger.LogDebug(
                    "None of the offline criteria matched. Assume that the base shouldn't be raidable.");
                return false;
            }
        }
    }
}