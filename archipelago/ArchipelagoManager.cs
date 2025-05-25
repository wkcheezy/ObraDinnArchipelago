using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Models;
using JetBrains.Annotations;
using ObraDinnArchipelago.Components;
using ObraDinnArchipelago.Patches;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ObraDinnArchipelago.Archipelago;

internal static class ArchipelagoManager
{
    internal static Action<APData.APItem> onItemReceived;

    internal const int ID_OFFSET = 39090;

    private static Dictionary<long, CheckInfo> checkInfos = new();

    private static Queue<ObraDinnItemInfo> itemQueue = new();

    private static Queue<ObraDinnItemInfo> itemsToVerifyQueue = new();

    [CanBeNull] public static ArchipelagoFile connectionDetails = null;

    internal static void Init()
    {
        ArchipelagoClient.OnConnectAttemptDone += OnConnectAttempt;
        ArchipelagoClient.OnNewItemReceived += OnItemReceived;
        ArchipelagoClient.OnProcessedItemReceived += OnItemToVerifyReceived;
    }

    private static void OnItemReceived(ObraDinnItemInfo item)
    {
        itemQueue.Enqueue(item);
    }

    private static void OnItemToVerifyReceived(ObraDinnItemInfo item)
    {
        itemsToVerifyQueue.Enqueue(item);
    }

    internal static bool ProcessNextItem()
    {
        if (itemQueue.Count <= 0) return false;
        AudioOneShot.Play(InitArchipelago.GetLoggerAudio().effects.Find(e => e.id == "ItemReceived").clips.clips[0]);

        ObraDinnItemInfo item = itemQueue.Dequeue();

        string message;
        if (item.PlayerSlot == ArchipelagoClient.Session.ConnectionInfo.Slot)
            message = "You have found your " + item.ItemName;
        else
            message = "Received " + item.ItemName + " from " + item.PlayerName;

        InitArchipelago.GetArchipelagoComponent().Logs.Add(new LogEntry(message));
        ArchipelagoModPlugin.Log.LogMessage(message);

        ApplyItemReceived(item.Item);

        // Singleton<ArchipelagoUI>.Instance.QueueSave();

        return true;
    }

    internal static void ApplyItemReceived(APData.APItem receivedItem)
    {
        onItemReceived?.Invoke(receivedItem);
    }

    private static void OnConnectAttempt(LoginResult result)
    {
        if (result.Successful)
        {
            if (connectionDetails == null)
            {
                var saveData = new SaveData(); 
                saveData.Reset(); 
                ArchipelagoData.saveId = Guid.NewGuid().ToString(); 
                saveData.Save(ArchipelagoData.saveId); 
                Settings.activeSaveId = ArchipelagoData.saveId; 
                Object.FindObjectOfType<NewConnectionPanel>().gameObject.SetActive(false); 
                Game.LoadStartingShip();
            }
            else
            {
                ArchipelagoData.Data = connectionDetails.Data;
                ArchipelagoData.saveId = Path.GetFileNameWithoutExtension(connectionDetails.FileName);
                InitializeFromServer();
                VerifyAllItems();

                // TODO: Need to ensure when switching between saves that we're loading the correct save
                Settings.activeSaveId = ArchipelagoData.saveId;
                Game.LoadSave(ArchipelagoData.saveId);
            }
            connectionDetails = null;
        }
        // TODO: Switch the successful login sound to bell chime
        AudioClip sound = InitArchipelago.GetLoggerAudio().effects.Find(e => e.id ==
                                                                             (result.Successful
                                                                                 ? "Connected"
                                                                                 : "Disconnected")).clips.clips[0];
        AudioOneShot.Play(sound);
    }
    
    internal static void InitializeFromServer()
    {
        // if (ArchipelagoClient.SlotData.TryGetValue("death_link", out var deathLink))
        //     ArchipelagoOptions.deathlink = Convert.ToInt32(deathLink) != 0;
        // else if (ArchipelagoClient.SlotData.TryGetValue("deathlink", out var deathlink))
        //     ArchipelagoOptions.deathlink = Convert.ToInt32(deathlink) != 0;

        // if (ArchipelagoClient.slotData.TryGetValue("goal", out var goal))
        //     ArchipelagoOptions.goal = (Goal)Convert.ToInt32(goal);

        ArchipelagoData.Data.seed = ArchipelagoClient.Session.RoomState.Seed;
        ArchipelagoData.Data.playerCount = ArchipelagoClient.Session.Players.AllPlayers.Count() - 1;
        ArchipelagoData.Data.totalLocationsCount = ArchipelagoClient.Session.Locations.AllLocations.Count();
        ArchipelagoData.Data.totalItemsCount = ArchipelagoData.Data.totalLocationsCount;
        // ArchipelagoData.Data.goalType = ArchipelagoOptions.goal;

        ScoutChecks();
        VerifyGoalCompletion();
        ArchipelagoClient.SendChecksToServerAsync();
    }

    internal static void VerifyAllItems()
    {
        while (itemsToVerifyQueue.Any())
        {
            ObraDinnItemInfo nextItem = itemsToVerifyQueue.Dequeue();

            if (!VerifyItem(nextItem))
            {
                ArchipelagoModPlugin.Log.LogWarning(
                    $"Item ID {nextItem.ItemId} ({nextItem.ItemName}) didn't apply properly. Retrying...");
                itemQueue.Enqueue(nextItem);
            }
        }
    }

    internal static bool VerifyItem(ObraDinnItemInfo item)
    {
        APData.APItem receivedItem = item.Item;

        return true;
    }

    internal static void ScoutChecks()
    {
        checkInfos.Clear();
        ArchipelagoClient.ScoutLocationsAsync(OnScoutDone);
    }

    private static void OnScoutDone(Dictionary<long, ScoutedItemInfo> packet)
    {
        foreach (ScoutedItemInfo scoutInfo in packet.Values)
        {
            checkInfos.Add((scoutInfo.LocationId - ID_OFFSET), new CheckInfo(
                scoutInfo.LocationId,
                scoutInfo.Player.Slot,
                scoutInfo.Player.Name,
                scoutInfo.ItemId,
                scoutInfo.ItemDisplayName)
            );
        }
    }

    // internal static void SendStoryCheckIfApplicable(StoryEvent storyEvent)
    // {
    //     if (storyCheckPairs.TryGetValue(storyEvent, out APCheck check))
    //     {
    //         SendCheck(check);
    //     }
    // }

    internal static void SendCheck(int check)
    {
        for (var i = 0; i < check; i++)
        {
            ArchipelagoModPlugin.Log.LogMessage($"Sending check: {APData.ChecksList[i]}");
            // if (ArchipelagoData.Data == null)
            // {
            //     ArchipelagoModPlugin.Log.LogError("Error: Data is null");
            //     return;
            // }

            long checkID = ID_OFFSET + (long)i;
            ArchipelagoModPlugin.Log.LogMessage($"Updated check: {checkID}");

            if (!ArchipelagoData.Data.completedChecks.Contains(checkID))
            {
                ArchipelagoData.Data.completedChecks.Add(checkID);
                ArchipelagoClient.SendChecksToServerAsync();
                // Singleton<ArchipelagoUI>.Instance.QueueSave();
            }
            else
            {
                ArchipelagoModPlugin.Log.LogMessage($"Skipping check: {checkID}");
            }
        }
        
        VerifyGoalCompletion();
    }

    internal static bool HasCompletedCheck(int check)
    {
        if (ArchipelagoData.Data == null) return false;

        long checkID = ID_OFFSET + (long)check;

        return ArchipelagoData.Data.completedChecks.Contains(checkID);
    }

    // internal static bool HasItem(APData.APItem item)
    // {
    //     if (ArchipelagoData.Data == null) return false;
    //
    //     return ArchipelagoData.Data.receivedItems.Any(x => x.Item == item);
    // }

    internal static void VerifyGoalCompletion()
    {
        if (ArchipelagoData.Data == null || ArchipelagoData.Data.goalCompletedAndSent) return;

        if (ArchipelagoData.Data.completedChecks.Count >= APData.ChecksList.Count)
        {
            ArchipelagoClient.SendGoalCompleted();
        }
    }

    internal static CheckInfo GetCheckInfo(int check)
    {
        if (checkInfos.TryGetValue(check, out CheckInfo info))
        {
            return info;
        }

        CheckInfo basicInfo = new CheckInfo(check + ID_OFFSET, 0, "Player", 0, check.ToString());

        return basicInfo;
    }
}

internal struct CheckInfo
{
    internal long checkId;
    internal int recipientId;
    internal string recipientName;
    internal long itemId;
    internal string itemName;

    public CheckInfo(long checkId, int recipientId, string recipientName, long itemId, string itemName)
    {
        this.checkId = checkId;
        this.recipientId = recipientId;
        this.recipientName = recipientName;
        this.itemId = itemId;
        this.itemName = itemName;
    }
}