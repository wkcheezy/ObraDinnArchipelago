using UnityEngine;

namespace ObraDinnArchipelago.Assets;

internal static class AssetsManager
{
    private static AssetBundle assetBundle;

    //internal static Sprite titleSprite;
    internal static GameObject serverInput, overlay;

    internal static void LoadAssets()
    {
        assetBundle = AssetBundle.LoadFromMemory(properties.Resources.archiassets);

        if (!assetBundle)
        {
            ArchipelagoModPlugin.Log.LogError("The asset bundle could not be loaded");
            return;
        }

        //titleSprite = assetBundle.LoadAsset<Sprite>("Title.png");
        serverInput = assetBundle.LoadAsset<GameObject>("Connections.prefab");
        overlay = assetBundle.LoadAsset<GameObject>("Overlay.prefab");
    }
}