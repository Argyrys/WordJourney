using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CrosswordDisplay : MonoBehaviour
{
    public static CrosswordDisplay Instance { get; private set; }

    public RectTransform wordContainer;

    [Header("Colors")]
    public Color emptySlotColor = new Color(0.06f, 0.12f, 0.22f, 0.92f);
    public Color foundWordColor = new Color(0.30f, 0.75f, 0.35f);
    public Color emptyTextColor = new Color(0.55f, 0.68f, 0.78f);
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

        if (containerWidth <= 0) containerWidth = 1000f;
        if (containerHeight <= 0) containerHeight = 600f;

        float paddingX = 20f;
        float paddingY = 15f;
        float availableWidth = containerWidth - paddingX * 2;
        float availableHeight = containerHeight - paddingY * 2;

        int leftCount = Mathf.CeilToInt(targetWords.Count / 2f);
        int rightCount = targetWords.Count - leftCount;
        int maxRows = Mathf.Max(leftCount, rightCount);

        float spacingY = 12f;
        float rowHeight = Mathf.Min((availableHeight - spacingY * (maxRows - 1)) / maxRows, 65f);
        rowHeight = Mathf.Max(rowHeight, 45f);
        float colGap = 40f;

        float maxCharPerSlot = 0;
        foreach (string w in targetWords)
            if (w.Length > maxCharPerSlot) maxCharPerSlot = w.Length;

        float maxSlotWidth = (availableWidth - colGap) / 2f;
        float charWidth = maxSlotWidth / (maxCharPerSlot + 2f);
        charWidth = Mathf.Min(charWidth, 48f);
        charWidth = Mathf.Max(charWidth, 32f);

        float leftColX = -availableWidth / 4f - colGap / 4f;
        float rightColX = availableWidth / 4f + colGap / 4f;

        float totalHeight = maxRows * rowHeight + (maxRows - 1) * spacingY;
        float startY = totalHeight / 2f - rowHeight / 2f;

        for (int i = 0; i < targetWords.Count; i++)
        {
            string word = targetWords[i];
            bool isLeft = i % 2 == 0;
            int row = i / 2;

            float slotWidth = (word.Length + 1.5f) * charWidth;
            slotWidth = Mathf.Max(slotWidth, 90f);
            slotWidth = Mathf.Min(slotWidth, maxSlotWidth);

            float x = isLeft ? leftColX : rightColX;
            float y = startY - row * (rowHeight + spacingY);

            float stagger = isLeft ? 0f : rowHeight * 0.35f;
            y -= stagger;

            CreateWordSlot(word, new Vector2(x, y), slotWidth, rowHeight);
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
        sRect.sizeDelta = new Vector2(2, -2);
        sRect.anchoredPosition = new Vector2(1, -1);
        Image sImg = shadowObj.AddComponent<Image>();
        sImg.color = new Color(0, 0, 0, 0.2f);
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
        tmp.fontSize = 28;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = emptyTextColor;
        tmp.characterSpacing = 6;

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
