#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public class DefineSymbolsSetup
{
    static DefineSymbolsSetup()
    {
        AddDefineSymbols();
    }

    [MenuItem("Tools/Add Define Symbols")]
    public static void AddDefineSymbols()
    {
        string scriptingDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);

        List<string> defines = new List<string>(scriptingDefines.Split(';'));
        bool changed = false;

        if (!defines.Contains("IAP_INSTALLED"))
        {
            defines.Add("IAP_INSTALLED");
            changed = true;
            Debug.Log("Added IAP_INSTALLED define symbol");
        }

        if (!defines.Contains("ADMOB_INSTALLED"))
        {
            defines.Add("ADMOB_INSTALLED");
            changed = true;
            Debug.Log("Added ADMOB_INSTALLED define symbol");
        }

        if (changed)
        {
            string newDefines = string.Join(";", defines);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, newDefines);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, newDefines);
            Debug.Log("Define symbols updated: " + newDefines);
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("Tools/Remove All Custom Defines")]
    public static void RemoveAllDefines()
    {
        string scriptingDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);

        List<string> defines = new List<string>(scriptingDefines.Split(';'));
        defines.Remove("ADMOB_INSTALLED");
        defines.Remove("IAP_INSTALLED");

        string newDefines = string.Join(";", defines);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, newDefines);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, newDefines);

        Debug.Log("Removed custom defines. Current: " + newDefines);
        AssetDatabase.Refresh();
    }
}
#endif
