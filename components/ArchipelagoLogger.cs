using System.Collections.Generic;
using System.Linq;
using ObraDinnArchipelago.Archipelago;
using UnityEngine;

namespace ObraDinnArchipelago.Components;

internal class LogEntry(string text, int duration = 1000)
{
    public readonly string Text = text;
    public int Duration = duration;
}

internal class ArchipelagoLogger : MonoBehaviour
{
    public List<LogEntry> Logs = [];

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 15, 100, 100), $"Archipelago Status: {(ArchipelagoClient.IsConnected ? "<color='green'>Connected</color>" : "<color='red'>Not connected</color>")}", new GUIStyle{richText = true});
        for (var i = 0; i < Logs.Count; i++)
        {
            GUI.Label(new Rect(10, 15 * (i + 2), Logs[i].Text.Length * 20, 100), Logs[i].Text);
            Logs[i].Duration--;
        }

        if (Logs.Any(l => l.Duration <= 0))
        {
            Logs = Logs.Where(l => l.Duration > 0).ToList();
        }
    }
    
}