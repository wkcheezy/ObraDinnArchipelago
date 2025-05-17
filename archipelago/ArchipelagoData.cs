using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using ObraDinnArchipelago.Patches;

namespace ObraDinnArchipelago.Archipelago;

internal class ArchipelagoData
{
    [JsonIgnore] internal static int currentVersion = 1;

    // [JsonIgnore] internal static string saveFilePath = "";
    //
    // [JsonIgnore] internal static string dataFilePath = "";

    [JsonIgnore] internal static ArchipelagoData Data;

    [JsonProperty("version")] internal int version = 0;

    [JsonIgnore] internal static string saveId = "";
    [JsonProperty("hostName")] internal string hostName = "archipelago.gg";
    [JsonProperty("port")] internal int port = 38281;
    [JsonProperty("slotName")] internal string slotName = "";
    [JsonProperty("password")] internal string password = "";

    [JsonProperty("seed")] internal string seed = "";
    [JsonProperty("playerCount")] internal int playerCount = 0;
    [JsonProperty("totalLocationsCount")] internal int totalLocationsCount = 0;
    [JsonProperty("totalItemsCount")] internal int totalItemsCount = 0;

    [JsonProperty("completedChecks")] internal List<long> completedChecks = new List<long>();
    [JsonProperty("receivedItems")] internal List<ObraDinnItemInfo> receivedItems = new List<ObraDinnItemInfo>();
    [JsonIgnore] internal List<ObraDinnItemInfo> itemsUnaccountedFor = new List<ObraDinnItemInfo>();

    [JsonProperty("goalCompletedAndSent")] internal bool goalCompletedAndSent = false;

    [JsonIgnore] internal uint index = 0;

    internal static void SaveToFile()
    {
        string json = JsonConvert.SerializeObject(Data);
        File.WriteAllText(Path.Combine(ArchipelagoSaveData.SavesPath, $"{saveId}.json"), json);
    }
}

