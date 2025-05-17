using System;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using ObraDinnArchipelago.Archipelago;
using ObraDinnArchipelago.Components;
using UnityEngine;
using UnityEngine.UI;

namespace ObraDinnArchipelago.Patches;

[HarmonyPatch]
internal class FilterNames
{
    private static bool _restrictNames;
    private static List<APData.APItem> _items = [];

    [HarmonyPrefix]
    [HarmonyPatch(typeof(FateEditor), "EditPart", MethodType.Normal)]
    public static bool DetermineRestriction(int partIndex)
    {
        _restrictNames = partIndex == 0;
        return true;
    }
    // TODO: We're excluding the names from the tutorial and Bargain, should this be the case?
    // TODO: Save filtered lists so we don't recalculate them every time?
    // TODO: Server's logic isn't supporting restricting the killer name list. For now, killer name list isn't restricted, but eventually restrict it (Should we add fates to the item pool?)
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Manifest), nameof(Manifest.IterateEnts), MethodType.Normal)]
    // ReSharper disable once InconsistentNaming
    public static void RestrictUncollectedNames(bool forSubject, ref  IEnumerable<Manifest.Ent> __result)
    {
        if (!forSubject || !_restrictNames) return;
        var receivedItems = ArchipelagoData.Data.receivedItems.Select(item => item.Item).ToList();

        __result = __result.Where(ent =>
            !Enum.IsDefined(typeof(APData.APItem), ent.id) || receivedItems.Contains(APData.CrewIdToItem[ent.id]));
    }
    
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Scrollable), "Awake", MethodType.Normal)]
    // ReSharper disable once InconsistentNaming
    public static void InitialPreventManifestSelection(ref Scrollable __instance)
    {
        if (__instance.transform.parent.name != "ScrollableManifest") return;
        var receivedItems = ArchipelagoData.Data.receivedItems.Select(item => item.Item).ToList();
        _items = receivedItems;
        for (var i = 0; i < __instance.transform.GetChild(0).childCount; i++)
        {
            var child = __instance.transform.GetChild(0).transform.GetChild(i).gameObject;
            child.GetComponent<Button>().enabled = APData.CrewIdToItem.ContainsKey(child.name) && _items.Contains(APData.CrewIdToItem[child.name]);
        }
    }
    
    // !BUG: This doesn't appear to be updating when we receive an item through !giveitem, need to check if this isolated to just the command or when we receive an item in general. Looks like we need to refresh the manifest somehow
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Scrollable), "Update", MethodType.Normal)]
    // ReSharper disable once InconsistentNaming
    public static bool PreventManifestSelection(ref Scrollable __instance)
    {
        if (__instance.transform.parent.name != "ScrollableManifest") return true;
        var receivedItems = ArchipelagoData.Data.receivedItems.Select(item => item.Item).ToList();
        if (_items.Count == receivedItems.Count) return true;
        _items = receivedItems;
        for (var i = 0; i < __instance.transform.GetChild(0).childCount; i++) 
        {
            var child = __instance.transform.GetChild(0).transform.GetChild(i).gameObject;
            child.GetComponent<Button>().enabled = APData.CrewIdToItem.ContainsKey(child.name) && _items.Contains(APData.CrewIdToItem[child.name]);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(__instance.gameObject.GetComponent<RectTransform>());

        return true;
    }
}