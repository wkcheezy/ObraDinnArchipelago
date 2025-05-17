using System.IO;
using System.Text.RegularExpressions;
using BepInEx;
using HarmonyLib;
using ObraDinnArchipelago.Archipelago;

namespace ObraDinnArchipelago.Patches;

[HarmonyPatch]
internal class ArchipelagoSaveData
{
    public static readonly string SavesPath = Path.Combine(Paths.GameRootPath, "ArchipelagoSaveFiles");

    /// Check to see if the passed save id is from the vanilla game (containing the p[number] name)
    private static bool CheckSaveFile(string id)
    {
        return Regex.IsMatch(id, "/P[1-3]/gm");
    }
    

    /// Stop the Profiles menu from loading its data
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ProfilesMenu), "Refresh", MethodType.Normal)]
    public static bool StopProfileLoading()
    {
        return false;
    }
    
    /// Format the saves file paths to point to the dedicated Archipelago folder
    [HarmonyPrefix]
    [HarmonyPatch(typeof(SaveData), "GetFilepath", MethodType.Normal)]
    // ReSharper disable once InconsistentNaming
    public static bool InterceptFilePath(string id, ref string __result, string subDir = "")
    {
        var path1 = SavesPath;
        if (subDir.HasValue())
            path1 = Path.Combine(path1, subDir);
        __result = Path.Combine(path1, id + ".txt");
        return false;
    }
    
    /// When the game saves, return the id of the current save id and also save the Archipelago data file
    // TODO: Need to patch backup files as well?
    [HarmonyPrefix]
    [HarmonyPatch(typeof(SaveData), nameof(SaveData.Save), MethodType.Normal)]
    public static bool SaveArchipelago(ref string id)
    {
        // id = ArchipelagoData.saveId;
        // ArchipelagoModPlugin.Log.LogInfo(id);
        ArchipelagoData.SaveToFile();
        return true;
    }

    /// Block all normal save file requests to the CanLoad function
    // [HarmonyPrefix]
    // [HarmonyPatch(typeof(SaveData), nameof(SaveData.CanLoad), MethodType.Normal)]
    // // ReSharper disable once InconsistentNaming
    // public static bool InterceptCanLoad(string id, ref bool __result)
    // {
    //     if (!CheckSaveFile(id)) return true;
    //     __result = false;
    //     return false;
    // }

    /// Only load files that don't have the vanilla game naming scheme
    // [HarmonyPrefix]
    // [HarmonyPatch(typeof(SaveData), nameof(SaveData.Load), MethodType.Normal)]
    // public static bool InterceptLoad(string id)
    // {
    //     return !CheckSaveFile(id);
    // }
}