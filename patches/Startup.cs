using HarmonyLib;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ObraDinnArchipelago.Patches;

[HarmonyPatch]
internal class Startup
{
    [HarmonyPatch(typeof(Game), nameof(Game.LoadIntro), MethodType.Normal)]
    [HarmonyPrefix]
    static bool Quickstart()
    {
        SaveData.it.DebugGive();
        SaveData.it.DebugVisitAllMoments();
        SaveData.it.SetPlayerExploringSpot(new Player.Spot(new Vector3(-4.64f, 0.9f, 4.14f), Quaternion.Euler(0, 180.7f, 0)));
        SaveData.it.general.helpedBookBookmarks = true;
        SaveData.it.general.helpedBookDifficulty = true;
        SaveData.it.general.helpedBookFaceBlur = true;
        SaveData.it.general.helpedBookFaceClear = true;
        SaveData.it.general.helpedBookFatesCheck = true;
        SaveData.it.general.helpedBookUsage = true;
        Game.LoadStartingShip();
        
        return false;
    }
}