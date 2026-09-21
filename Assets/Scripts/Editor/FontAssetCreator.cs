#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;

public class FontAssetCreator : MonoBehaviour
{
    [MenuItem("Tools/Create TMP Font Asset")]
    public static void CreateFontAsset()
    {
        string fontPath = "Assets/Fonts/Nunito.ttf";
        string outputPath = "Assets/Fonts/Nunito SDF.asset";

        Font font = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
        if (font == null)
        {
            Debug.LogError("Font not found at: " + fontPath);
            return;
        }

        TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(font, 90, 8192);
        if (fontAsset != null)
        {
            fontAsset.name = "Nunito SDF";
            AssetDatabase.CreateAsset(fontAsset, outputPath);
            AssetDatabase.SaveAssets();
            Debug.Log("Font asset created at: " + outputPath);
        }
        else
        {
            Debug.LogError("Failed to create font asset");
        }
    }
}
#endif
