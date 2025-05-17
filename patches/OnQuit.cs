using HarmonyLib;
using ObraDinnArchipelago.Archipelago;

namespace ObraDinnArchipelago.Patches;

[HarmonyPatch(typeof(Game))]
internal class OnQuit
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Game.LoadTitle), MethodType.Normal)]
    private static bool DisconnectOnQuit()
    {
        // TODO: Should we prevent the main menu from loading if the client can't disconnect/wasn't disconnected?
        if (ArchipelagoClient.IsConnected) ArchipelagoClient.Disconnect();
        ArchipelagoData.saveId = "";
        Settings.activeSaveId = "";
        return true;
    }
}