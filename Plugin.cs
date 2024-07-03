using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ObraDinnArchipelago.Assets;
using ObraDinnArchipelago.patches;
using UnityEngine;

namespace ObraDinnArchipelago;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        AssetsManager.LoadAssets();
    }

    // For debugging and development purposes only when using Runtime Unity Editor. For release, comment this code out.
    #if DEBUG
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
            FreeCursor.debug = !FreeCursor.debug;
    }
    #endif
}