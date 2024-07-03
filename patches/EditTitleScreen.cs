using HarmonyLib;
using ObraDinnArchipelago.Assets;
using UnityEngine;

namespace ObraDinnArchipelago;

[HarmonyPatch(typeof(TitleRoot), "Start", MethodType.Normal)]
public class EditTitleScreen
{
    [HarmonyPostfix]
    static void UpdateTitleTexture()
    {
        var titleQuad = GameObject.Find("Title Quad");
        titleQuad.GetComponent<MeshRenderer>().material.mainTexture = AssetsManager.titleSprite.texture;
    }
}