using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HarmonyLib;
using ObraDinnArchipelago.Archipelago;
using UnityEngine;
using UnityEngine.Bindings;

namespace ObraDinnArchipelago.Patches;

[HarmonyPatch]
internal class SendChecksOnCorrectGuesses
{
    private static GameObject climb;
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Book), nameof(Book.RevealCorrectGuesses), MethodType.Normal)]
    private static bool SendChecks(List<string> crewIds)
    {
        // TODO: Need to account for unconnected sending (i.e. if we're not connected to the server, we need to send out the checks once we connect)
        // TODO: Need to log to in-game logs when sending items
        // TODO: This is sending a check for three items individually, maybe just reduce it to one call with the three items added to found array
        if (ArchipelagoData.Data.goalCompletedAndSent) return true;
        
        ArchipelagoManager.SendCheck(ArchipelagoData.Data.completedChecks.Count + Math.Min(3, crewIds.Count));

        return true;
    }

    // TODO: Coalesce this file better
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Game), "Update", MethodType.Normal)]
    // ReSharper disable once InconsistentNaming
    private static void PreventLeaveShip(string ___activeSceneName)
    {
        if (___activeSceneName != "Ship") return;
        if (!climb) climb = GameObject.Find("Climb S");
        if (climb.activeSelf == ArchipelagoData.Data.goalCompletedAndSent) return;
        climb.SetActive(ArchipelagoData.Data.goalCompletedAndSent);
    }
    
}