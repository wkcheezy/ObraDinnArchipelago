using UnityEngine;

namespace ObraDinnArchipelago.Assets;

internal static class AssetsManager
{
    private static AssetBundle assetBundle;

    internal static Sprite titleSprite;

    internal static void LoadAssets()
    {
        assetBundle = AssetBundle.LoadFromMemory(properties.Resources.archiassets);

        if (!assetBundle)
        {
            Plugin.Logger.LogError("The asset bundle could not be loaded");
            return;
        }

        titleSprite = assetBundle.LoadAsset<Sprite>("Title.png");
    }
}