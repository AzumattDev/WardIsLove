using System;
using JetBrains.Annotations;
using UnityEngine;
using WardIsLove.Extensions;
using WardIsLove.Util;

namespace WardIsLove.API
{
    [PublicAPI]
    public class API
    {
        // Events for ward interactions that can be subscribed to externally
        public static event Action<Vector3>? OnWardEntered; // Triggered when a player or entity enters a ward
        public static event Action<Vector3>? OnWardExited; // Triggered when a player or entity exits a ward
        public static event Action<Vector3>? OnBubbleOn; // Triggered when a ward's bubble shield is activated
        public static event Action<Vector3>? OnBubbleOff; // Triggered when a ward's bubble shield is deactivated
        public static event Action<Vector3, float>? OnDamageTaken; // Triggered when a ward takes damage

        // Delegate to check ward presence, allowing other parts of the game or mods to query ward status
        public delegate bool WardCheckHandler(Vector3 location);

        public static event WardCheckHandler? CheckWardPresence;

        /// <summary>
        /// Indicates whether the API is currently loaded and available.
        /// This method can be used by other mods to check for API presence.
        /// </summary>
        public static bool IsLoaded()
        {
#if API
            return false; // When compiled as an API, assume it's not loaded as part of the game
#else
            return true; // When compiled into the game, the API is considered loaded
#endif
        }

        /// <summary>
        /// Checks if a given point is inside any ward area.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>True if the point is inside a ward, false otherwise.</returns>
        public static bool IsInsideWard(Vector3 point)
        {
            return CheckWardPresence?.Invoke(point) ?? false;
        }

        public void DisableWardPlayerIsIn(Vector3 point, bool destroyPiece = false)
        {
#if !API
            WardMonoscript.CheckInWardOutWard(point, out WardMonoscript wardout);
            if (wardout != null)
            {
                wardout.SetEnabled(false);
                if (destroyPiece)
                {
                    DestroyWard(wardout.m_piece);
                }
            }
#endif
        }

        public GameObject GetWardObject(Vector3 point)
        {
#if !API
            WardMonoscript.CheckInWardOutWard(point, out WardMonoscript ward);
            return ward?.gameObject;
#endif
            return null!;
        }

        public GameObject GetWardBubble(Vector3 point)
        {
#if !API
            WardMonoscript.CheckInWardOutWard(point, out WardMonoscript ward);
            return ward?.m_bubble.gameObject;
#endif
            return null!;
        }

        public void DestroyWard(Piece piece)
        {
            ZNetView component = piece.GetComponent<ZNetView>();
            if (component != null)
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
#if !API
        internal static void WardEntered(Vector3 position)
        {
            OnWardEntered?.Invoke(position);
        }

        internal static void WardExited(Vector3 position)
        {
            OnWardExited?.Invoke(position);
        }

        internal static void BubbleOn(Vector3 position)
        {
            OnBubbleOn?.Invoke(position);
        }

        internal static void BubbleOff(Vector3 position)
        {
            OnBubbleOff?.Invoke(position);
        }

        internal static void DamageTaken(Vector3 position, float damage)
        {
            OnDamageTaken?.Invoke(position, damage);
        }
#endif
    }
}