using System.Collections.Generic;
using System.Linq;
using WardIsLove.Extensions;

namespace WardIsLove.Util
{
    public class OfflineStatus
    {
        private const string NotEnoughPlayersMessage = "Not enough players on this ward are online; ONLINE: {0}, NEEDED: {1}";
        private const string IndestructibleMessage = "All structures inside the ward are indestructible";
        private const string RaidableMessage = "Raidable";

        internal static bool CheckOfflineStatus(WardMonoscript ward, bool isPlayer = false)
        {
            List<KeyValuePair<long, string>> permittedList = ward.GetPermittedPlayers();
            List<string> playerList = ZNet.instance.GetPlayerList().Select(player => player.m_name).ToList();
            int raidProtectionPlayerNeeded = ward.GetRaidProtectionPlayerNeeded();
            int permittedOnline = permittedList.Count(permittedPlayer => playerList.Contains(permittedPlayer.Value));
            string creatorName = ward.GetCreatorName();
            bool isCreatorOnline = playerList.Contains(creatorName);

            if (IsWardRaidable(permittedList.Count, isCreatorOnline, permittedOnline, raidProtectionPlayerNeeded))
            {
                DisplayMessage(isPlayer, true, permittedOnline, raidProtectionPlayerNeeded, isCreatorOnline);
                return true;
            }

            DisplayMessage(isPlayer, false, permittedOnline, raidProtectionPlayerNeeded, isCreatorOnline);
            return false;
        }

        private static bool IsWardRaidable(int permittedListCount, bool isCreatorOnline, int permittedOnline, int raidProtectionPlayerNeeded)
        {
            if (permittedListCount == 0 && isCreatorOnline)
            {
                return raidProtectionPlayerNeeded <= 1;
            }

            int effectivePlayerCount = isCreatorOnline ? permittedOnline + 1 : permittedOnline;

            return raidProtectionPlayerNeeded >= 1 && effectivePlayerCount >= raidProtectionPlayerNeeded;
        }

        private static void DisplayMessage(bool isPlayer, bool isRaidable, int permittedOnline, int raidProtectionPlayerNeeded, bool isCreatorOnline)
        {
            if (!isPlayer || !WardIsLovePlugin.ShowraidableMessage.Value)
            {
                return;
            }

            int effectivePlayerCount = isCreatorOnline ? permittedOnline + 1 : permittedOnline;

            if (isRaidable)
            {
                Chat.m_instance.AddString("[WardIsLove]", $"<color=\"green\">{RaidableMessage}</color>", Talker.Type.Normal);
            }
            else
            {
                Chat.m_instance.AddString("[WardIsLove]", string.Format(NotEnoughPlayersMessage, effectivePlayerCount, raidProtectionPlayerNeeded), Talker.Type.Normal);
                Chat.m_instance.AddString("[WardIsLove]", $"<color=\"red\">{IndestructibleMessage}</color>", Talker.Type.Normal);
            }
        }
    }
}