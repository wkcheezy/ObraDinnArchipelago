using System;
using ObraDinnArchipelago.Archipelago;
using ObraDinnArchipelago.Patches;
using UnityEngine;
using UnityEngine.UI;

namespace ObraDinnArchipelago.Components;

internal class NewConnectionPanel : MonoBehaviour
{
    private Button _connectButton;
    private InputField _addressInput, _slotInput, _passwordInput;

    private void Awake()
    {
        // Find the components
        var inputFields = GameObject.Find("Center Holder/Center Panel/Items Panel")
            .GetComponentsInChildren<InputField>();
        _addressInput = inputFields[0];
        _slotInput = inputFields[1];
        _passwordInput = inputFields[2];
        _connectButton = transform.Find("Center Holder/Center Panel/Connect Button").GetComponent<Button>();

        // TODO: Need to add a cancel button that closes the new connection dialog
        // TODO: When hovering over a text panel, focus will switch to that panel even if currently typing in another
        _connectButton.onClick.AddListener(() =>
        {
            if (ArchipelagoClient.IsConnecting) return;
            if (_addressInput.text.Contains(":"))
            {
                var addressDetails = _addressInput.text.Split(':');
                // TODO: Players are loading in to the ship before the connection fully goes through (though its only by a couple seconds so it might be fine)
                ArchipelagoClient.ConnectAsync(addressDetails[0], int.Parse(addressDetails[1].Replace(":", "")),
                    _slotInput.text, _passwordInput.text);
                var saveData = new SaveData();
                saveData.Reset();
                ArchipelagoData.saveId = Guid.NewGuid().ToString();
                saveData.Save(ArchipelagoData.saveId);
                Settings.activeSaveId = ArchipelagoData.saveId;
                gameObject.SetActive(false);
                Game.LoadStartingShip();
            }
            else
            {
                ArchipelagoModPlugin.Log.LogError("Invalid address");
                InitArchipelago.GetArchipelagoComponent().Logs.Add(new LogEntry("Invalid address"));
            }
        });

        // Hide panel
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        // Reset the text fields back to their default if there's not there already
        // TODO: Need to use a graphic to set a placeholder so we don't necessarily have to do this

        _addressInput.text = "localhost:8399";
        _slotInput.text = "Slot";
        _passwordInput.text = "Password";
    }

    private void Update()
    {
        if (!Input.GetKeyUp(KeyCode.Escape) || ArchipelagoClient.IsConnecting) return;
        {
            gameObject.SetActive(false);
        }
    }
}