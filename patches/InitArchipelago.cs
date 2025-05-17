using HarmonyLib;
using ObraDinnArchipelago.Assets;
using ObraDinnArchipelago.Components;
using UnityEngine;

namespace ObraDinnArchipelago.Patches;

[HarmonyPatch(typeof(SceneRoot))]
internal class InitArchipelago
{
    private static GameObject _logger;

    [HarmonyPostfix]
    [HarmonyPatch("Awake", MethodType.Normal)]
    private static void LoadArchipelago()
    {
        if (_logger != null) return;

        GameObject gameObject = Object.Instantiate(AssetsManager.overlay);
        gameObject.AddComponent<ArchipelagoLogger>();
        _logger = gameObject;
    }
    
    public static ArchipelagoLogger GetArchipelagoComponent() => _logger.GetComponent<ArchipelagoLogger>();
    
    public static AudioKit GetLoggerAudio() => _logger.GetComponent<AudioKit>();
}