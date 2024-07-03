using HarmonyLib;
using UnityEngine;

namespace ObraDinnArchipelago.patches;

[HarmonyPatch]
public class FreeCursor
{
    public static bool debug = false;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(RInput), nameof(RInput.UpdateMousePosition))]
    public static bool RInput_UpdateMousePosition_Prefix()
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