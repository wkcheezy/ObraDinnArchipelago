using System.Linq;
using ObraDinnArchipelago.Archipelago;
using ObraDinnArchipelago.Patches;
using UnityEngine;
using UnityEngine.UI;

namespace ObraDinnArchipelago.Components;

internal class NewConnectionPanel : MonoBehaviour
{
    private Button _connectButton;
    private InputField _addressInput, _slotInput, _passwordInput;
    private bool _confirmNewSave;

    private void Awake()
    {
        // Find the components
        var inputFields = GameObject.Find("Center Holder/Center Panel/Items Panel")
            .GetComponentsInChildren<InputField>();
        _addressInput = inputFields[0];
        _slotInput = inputFields[1];
        _passwordInput = inputFields[2];
        _connectButton = transform.Find("Center Holder/Center Panel/Connect Button").GetComponent<Button>();
        // TODO: When hovering over a text panel, focus will switch to that panel even if currently typing in another
        _connectButton.onClick.AddListener(() =>
        {
            if (transform.parent.GetComponentInChildren<ConnectionsPanel>().dataList
                .Any(d => d.Value.Data.slotName == _slotInput.text && d.Value.Data.password == _passwordInput.text) && _confirmNewSave == false)
            {
                _confirmNewSave = true;
                InitArchipelago.GetArchipelagoComponent().Logs.Add(new LogEntry("Found existing save file. To confirm creating new save file with same details, click connect again"));
                return;
            }
            if (ArchipelagoClient.IsConnecting) return;
            if (_addressInput.text.Contains(":"))
            {
                var addressDetails = _addressInput.text.Split(':');
                ArchipelagoClient.ConnectAsync(addressDetails[0], int.Parse(addressDetails[1].Replace(":", "")),
                    _slotInput.text, _passwordInput.text);
                _confirmNewSave = false;
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
        gameObject.SetActive(false);
        _confirmNewSave = false;
    }
}