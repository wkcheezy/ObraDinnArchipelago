using HarmonyLib;
using UnityEngine;

namespace ObraDinnArchipelago.patches;

[HarmonyPatch]
public class FreeCursor
{
    public static bool debug = false;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(RInput), "UpdateMousePosition")]
    public static bool RInput_UpdateMousePosition_Prefix(bool appHasFocus,
        Vector2 ___mousePosition_)
    {
        if (debug)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return false;
        }
        else return true;

    }
}