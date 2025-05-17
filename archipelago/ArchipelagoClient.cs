using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using ObraDinnArchipelago.Components;
using ObraDinnArchipelago.Patches;

namespace ObraDinnArchipelago.Archipelago;

internal static class ArchipelagoClient
{
    internal static Action<LoginResult> OnConnectAttemptDone;
    internal static Action<ObraDinnItemInfo> OnNewItemReceived;
    internal static Action<ObraDinnItemInfo> OnProcessedItemReceived;

    internal static bool IsConnecting => _isConnecting;
    internal static bool IsConnected => _isConnected;

    private const string ArchipelagoVersion = "0.5.1";

    private delegate void OnConnectAttempt(LoginResult result);

    private static bool _isConnecting = false;
    private static bool _isConnected = false;

    internal static ArchipelagoSession Session;

    internal static Dictionary<string, object> SlotData = new();
    
    internal static void ConnectAsync(string hostName, int port, string slotName, string password)
    {
        if (_isConnecting || _isConnected) return;

        ArchipelagoData.Data.hostName = hostName;
        ArchipelagoData.Data.port = port;
        ArchipelagoData.Data.slotName = slotName;
        ArchipelagoData.Data.password = password;
        
        _isConnecting = true;
        InitArchipelago.GetArchipelagoComponent().Logs.Add(new LogEntry($"Connecting as {slotName}..."));
        ArchipelagoModPlugin.Log.LogInfo($"Connecting as {slotName}...");

        ThreadPool.QueueUserWorkItem(_ => Connect(OnConnected, port, hostName));
    }

    internal static void Disconnect()
    {
        ArchipelagoModPlugin.Log.LogInfo("Disconnecting from server...");
        Session?.Socket.Disconnect();
        Session = null;
        _isConnected = false;
        _isConnecting = false;
    }

    internal static void ScoutLocationsAsync(Action<Dictionary<long, ScoutedItemInfo>> callback)
    {
        if (!_isConnected) return;

        Session.Locations.ScoutLocationsAsync(callback, Session.Locations.AllLocations.ToArray());
    }

    internal static void checksSentResult(bool success)
    {
        if (success)
        {
            ArchipelagoModPlugin.Log.LogInfo($"Successfully sent result: {success}");
        }
        else
        {
            ArchipelagoModPlugin.Log.LogError($"Failed to send result: {success}");
        }
    }
    
    internal static void SendChecksToServerAsync()
    {
        if (!_isConnected) return;

        Session.Locations.CompleteLocationChecksAsync(checksSentResult, ArchipelagoData.Data.completedChecks.ToArray());
    }

    internal static void SendGoalCompleted()
    {
        if (!_isConnected) return;

        Session.SetGoalAchieved();
        ArchipelagoData.Data.goalCompletedAndSent = true;
    }

    private static ArchipelagoSession CreateSession(int port, string host)
    {
        var session = ArchipelagoSessionFactory.CreateSession($"ws://{host}:{port}/");
        session.MessageLog.OnMessageReceived += OnMessageReceived;
        session.Socket.ErrorReceived += SessionErrorReceived;
        session.Socket.SocketClosed += SessionSocketClosed;
        session.Items.ItemReceived += OnItemReceived;
        return session;
    }

    private static void Connect(OnConnectAttempt attempt, int port, string host)
    {
        LoginResult result;
        // TODO: Check that the address and port provided match the end server (not the client) to prevent someone from joining with a older/unconnected save file
        // TODO: Remove address input and standardize the address, since thats handled by the PyClient
        try
        {
            Session = CreateSession(8399, "localhost");
            result = Session.TryConnectAndLogin(
                "Return of the Obra Dinn",
                ArchipelagoData.Data.slotName,
                ItemsHandlingFlags.AllItems,
                new System.Version(ArchipelagoVersion),
                password: ArchipelagoData.Data.password == "" ? null : ArchipelagoData.Data.password
            );
        }
        catch (Exception e)
        {
            ArchipelagoModPlugin.Log.LogError(e.Message + "\n" + e.StackTrace);
            result = new LoginFailure(e.Message);
        }

        attempt(result);
    }

    private static void OnConnected(LoginResult result)
    {
        if (result.Successful)
        {
            ArchipelagoModPlugin.Log.LogInfo("Login successful!");
            InitArchipelago.GetArchipelagoComponent().Logs.Add(new LogEntry("Connected successfully!"));
            SlotData = ((LoginSuccessful)result).SlotData;
            _isConnected = true;
        }
        else
        {
            LoginFailure failure = (LoginFailure)result;
            foreach (var error in failure.Errors)
            {
                ArchipelagoModPlugin.Log.LogError(error);
            }
            string errorMessage = $"Failed to connect to {ArchipelagoData.Data.hostName}: ";
            errorMessage = failure.Errors.Aggregate(errorMessage, (current, error) => current + $"\n    {error}");

            ArchipelagoModPlugin.Log.LogError(errorMessage);
            foreach (var error in failure.Errors)
            {
                InitArchipelago.GetArchipelagoComponent().Logs.Add(new LogEntry(error));
            }

            Disconnect();
        }

        OnConnectAttemptDone?.Invoke(result);

        _isConnecting = false;
    }

    private static void SessionSocketClosed(string reason)
    {
        ArchipelagoModPlugin.Log.LogInfo($"Connection lost: {reason}");
        if (Session != null)
            Disconnect();
    }

    private static void SessionErrorReceived(Exception e, string message)
    {
        if (InitArchipelago.GetArchipelagoComponent() == null || ArchipelagoModPlugin.Log == null) return;

        InitArchipelago.GetArchipelagoComponent().Logs.Add(new LogEntry(message));
        ArchipelagoModPlugin.Log.LogError($"Archipelago error: {message}");
        if (Session != null)
            Disconnect();
    }

    private static void OnMessageReceived(LogMessage message)
    {
        ArchipelagoModPlugin.Log.LogMessage(message.ToString());
        InitArchipelago.GetArchipelagoComponent().Logs.Add(new LogEntry(message.ToString()));
    }
    
    private static void OnItemReceived(ReceivedItemsHelper helper)
    {
        if (ArchipelagoData.Data.index >= helper.Index) return;

        ArchipelagoData.Data.index++;

        ItemInfo nextItem = helper.DequeueItem();
        ObraDinnItemInfo matchedItem =
            ArchipelagoData.Data.itemsUnaccountedFor.FirstOrDefault(x => IsSameItem(x, nextItem));

        if (matchedItem == null)
        {
            ArchipelagoModPlugin.Log.LogInfo("Receiving item");
            // This item is new
            ObraDinnItemInfo newItemInfo = new ObraDinnItemInfo(
                (APData.APItem)Convert.ToInt32(nextItem.ItemId - ArchipelagoManager.ID_OFFSET),
                nextItem.ItemName, nextItem.ItemId,
                nextItem.LocationId, nextItem.Player.Slot, nextItem.Player.Name);

            ArchipelagoData.Data.receivedItems.Add(newItemInfo);

            OnNewItemReceived?.Invoke(newItemInfo);
            ArchipelagoModPlugin.Log.LogInfo("Item recieved");
        }
        else
        {
            ArchipelagoData.Data.itemsUnaccountedFor.Remove(matchedItem);

            OnProcessedItemReceived?.Invoke(matchedItem);
        }
    }

    private static bool IsSameItem(ObraDinnItemInfo left, ItemInfo right)
    {
        return left.ItemId == right.ItemId
               && left.LocationId == right.LocationId
               && left.PlayerSlot == right.Player.Slot;
    }

    internal static string GetPlayerName(int player)
    {
        if (!_isConnected) return "";

        return Session.Players.GetPlayerName(player);
    }

    internal static string GetItemName(long item)
    {
        if (!_isConnected) return "";

        return Session.Items.GetItemName(item);
    }
}