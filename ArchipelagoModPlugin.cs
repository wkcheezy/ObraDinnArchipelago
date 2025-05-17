using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ObraDinnArchipelago.Archipelago;
using ObraDinnArchipelago.Assets;
using ObraDinnArchipelago.Patches;
using UnityEngine;

namespace ObraDinnArchipelago;

// TODO: If we implement memory blocking (conditional or otherwise), make the watch do its crazy spin animation
// TODO: If data about the fate is missing (i.e. name and/or verb is locked), make the face fuzzy? (Perhaps make this an optional feature, since it could be spoilers)
// TODO: Add a deathlink/high difficulty setting that, if the guess/three guesses are not immedtaely correct, send out a death ("Perfectionist") (Remove all previously confirmed names?)

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class ArchipelagoModPlugin : BaseUnityPlugin
{
    internal static ManualLogSource Log;
    // public static ConfigEntry<bool> configAmnesia;

    private void Awake()
    {
        // configAmnesia = Config.Bind("General", "Amnesia", false, "Prevent the loading of memories");
        Log = Logger;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        AssetsManager.LoadAssets();
        ArchipelagoManager.Init();
    }

    // TODO: 'Amnesia' mode: Block all memories from being loaded (remove pocket watch)
    // For debugging and development purposes only when using Runtime Unity Editor. For release, comment this code out.
    #if DEBUG
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
            FreeCursor.debug = !FreeCursor.debug;
    }
    #endif
}