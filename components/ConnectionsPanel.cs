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

internal class ArchipelagoFile(string fileName, ArchipelagoData data)
{
    public readonly string FileName = fileName;
    public readonly ArchipelagoData Data = data;
}

internal class ConnectionsPanel : MonoBehaviour
{
    private GameObject _existingConnectionsPanel;
    private SortedList<DateTime, ArchipelagoFile> _dataList = new();
    private int pageNum = 1;
    private const int maxDisplay = 5;
    private Text pageNumText;

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
        var existingConnectionPanel =
            transform.Find("ConnectionsPanel/Center Holder/Center Panel/Items Panel").gameObject;
        existingConnectionPanel.transform.GetChild(0).GetComponent<Button>().onClick
            .AddListener(() => { newConnectionsPanel.SetActive(true); });
        var existingConnectionPrefab = existingConnectionPanel.transform.GetChild(1);
        foreach (var connection in _dataList.Reverse().Take(maxDisplay))
        {
            var newExistingConnection = Instantiate(existingConnectionPrefab, existingConnectionPanel.transform, false);
            var listItem = newExistingConnection.GetChild(0);
            var text = listItem.GetChild(0).GetChild(0).GetComponent<Text2>();
            text.text = $"{connection.Value.Data.slotName} ({connection.Key})";

            listItem.GetComponent<Button>().onClick.RemoveAllListeners();
            listItem.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (ArchipelagoClient.IsConnecting) return;

                ArchipelagoClient.ConnectAsync(connection.Value.Data.hostName, connection.Value.Data.port,
                    connection.Value.Data.slotName, connection.Value.Data.password);
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
        pageNumText = pageNumber.GetComponent<Text>();
        UpdatePageText();
        spacerPanel.SetAsLastSibling();

        var pageButtonPanel = transform.Find("ConnectionsPanel/Center Holder/Center Panel/Page Buttons Panel");
        pageButtonPanel.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
        {
            if (pageNum >= Math.Max((int)Math.Ceiling((decimal)_dataList.Count / maxDisplay), 1)) return;
            pageNum++;
            // TODO: Would be better code if we combine these calls/methods somehow since we seem to be doing both one after another
            UpdateFileListings();
            UpdatePageText();
        });
        pageButtonPanel.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
        {
            if (pageNum <= 1) return;
            pageNum--;
            UpdateFileListings();
            UpdatePageText();
        });

        // Hide connection list entry prefab
        existingConnectionPrefab.gameObject.SetActive(false);

        _existingConnectionsPanel = existingConnectionPanel;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        pageNum = 1;
        UpdateFileListings();
        UpdatePageText();
        _existingConnectionsPanel.SetActive(true);
    }

    private void UpdatePageText()
    {
        pageNumText.text = $"{pageNum} / {Math.Max((int)Math.Ceiling((decimal)_dataList.Count / maxDisplay), 1)}";
    }

    private void UpdateFileListings()
    {
        foreach (var value in _existingConnectionsPanel.AllChildren().Where((o) => o.name == "Existing Connection(Clone)").Select((child, i) => new {child, i}))
        {
            var dataIndex = value.i + maxDisplay * (pageNum - 1);
            if (dataIndex > dataList.Count - 1) value.child.SetActive(false);
            else
            {
                if (value.child.activeSelf == false) value.child.SetActive(true);
                var text = value.child.GetComponentInChildren<Text2>();
                var connection = _dataList.Reverse().ElementAt(dataIndex);
                text.text = $"{connection.Value.Data.slotName} ({connection.Key})";
                var button = value.child.GetComponentInChildren<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    if (ArchipelagoClient.IsConnecting) return;

                    ArchipelagoClient.ConnectAsync(connection.Value.Data.hostName, connection.Value.Data.port,
                        connection.Value.Data.slotName, connection.Value.Data.password);
                    ArchipelagoData.Data = connection.Value.Data;
                    ArchipelagoData.saveId = Path.GetFileNameWithoutExtension(connection.Value.FileName);
                    StartCoroutine(PostConnect(ArchipelagoData.saveId));
                });

                value.child.GetComponent<ListButton>().texts = [text];
            }
        }
    }

    private void Update()
    {
        // TODO: Add 'A' and 'D' controls
        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            if (Input.GetKeyUp(KeyCode.LeftArrow) && pageNum > 1)
            {
                pageNum--;
                UpdateFileListings();
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow) &&
                     pageNum < Math.Max((int)Math.Ceiling((decimal)dataList.Count / maxDisplay), 1))
            {
                pageNum++;
                UpdateFileListings();
            }

            UpdatePageText();
        }

        if (Input.GetKeyUp(KeyCode.Escape) && !ArchipelagoClient.IsConnecting && transform.GetChild(1).gameObject.activeSelf == false)
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