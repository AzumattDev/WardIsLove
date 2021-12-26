using System;
using System.Linq;
using HarmonyLib;

namespace WardIsLove.Util
{
    [HarmonyPatch]
    public static class ObjectDBWrapper
    {
        /* Wrap the ObjectDB to find items */
        public static ItemDrop GetItem(string name)
        {
            return ObjectDB.instance.m_items.Select(gameObject => gameObject.GetComponent<ItemDrop>())
                       .FirstOrDefault(component => component.m_itemData.m_shared.m_name == name) ??
                   throw new InvalidOperationException();
        }
        
        /* Wrap the ObjectDB to find the prefab */
        internal static ItemDrop? GetItemWithPrefab(string prefabName, string wardName)
        {
            try
            {
                return ObjectDB.instance.GetItemPrefab(prefabName).GetComponent<ItemDrop>();
            }
            catch
            {
                WardIsLovePlugin.WILLogger.LogDebug($"Error grabbing the prefab name from the ObjectDB that is set in the ward requirements for {wardName}");
                return null;
            }
        }
    }
}