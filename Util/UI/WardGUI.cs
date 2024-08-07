﻿using System;
using PieceManager;
using UnityEngine;
using UnityEngine.UI;
using WardIsLove.PatchClasses;
using WardIsLove.Util.Bubble;
using Object = UnityEngine.Object;

namespace WardIsLove.Util.UI
{
    public class WardGUI
    {
        public static GameObject wardGUI = null!;
        public static GameObject wardGUINoAdmin = null!;
        public static WardMonoscript interactedWard = null!;
        public static Dropdown EffectAreaDropdown = null!;
        public static Dropdown FeedbackDropdown = null!;
        public static Text EffectAreaDropdownText = null!;
        public static Text FeedbackDropdownText = null!;
        public static string FeedbackDropdownValue = null!;
        public static bool setUILit = false;

        public static bool IsPanelVisible()
        {
            return (wardGUI && wardGUI.activeSelf) || (wardGUINoAdmin && wardGUINoAdmin.activeSelf);
        }

        public static void Init()
        {
            AssetBundle wardMenuBundle = WardIsLovePlugin.GetAssetBundle("wardislove");

            WardIsLovePlugin.Thorward = new BuildPiece(wardMenuBundle, "Thorward");
            WardIsLovePlugin.Thorward.Name.English("Thorward");
            WardIsLovePlugin.Thorward.Description.English("The power of Thor stored in order to protect you.");
            WardIsLovePlugin.Thorward.RequiredItems.Add("FineWood", 10, true);
            WardIsLovePlugin.Thorward.RequiredItems.Add("SurtlingCore", 10, true);
            WardIsLovePlugin.Thorward.RequiredItems.Add("Thunderstone", 1, true);
            WardIsLovePlugin.Thorward.Category.Set(BuildPieceCategory.Misc);
            WardIsLovePlugin.Thorward.Crafting.Set(CraftingTable.Forge);
            GameObject go2 = wardMenuBundle.LoadAsset<GameObject>("Assets/CustomItems/Wards/WardIsLoveGUINoAdmin.prefab");
            GameObject go = wardMenuBundle.LoadAsset<GameObject>("Assets/CustomItems/Wards/WardIsLoveGUI.prefab");
            WardIsLovePlugin.LightningVFX = PiecePrefabManager.RegisterPrefab(wardMenuBundle, "wardlightningAOE");
            PiecePrefabManager.RegisterPrefab(wardMenuBundle, "wardlightningActivation");
            try
            {
                WardIsLovePlugin.Thorward.Prefab.GetComponent<WardMonoscript>().m_bubble.AddComponent<CollisionBubble>();

                wardGUI = Object.Instantiate(go);
                Object.DontDestroyOnLoad(wardGUI);
                wardGUINoAdmin = Object.Instantiate(go2);
                Object.DontDestroyOnLoad(wardGUINoAdmin);
                FeedbackDropdown = wardGUI.transform.Find("Canvas/UI/panel/Tabs/FeedbackPanel/FeedbackType/FeedbackTypeDropdown").GetComponent<Dropdown>();
                FeedbackDropdownText = wardGUI.transform.Find("Canvas/UI/panel/Tabs/FeedbackPanel/FeedbackType/FeedbackTypeDropdown/Label").GetComponent<Text>();
                FeedbackDropdown.onValueChanged.AddListener(DropdownSelection);

                FeedbackDropdown.ClearOptions();
            }
            catch
            {
                // ignored
            }

            for (int i = 0; i < Enum.GetNames(typeof(WardIsLovePlugin.WardGUIFeedbackEnums)).Length; ++i)
            {
                string en = GetString((WardIsLovePlugin.WardGUIFeedbackEnums)i);

                FeedbackDropdown.options.Add(new Dropdown.OptionData { text = en });
            }

            /////Here initialize UI (write the data you want to text, etc.)
            wardGUI.SetActive(false);
            wardGUINoAdmin.SetActive(false);
        }


        private static void DropdownSelection(int arg0)
        {
            WardIsLovePlugin.WardGUIFeedbackEnums en = (WardIsLovePlugin.WardGUIFeedbackEnums)arg0;

            FeedbackDropdownValue = en.ToString();
        }

        private static string GetString(WardIsLovePlugin.WardGUIFeedbackEnums me)
        {
            return me switch
            {
                WardIsLovePlugin.WardGUIFeedbackEnums.Feedback => Localization.instance.Localize("$wardmenu_optionfeedback"),
                WardIsLovePlugin.WardGUIFeedbackEnums.Bug => Localization.instance.Localize("$wardmenu_optionbug"),
                WardIsLovePlugin.WardGUIFeedbackEnums.Idea => Localization.instance.Localize("$wardmenu_optionidea"),
                _ => "ERROR"
            };
        }

        public static void Hide()
        {
            ShowCursor(false);
            wardGUI.SetActive(false);
            wardGUINoAdmin.SetActive(false);
        }

        public static void Show(WardMonoscript ward)
        {
            Utils.FindChild(wardGUINoAdmin.transform, "Canvas").GetComponent<CanvasScaler>().uiScaleMode = WardIsLovePlugin.CanvasScaleMode.Value;
            Utils.FindChild(wardGUI.transform, "Canvas").GetComponent<CanvasScaler>().uiScaleMode = WardIsLovePlugin.CanvasScaleMode.Value;
            SetInteractedPa(ward);
            if (!setUILit)
            {
                Image? wardGuiBkg = Utils.FindChild(wardGUI.transform, "panel").GetComponent<Image>();
                Image? wardGuiNoAdminBkg = Utils.FindChild(wardGUINoAdmin.transform, "panel").GetComponent<Image>();
                wardGuiBkg.material = GetLitPanelMaterial.originalMaterial;
                wardGuiNoAdminBkg.material = GetLitPanelMaterial.originalMaterial;
                setUILit = true;
            }

            if (ward.m_piece.IsCreator() && !WardIsLovePlugin.Admin)
            {
                wardGUINoAdmin.SetActive(true);
            }
            else
            {
                wardGUI.SetActive(true);
            }

            ShowCursor(true);
        }

        private static void SetInteractedPa(WardMonoscript pa)
        {
            interactedWard = pa;
        }

        internal static WardMonoscript PassInWardMonoscriptToGui()
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