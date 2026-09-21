#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class FontAssetCreator : MonoBehaviour
{
    [MenuItem("Tools/Create TMP Font Asset")]
    public static void CreateFontAsset()
    {
        string fontPath = "Assets/Fonts/Nunito.ttf";

        if (!System.IO.File.Exists(fontPath))
        {
            Debug.LogError("Font not found at: " + fontPath);
            return;
        }

        Debug.Log("To create the font asset:\n1. Go to Window > TextMeshPro > Font Asset Creator\n2. Select Assets/Fonts/Nunito.ttf\n3. Click Generate Font Atlas\n4. Save as Assets/Fonts/Nunito SDF.asset");
        EditorUtility.DisplayDialog("Create Font Asset",
            "1. Go to Window > TextMeshPro > Font Asset Creator\n" +
            "2. Select Assets/Fonts/Nunito.ttf\n" +
            "3. Click Generate Font Atlas\n" +
            "4. Save as Assets/Fonts/Nunito SDF.asset",
            "OK");
    }
}
#endif
