using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using WardIsLove.Extensions;
using WardIsLove.Util.UI;

namespace WardIsLove.Util.Bubble
{
    [HarmonyPatch]
    public class WardEntryDetector : MonoBehaviour
    {
        public WardMonoscript m_wardEntered;

        private void OnTriggerEnter(Collider collider)
        {
            Player component = collider.GetComponent<Player>();
            if (component == null || Player.m_localPlayer != component)
                return;
            if (m_wardEntered.IsEnabled() && m_wardEntered.GetWardNotificationsOn())
                Player.m_localPlayer.Message(MessageHud.MessageType.Center,
                    string.Format(m_wardEntered.GetWardEnterNotifyMessage(), m_wardEntered.GetCreatorName()));

            /* Send the player a message about the raidable status on entry as well */
            if (m_wardEntered.GetRaidProtectionOn())
                OfflineStatus.CheckOfflineStatus(m_wardEntered);

            //SendWardMessage(m_wardEntered, component.GetPlayerName(), "Entered", component.GetPlayerID());
        }

        private void OnTriggerExit(Collider collider)
        {
            Player component = collider.GetComponent<Player>();
            if (component == null || Player.m_localPlayer != component)
                return;
            if (m_wardEntered.IsEnabled() && m_wardEntered.GetWardNotificationsOn())
                Player.m_localPlayer.Message(MessageHud.MessageType.Center,
                    string.Format(m_wardEntered.GetWardExitNotifyMessage(), component.GetPlayerName()));
            //SendWardMessage(m_wardEntered, component.GetPlayerName(), "Exited", component.GetPlayerID());
        }


        public void SendWardMessage(WardMonoscript ward, string playerName, string detection, long playerID)
        {
            // if (string.IsNullOrWhiteSpace(m_text.text.ToString())) return;
            long playerId = Game.instance.GetPlayerProfile().m_playerID;
            _ = Task.Run(async () =>
            {
                string? asyncResult =
                    await WardGUIUtil.GetAsync("https://wardislove-13a2b-default-rtdb.firebaseio.com/WardIsLove.json");
                string link = asyncResult.Trim('"');
                string messageSent = detection == "Exited"
                    ? string.Format(m_wardEntered.GetWardExitNotifyMessage(), playerName)
                    : string.Format(m_wardEntered.GetWardEnterNotifyMessage(), m_wardEntered.GetCreatorName());
                print(link);
                string json =
                    $@"{{""username"":""WardIsLove v{WardIsLovePlugin.version}"",""avatar_url"":""https://i.imgur.com/CzwaEed.png""," +
                    $@"""embeds"":[{{""title"":""{playerName}"",""description"":""" + detection + " ward" +
                    @""",""color"":15258703,""fields"":[{""name"":""Ward Owner"",""value"":""" + ward.GetCreatorName() +
                    @""",""inline"":true},{""name"":""Permitted"",""value"":""" + ward.IsPermitted(playerID) +
                    @""",""inline"":true},{""name"":""Message Shown To Player"",""value"":""" + messageSent +
                    @""",""inline"":false}]}]}";
                WardGUIUtil.SendMSG(link, json);
            });
        }
    }
}