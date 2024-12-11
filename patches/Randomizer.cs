using HarmonyLib;
using System.Collections.Generic;

namespace ObraDinnArchipelago.Patches;

[HarmonyPatch(typeof(Manifest))]
internal class Randomizer
{
    // TODO: This works to remove entities from the list, but it should replace them with [REDACTED] or something to help mark it as a check
    [HarmonyPatch(nameof(Manifest.IterateEnts), MethodType.Normal)]
    [HarmonyPrefix]
    public static bool blockEnts(ref List<Manifest.Ent> ___ents)
    {
        ___ents.RemoveAt(1);
        return true;
    }
}