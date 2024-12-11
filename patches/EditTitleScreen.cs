using HarmonyLib;
using ObraDinnArchipelago.Assets;
using UnityEngine;

namespace ObraDinnArchipelago.Patches;

[HarmonyPatch(typeof(TitleRoot), "Start", MethodType.Normal)]
internal class EditTitleScreen
{
    [HarmonyPostfix]
    static void UpdateTitleTexture()
    {
        var titleQuad = GameObject.Find("Title Quad");
        titleQuad.GetComponent<MeshRenderer>().material.mainTexture = AssetsManager.titleSprite.texture;
    }
}