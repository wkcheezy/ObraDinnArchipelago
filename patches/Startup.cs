using HarmonyLib;
using UnityEngine;

namespace ObraDinnArchipelago.Patches;

[HarmonyPatch]
internal class Startup
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Game), nameof(Game.LoadStartingShip), MethodType.Normal)]
    static bool Quickstart()
    {
        // TODO: 'Rashomom' mode: Memories change each time you enter them
        SaveData.it.DebugGive();
        SaveData.it.DebugVisitAllMoments();
        SaveData.it.SetPlayerExploringSpot(new Player.Spot(new Vector3(-4.64f, 0.9f, 4.14f), Quaternion.Euler(0, 180.7f, 0)));
        SaveData.it.general.helpedBookBookmarks = true;
        SaveData.it.general.helpedBookDifficulty = true;
        SaveData.it.general.helpedBookFaceBlur = true;
        SaveData.it.general.helpedBookFaceClear = true;
        SaveData.it.general.helpedBookFatesCheck = true;
        SaveData.it.general.helpedBookUsage = true;

        return true;
    }

    // TODO: Eventually just withhold a random name on the server
    [HarmonyPostfix]
    [HarmonyPatch(typeof(SaveData), "DebugSetFateCorrect", MethodType.Normal)]
    static void SetCaptainCorrect(SaveData __instance)
    {
        SaveData.FaceData faceData = __instance.face["captain"];
        if (faceData.markedCorrect) return;
        faceData.nameId = "captain";
        faceData.fateId = Manifest.it.GetCrewFateIds("captain")[0];
        faceData.markedCorrect = true;
    }
}