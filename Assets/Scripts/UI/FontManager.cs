using UnityEngine;
using TMPro;

public class FontManager : MonoBehaviour
{
    public static FontManager Instance { get; private set; }

    public TMP_FontAsset gameFont;

    private void Awake()
    {
        Instance = this;
        if (gameFont == null)
        {
            gameFont = Resources.Load<TMP_FontAsset>("Nunito SDF");
        }
    }

    public void ApplyFontToAll()
    {
        if (gameFont == null) return;

        TextMeshProUGUI[] allText = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        foreach (TextMeshProUGUI tmp in allText)
        {
            if (tmp.font != gameFont)
            {
                tmp.font = gameFont;
            }
        }
    }

    public void ApplyFontToObject(GameObject obj)
    {
        if (gameFont == null || obj == null) return;

        TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.font = gameFont;
            return;
        }

        TextMeshProUGUI[] children = obj.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI child in children)
        {
            child.font = gameFont;
        }
    }
}
