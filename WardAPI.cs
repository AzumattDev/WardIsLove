using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WardIsLove.Util;

namespace WardAPI;

[PublicAPI]
public class WardIsLove_API
{

    private static readonly bool _IsInstalled;
    private static MethodInfo DisableWardPlayerIsInAPI;

    public static bool IsInstalled() => _IsInstalled;
    public static void DisableWardPlayerIsIn(Vector3 PlayerLocation, bool DestroyPiece = false)
    {
        DisableWardPlayerIsInAPI?.Invoke(null, new object[] { PlayerLocation, DestroyPiece });
    }


    static WardIsLove_API()
    {
        if (Type.GetType("WardAPI.Ward_API, WardIsLove") == null)
        {
            _IsInstalled = false;
            return;
        }
        Type WILAPI = Type.GetType("WardAPI.Ward_API, WardIsLove");
        _IsInstalled = true;

        DisableWardPlayerIsInAPI = WILAPI.GetMethod("DisableWardPlayerIsIn", BindingFlags.Public | BindingFlags.Static);
    }
}


// don't use
public static class Ward_API
{
    public static void DisableWardPlayerIsIn(UnityEngine.Vector3 point, bool DestroyPiece = false)
    {
        WardMonoscript.CheckInWardOutWard(point, out WardMonoscript wardout);
        // wardout.FlashShield(true);
        wardout.SetEnabled(false);
        if (DestroyPiece)
            DestoryWard(wardout.m_piece); // Make Sure this is doesn't hurt WIL
    }

    public static void DestoryWard(Piece piece)
    {
        ZNetView component = piece.GetComponent<ZNetView>();
        if (component == null)
        {
        }
        else
        {
            WearNTear component2 = piece.GetComponent<WearNTear>();
            if ((bool)component2)
            {
                component2.Remove();
            }
            else
            {
                ZLog.Log("Removing non WNT object with hammer " + piece.name);
                component.ClaimOwnership();
                piece.DropResources();
                piece.m_placeEffect.Create(piece.transform.position, piece.transform.rotation, piece.gameObject.transform);
                Player.m_localPlayer.m_removeEffects.Create(piece.transform.position, Quaternion.identity);
                ZNetScene.instance.Destroy(piece.gameObject);
            }
        }
    }

}