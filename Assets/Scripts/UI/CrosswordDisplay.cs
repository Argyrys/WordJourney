using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CrosswordDisplay : MonoBehaviour
{
    public static CrosswordDisplay Instance { get; private set; }

    public RectTransform wordContainer;

    [Header("Colors")]
    public Color emptySlotColor = new Color(0.08f, 0.15f, 0.25f, 0.95f);
    public Color foundWordColor = new Color(0.2f, 0.82f, 0.48f);
    public Color emptyTextColor = new Color(0.4f, 0.55f, 0.65f);
    public Color foundTextColor = Color.white;

    private List<WordSlot> wordSlots = new List<WordSlot>();

    private void Awake()
    {
        Instance = this;
    }

    public void SetupWords(List<string> targetWords)
    {
        ClearWords();

        if (wordContainer == null) return;

        float containerWidth = wordContainer.rect.width;
        float containerHeight = wordContainer.rect.height;

        if (containerWidth <= 0) containerWidth = 900f;
        if (containerHeight <= 0) containerHeight = 400f;

        float padding = 20f;
        float availableWidth = containerWidth - padding * 2;
        float availableHeight = containerHeight - padding * 2;

        int maxPerRow = Mathf.CeilToInt(targetWords.Count / 2f);
        int rows = targetWords.Count <= maxPerRow ? 1 : 2;

        float slotHeight = Mathf.Min(availableHeight / rows - 10f, 90f);
        float spacingX = 12f;
        float spacingY = 15f;
        float slotWidth = (availableWidth - spacingX * (maxPerRow - 1)) / maxPerRow;

        slotWidth = Mathf.Min(slotWidth, 200f);

        float totalWidth = maxPerRow * slotWidth + (maxPerRow - 1) * spacingX;
        float totalHeight = rows * slotHeight + (rows - 1) * spacingY;

        for (int i = 0; i < targetWords.Count; i++)
        {
            string word = targetWords[i];
            int row = i < maxPerRow ? 0 : 1;
            int col = row == 0 ? i : i - maxPerRow;

            float x = -totalWidth / 2f + col * (slotWidth + spacingX) + slotWidth / 2f;
            float y = totalHeight / 2f - row * (slotHeight + spacingY) - slotHeight / 2f;

            CreateWordSlot(word, new Vector2(x, y), slotWidth, slotHeight);
        }
    }

    private void CreateWordSlot(string word, Vector2 position, float width, float height)
    {
        GameObject slotObj = new GameObject($"Word_{word}");
        slotObj.transform.SetParent(wordContainer, false);

        RectTransform rect = slotObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(width, height);

        GameObject shadowObj = new GameObject("Shadow");
        shadowObj.transform.SetParent(slotObj.transform, false);
        RectTransform sRect = shadowObj.AddComponent<RectTransform>();
        sRect.anchorMin = Vector2.zero;
        sRect.anchorMax = Vector2.one;
        sRect.sizeDelta = new Vector2(3, -3);
        sRect.anchoredPosition = new Vector2(2, -2);
        Image sImg = shadowObj.AddComponent<Image>();
        sImg.color = new Color(0, 0, 0, 0.25f);
        shadowObj.transform.SetAsFirstSibling();

        Image bg = slotObj.AddComponent<Image>();
        bg.color = emptySlotColor;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(slotObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = new string('_', word.Length);
        tmp.fontSize = 42;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = emptyTextColor;

        WordSlot slot = new WordSlot
        {
            word = word,
            background = bg,
            text = tmp,
            found = false
        };
        wordSlots.Add(slot);
    }

    public void OnWordFound(string word)
    {
        foreach (WordSlot slot in wordSlots)
        {
            if (!slot.found && slot.word == word)
            {
                slot.found = true;
                slot.text.text = word;
                slot.text.color = foundTextColor;
                slot.background.color = foundWordColor;
                break;
            }
        }
    }

    private void ClearWords()
    {
        foreach (WordSlot slot in wordSlots)
        {
            if (slot.background != null)
                Destroy(slot.background.gameObject);
        }
        wordSlots.Clear();
    }
}

[System.Serializable]
public class WordSlot
{
    public string word;
    public Image background;
    public TextMeshProUGUI text;
    public bool found;
}
