using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ObraDinnArchipelago.Archipelago;
using ObraDinnArchipelago.Patches;
using UnityEngine;
using UnityEngine.UI;

namespace ObraDinnArchipelago.Components;

// TODO: Remove address input (or just disable/prefill it)
internal class ArchipelagoFile(string fileName, ArchipelagoData data)
{
    public readonly string FileName = fileName;
    public readonly ArchipelagoData Data = data;
}

internal class ConnectionsPanel : MonoBehaviour
{
    private GameObject _existingConnectionsPanel;
    private SortedList<DateTime, ArchipelagoFile> _dataList = new();
    
    private void InitializeSaves()
    {
        if (!Directory.Exists(ArchipelagoSaveData.SavesPath))
        {
            try
            {
                Directory.CreateDirectory(ArchipelagoSaveData.SavesPath);
            }
            catch (Exception e)
            {
                ArchipelagoModPlugin.Log.LogError("Failed to create save directory: " + e.Message);
                return;
            }
        }
        // TODO: Check to make sure that there's a matching save file for each data file
        string[] archDataFiles = Directory.GetFiles(ArchipelagoSaveData.SavesPath, "*.json");
        foreach (var dataFile in archDataFiles)
        {
            string dataContent;

            try
            {
                dataContent = File.ReadAllText(dataFile);
            }
            catch (Exception e)
            {
                ArchipelagoModPlugin.Log.LogError("Failed to load save data from " + dataFile + ": " + e.Message);
                continue;
            }
            
            ArchipelagoData loadedData;
    
            try
            {
                loadedData = JsonConvert.DeserializeObject<ArchipelagoData>(dataContent);
            }
            catch
            {
                loadedData = null;
            }
            
            if (loadedData != null)
            {
                if (!SaveData.CanLoad(Path.GetFileNameWithoutExtension(dataFile)))
                {
                    ArchipelagoModPlugin.Log.LogError("Missing save file for \"" + dataFile + "\".");
                }
                
                loadedData.itemsUnaccountedFor = [..loadedData.receivedItems];
    
                try
                {
                    _dataList.Add(File.GetCreationTime(dataFile), new ArchipelagoFile(dataFile, loadedData));
                }
                catch (Exception e)
                {
                    ArchipelagoModPlugin.Log.LogError("Failed to add save entry " + dataFile + ": " + e.Message);
                }
            }
            else
            {
                ArchipelagoModPlugin.Log.LogError("Failed to load data from save file \"" + dataFile +
                                                  "\". The file might be corrupted or is unsupported by this version of the mod.");
            }
        }
    
        // TODO: Likely need to get this working to refresh the saves list
        // LayoutRebuilder.ForceRebuildLayoutImmediate(this.gameObject);
    }
    
    // public void OnSaveFileDeleted(string saveName)
    // {
    //     // TODO: Add logging
    // }
    //
    // private void DeleteSaveFile(string saveName)
    // {
    //     Directory.Delete(Path.Combine(ArchipelagoSaveData.savesPath, saveName), true);
    //     DateTime keyToDelete = _dataList.FirstOrDefault(pair => pair.Value.Item1 == saveName).Key;
    //     _dataList.Remove(keyToDelete);
    //     if (saveUIEntries.TryGetValue(saveName, out GameObject entry))
    //     {
    //         Destroy(entry);
    //         saveUIEntries.Remove(saveName);
    //     }
    //     
    //     // TODO: Likely need to get this working to refresh the saves list
    //     // LayoutRebuilder.ForceRebuildLayoutImmediate(saveSelectViewport);
    // }
    // TODO: Need to disable/cross-out completed connections


    private void Awake()
    {
        InitializeSaves();
        ArchipelagoData.Data = new ArchipelagoData();
        var newConnectionsPanel = transform.GetChild(1).gameObject;
        newConnectionsPanel.AddComponent<NewConnectionPanel>();
        var existingConnectionPanel= transform.Find("ConnectionsPanel/Center Holder/Center Panel/Items Panel").gameObject;
        existingConnectionPanel.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => {newConnectionsPanel.SetActive(true);});
        var existingConnectionPrefab = existingConnectionPanel.transform.GetChild(1);
        foreach (var connection in _dataList.Reverse())
        {
            var newExistingConnection = Instantiate(existingConnectionPrefab, existingConnectionPanel.transform, false);
            var listItem = newExistingConnection.GetChild(0);
            var text = listItem.GetChild(0).GetChild(0).GetComponent<Text2>();
            text.text = $"{connection.Value.Data.slotName} ({connection.Key})";
            
            listItem.GetComponent<Button>().onClick.RemoveAllListeners();
            listItem.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (ArchipelagoClient.IsConnecting) return;
                
                ArchipelagoClient.ConnectAsync(connection.Value.Data.hostName, connection.Value.Data.port, connection.Value.Data.slotName, connection.Value.Data.password);
                ArchipelagoData.Data = connection.Value.Data;
                ArchipelagoData.saveId = Path.GetFileNameWithoutExtension(connection.Value.FileName);
                StartCoroutine(PostConnect(ArchipelagoData.saveId));
            });

            listItem.GetComponent<ListButton>().texts = [text];
            newExistingConnection.SetAsLastSibling();
        }
        
        // Update page counter
        var spacerPanel = existingConnectionPanel.transform.GetChild(2);
        var pageNumber = spacerPanel.transform.GetChild(0);
        pageNumber.GetComponent<Text>().text = $"1 / {Math.Max((int)Math.Ceiling((decimal)_dataList.Count / 5), 1)}";
        spacerPanel.SetAsLastSibling();

        // Hide connection list entry prefab
        existingConnectionPrefab.gameObject.SetActive(false);

        _existingConnectionsPanel = existingConnectionPanel;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _existingConnectionsPanel.SetActive(true);
    }

    private void Update()
    {
        if (!Input.GetKeyUp(KeyCode.Escape)) return;
        if (transform.GetChild(1).gameObject.activeSelf == false)
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator PostConnect(string saveId)
    {
        yield return new WaitUntil(() => !ArchipelagoClient.IsConnecting);

        if (!ArchipelagoClient.IsConnected) yield break;
        // if (ArchipelagoData.Data.seed != "" &&
        //     ArchipelagoClient.session.RoomState.Seed != ArchipelagoData.Data.seed)
        // {
        //
        // }
            
        yield return new WaitForSeconds(0.75f);
    
        ArchipelagoManager.InitializeFromServer();
    
        yield return null;
        
        ArchipelagoManager.VerifyAllItems();
    
        yield return null;
        
        // TODO: Need to ensure when switching between saves that we're loading the correct save
        Settings.activeSaveId = ArchipelagoData.saveId;
        Game.LoadSave(saveId);
    }
}