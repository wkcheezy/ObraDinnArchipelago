using HarmonyLib;
using ObraDinnArchipelago.Assets;
using ObraDinnArchipelago.Components;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ObraDinnArchipelago.Patches;

[HarmonyPatch(typeof(TitleRoot))]
internal class EditTitleScreen
{
    private static GameObject _connectionsObject;

    private enum PageId
    {
        Main = 0,
        Settings = 1,
        Profiles = 2
    }

    [HarmonyPostfix]
    [HarmonyPatch("Start", MethodType.Normal)]
    // ReSharper disable once InconsistentNaming
    private static void UpdateTitleTextureAndStartConnectionsMenu(TitleRoot __instance)
    {
        // Update logo sprite
        // GameObject.Find("Title Quad").GetComponent<MeshRenderer>().material.mainTexture = AssetsManager.titleSprite.texture;
        
        LoadConnections(__instance);
    }

    /// Set connections for Connections page
    private static void LoadConnections(TitleRoot instance)
    {
        var c = Object.Instantiate(AssetsManager.serverInput, instance.transform.GetChild(0));
        c.AddComponent<ConnectionsPanel>();
        
        _connectionsObject = c;
    }

    [HarmonyPrefix]
    [HarmonyPatch("SetPage", MethodType.Normal)]
    private static bool InterceptProfilesPage(PageId pageId)
    {
        if (pageId != PageId.Profiles) return true;
        _connectionsObject.SetActive(true);
        return false;
    }
}