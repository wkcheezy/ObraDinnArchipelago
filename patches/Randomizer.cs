using System.Collections.Generic;
using HarmonyLib;

namespace ObraDinnArchipelago.patches;

[HarmonyPatch(typeof(Manifest))]
public class Randomizer
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