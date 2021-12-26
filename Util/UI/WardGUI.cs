using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace WardIsLove.Util.UI
{
    public class WardGUI
    {
        public static GameObject wardGUI;
        public static WardMonoscript interactedWard;
        public static Dropdown EffectAreaDropdown;
        public static Dropdown FeedbackDropdown;
        public static Text EffectAreaDropdownText;
        public static Text FeedbackDropdownText;
        public static string FeedbackDropdownValue;

        public static bool IsPanelVisible()
        {
            return wardGUI && wardGUI.activeSelf;
        }

        public static void Init()
        {
            AssetBundle wardMenuBundle = WardIsLovePlugin.GetAssetBundle("wardislove");
            //AssetBundle wardMenuBundle2 = WardIsLovePlugin.GetAssetBundle("guildfabs");
            GameObject go = wardMenuBundle.LoadAsset<GameObject>("Assets/CustomItems/Wards/WardIsLoveGUI.prefab");
            try
            {
                WardIsLovePlugin.Thorward =
                    wardMenuBundle.LoadAsset<GameObject>("Assets/CustomItems/Wards/Thorward.prefab");


                wardGUI = Object.Instantiate(go);
                Object.DontDestroyOnLoad(wardGUI);
                FeedbackDropdown = wardGUI.transform
                    .Find("Canvas/UI/panel/Tabs/FeedbackPanel/FeedbackType/FeedbackTypeDropdown")
                    .GetComponent<Dropdown>();
                FeedbackDropdownText = wardGUI.transform
                    .Find("Canvas/UI/panel/Tabs/FeedbackPanel/FeedbackType/FeedbackTypeDropdown/Label")
                    .GetComponent<Text>();
                FeedbackDropdown.onValueChanged.AddListener(DropdownSelection);

                FeedbackDropdown.ClearOptions();
            }
            catch
            {
            }

            for (int i = 0; i < Enum.GetNames(typeof(WardIsLovePlugin.WardGUIFeedbackEnums)).Length; i++)
            {
                string? en = GetString((WardIsLovePlugin.WardGUIFeedbackEnums)i);

                FeedbackDropdown.options.Add(new Dropdown.OptionData { text = en });
            }

            /////Here initialize UI (write the data you want to text, etc.
            wardGUI.SetActive(false);
        }


        private static void DropdownSelection(int arg0)
        {
            WardIsLovePlugin.WardGUIFeedbackEnums en = (WardIsLovePlugin.WardGUIFeedbackEnums)arg0;

            FeedbackDropdownValue = en.ToString();
        }

        public static string GetString(WardIsLovePlugin.WardGUIFeedbackEnums me)
        {
            switch (me)
            {
                case WardIsLovePlugin.WardGUIFeedbackEnums.Feedback:
                    return Localization.instance.Localize("$wardmenu_optionfeedback");
                case WardIsLovePlugin.WardGUIFeedbackEnums.Bug:
                    return Localization.instance.Localize("$wardmenu_optionbug");
                case WardIsLovePlugin.WardGUIFeedbackEnums.Idea:
                    return Localization.instance.Localize("$wardmenu_optionidea");
                default:
                    return "ERROR";
            }
        }

        public static void Hide()
        {
            ShowCursor(false);
            wardGUI.SetActive(false);
        }

        public static void Show(WardMonoscript ward)
        {
            ZNetView znet = ward.m_nview;
            int accessMode = znet.GetZDO().GetInt("bubbleMode");

            SetInteractedPa(ward);

            wardGUI.SetActive(true);
            ShowCursor(true);
        }

        public static void SetInteractedPa(WardMonoscript pa)
        {
            interactedWard = pa;
        }

        public static WardMonoscript PassInWardMonoscriptToGui()
        {
            return interactedWard;
        }

        public static void ShowCursor(bool flag)
        {
            if (!Player.m_localPlayer) return;

            if (flag)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                GameCamera.instance.enabled = false;
                Player.m_localPlayer.GetComponent<PlayerController>().enabled = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                GameCamera.instance.enabled = true;
                Player.m_localPlayer.GetComponent<PlayerController>().enabled = true;
            }
        }
    }
}