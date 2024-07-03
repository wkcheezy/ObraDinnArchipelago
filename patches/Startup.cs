using HarmonyLib;
using UnityEngine;

namespace ObraDinnArchipelago;

[HarmonyPatch]
public class Startup
{
    [HarmonyPatch(typeof(Game), "LoadIntro", MethodType.Normal)]
    [HarmonyPrefix]
    static bool Quickstart()
    {
        SaveData.it.DebugGive();
        SaveData.it.DebugVisitAllMoments();
        SaveData.it.SetPlayerExploringSpot(new Player.Spot(new Vector3(-4.64f, 0.9f, 4.14f), Quaternion.Euler(0, 180.7f, 0)));
        Game.LoadStartingShip();
        return false;
    }
}