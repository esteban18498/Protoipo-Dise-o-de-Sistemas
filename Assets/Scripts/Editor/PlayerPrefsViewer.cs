#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public class PlayerPrefsViewer : EditorWindow
{
    private Vector2 _scroll;
    private Entry[] _entries = new Entry[0];
    private string _usedPath = "<none>";          // e.g., HKCU\Software\Unity\UnityEditor\Company\Product
    private string _currentRelPath = null;         // e.g., Software\Unity\UnityEditor\Company\Product  (no HKCU\)
    private string _search = "";

    private static readonly Regex HashSuffix = new Regex(@"_h\d+$", RegexOptions.Compiled);

    private struct Entry
    {
        public string rawKey;      // "masterVolume_h3359004273"
        public string apiKey;      // "masterVolume"
        public string apiValue;    // value via PlayerPrefs
        public string storageKind; // registry kind
        public string raw;         // raw registry value (info)
    }

    [MenuItem("Tools/PlayerPrefs Viewer")]
    private static void Open()
    {
        var w = GetWindow<PlayerPrefsViewer>("PlayerPrefs");
        w.Refresh();
        w.Show();
    }

    private void OnGUI()
    {
        // Header path
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Registry Path:", GUILayout.Width(90));
        EditorGUILayout.SelectableLabel(_usedPath, GUILayout.Height(16));
        EditorGUILayout.EndHorizontal();

        // Toolbar: Refresh + Search
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(70)))
            Refresh();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Search", EditorStyles.miniLabel);
        _search = GUILayout.TextField(_search, EditorStyles.toolbarTextField, GUILayout.MinWidth(160));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox("API Value = value read via PlayerPrefs (exact). Click X to delete a single key.", MessageType.None);

        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        if (_entries.Length == 0)
        {
            EditorGUILayout.HelpBox("No PlayerPrefs found at the current path.", MessageType.Info);
        }
        else
        {
            DrawHeader();
            foreach (var e in _entries)
            {
                if (!PassesFilter(e)) continue;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.SelectableLabel(e.apiKey, GUILayout.MaxHeight(18), GUILayout.MinWidth(160));
                EditorGUILayout.SelectableLabel(e.apiValue, GUILayout.MaxHeight(18), GUILayout.MinWidth(120));
                EditorGUILayout.SelectableLabel(e.rawKey, GUILayout.MaxHeight(18), GUILayout.MinWidth(240));
                EditorGUILayout.SelectableLabel(e.storageKind, GUILayout.MaxHeight(18), GUILayout.Width(70));
                EditorGUILayout.SelectableLabel(e.raw, GUILayout.MaxHeight(18));
                // Delete button
                if (GUILayout.Button("X", GUILayout.Width(24)))
                {
                    if (EditorUtility.DisplayDialog("Delete Key",
                        $"Delete PlayerPrefs key:\n\n{e.apiKey} ?",
                        "Delete", "Cancel"))
                    {
                        DeleteEntry(e);
                        Refresh();
                        GUIUtility.ExitGUI();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();

        GUILayout.FlexibleSpace();

        // Delete all
        if (GUILayout.Button("Delete All PlayerPrefs"))
        {
            if (EditorUtility.DisplayDialog("Confirm", "Delete ALL PlayerPrefs?", "Yes", "No"))
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
                // Also clear all values under the current registry path (Windows editor only)
#if UNITY_EDITOR_WIN
                TryClearRegistryPath(_currentRelPath);
#endif
                Refresh();
            }
        }
    }

    private bool PassesFilter(Entry e)
    {
        if (string.IsNullOrEmpty(_search)) return true;
        var s = _search.ToLowerInvariant();
        return (e.apiKey?.ToLowerInvariant().Contains(s) ?? false)
            || (e.apiValue?.ToLowerInvariant().Contains(s) ?? false)
            || (e.rawKey?.ToLowerInvariant().Contains(s) ?? false);
    }

    private void DrawHeader()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label("Key (API)", GUILayout.MinWidth(160));
        GUILayout.Label("API Value", GUILayout.MinWidth(120));
        GUILayout.Label("Key (Raw Registry)", GUILayout.MinWidth(240));
        GUILayout.Label("Kind", GUILayout.Width(70));
        GUILayout.Label("Raw", GUILayout.ExpandWidth(true));
        GUILayout.Label("Del", GUILayout.Width(28));
        EditorGUILayout.EndHorizontal();
    }

    private void Refresh()
    {
        string company = Application.companyName;
        string product = Application.productName;

        string editorRel = $@"Software\Unity\UnityEditor\{company}\{product}";
        string playerRel = $@"Software\{company}\{product}";

        // Try Editor path first
        if (!TryRead(editorRel, out _entries))
        {
            if (!TryRead(playerRel, out _entries))
            {
                _usedPath = "<none>";
                _currentRelPath = null;
                _entries = new Entry[0];
                Repaint();
                return;
            }
            _usedPath = $@"HKCU\{playerRel}";
            _currentRelPath = playerRel;
        }
        else
        {
            _usedPath = $@"HKCU\{editorRel}";
            _currentRelPath = editorRel;
        }

        Repaint();
    }

    private bool TryRead(string relPath, out Entry[] outEntries)
    {
        using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(relPath, false))
        {
            if (regKey == null)
            {
                outEntries = new Entry[0];
                return false;
            }

            string[] valueNames = regKey.GetValueNames();
            var list = new List<Entry>(valueNames.Length);

            foreach (string rawName in valueNames)
            {
                object valueObj = regKey.GetValue(rawName);
                RegistryValueKind valueKind = regKey.GetValueKind(rawName);

                string apiKey = HashSuffix.Replace(rawName, string.Empty);
                string apiValue = ReadViaAPI(apiKey);
                string friendlyKind = valueKind.ToString();
                string rawDisplay = ToRawString(valueObj, valueKind);

                list.Add(new Entry
                {
                    rawKey = rawName,
                    apiKey = apiKey,
                    apiValue = apiValue,
                    storageKind = friendlyKind,
                    raw = rawDisplay
                });
            }

            outEntries = list.ToArray();
            return valueNames.Length > 0;
        }
    }

    private string ReadViaAPI(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            // Prefer float if present; otherwise int; otherwise string.
            float f = PlayerPrefs.GetFloat(key, float.NaN);
            if (!float.IsNaN(f)) return f.ToString("0.###");

            int i = PlayerPrefs.GetInt(key, int.MinValue);
            if (i != int.MinValue) return i.ToString();

            string s = PlayerPrefs.GetString(key, null);
            if (!string.IsNullOrEmpty(s)) return s;
        }
        return "<no key>";
    }

    private string ToRawString(object valueObj, RegistryValueKind kind)
    {
        if (valueObj == null) return "<null>";
        switch (kind)
        {
            case RegistryValueKind.Binary:
                var bytes = valueObj as byte[];
                if (bytes == null) return "<binary?>";
                if (bytes.Length == 8) return $"(double) {System.BitConverter.ToDouble(bytes, 0):0.###}";
                if (bytes.Length == 4) return $"(int) {System.BitConverter.ToInt32(bytes, 0)}";
                return BytesToHex(bytes);

            case RegistryValueKind.QWord:
                long q = (long)valueObj;
                return $"(double) {System.BitConverter.Int64BitsToDouble(q):0.###}";

            case RegistryValueKind.DWord:
                return $"(int) {valueObj}";

            case RegistryValueKind.String:
            case RegistryValueKind.ExpandString:
                return (string)valueObj;

            case RegistryValueKind.MultiString:
                return string.Join(", ", (string[])valueObj);

            default:
                return valueObj.ToString();
        }
    }

    private string BytesToHex(byte[] data)
    {
        var sb = new StringBuilder(data.Length * 2);
        foreach (byte b in data) sb.Append(b.ToString("X2"));
        return sb.ToString();
    }

    private void DeleteEntry(Entry e)
    {
        // Delete via API (cross-platform)
        PlayerPrefs.DeleteKey(e.apiKey);
        PlayerPrefs.Save();

        // Also try removing the raw registry value at the current path (Windows editor)
#if UNITY_EDITOR_WIN
        if (!string.IsNullOrEmpty(_currentRelPath) && !string.IsNullOrEmpty(e.rawKey))
        {
            try
            {
                using var k = Registry.CurrentUser.OpenSubKey(_currentRelPath, writable: true);
                k?.DeleteValue(e.rawKey, throwOnMissingValue: false);
            }
            catch { /* ignore */ }
        }
#endif
    }

#if UNITY_EDITOR_WIN
    private void TryClearRegistryPath(string relPath)
    {
        if (string.IsNullOrEmpty(relPath)) return;
        try
        {
            using var k = Registry.CurrentUser.OpenSubKey(relPath, writable: true);
            if (k == null) return;
            foreach (var name in k.GetValueNames())
                k.DeleteValue(name, false);
        }
        catch { /* ignore */ }
    }
#endif
}
#endif
