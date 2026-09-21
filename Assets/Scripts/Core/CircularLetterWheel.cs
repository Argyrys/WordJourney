using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class CircularLetterWheel : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public static CircularLetterWheel Instance { get; private set; }

    [Header("Layout")]
    public RectTransform wheelCenter;
    public float wheelRadius = 160f;
    public float letterSize = 70f;

    [Header("Colors")]
    public Color[] letterColors = new Color[]
    {
        new Color(0.20f, 0.75f, 0.35f),
        new Color(0.90f, 0.75f, 0.15f),
        new Color(0.20f, 0.50f, 0.95f),
        new Color(0.90f, 0.25f, 0.40f),
        new Color(0.60f, 0.30f, 0.85f),
        new Color(0.15f, 0.80f, 0.80f),
        new Color(0.95f, 0.50f, 0.20f),
        new Color(0.40f, 0.80f, 0.30f)
    };

    [Header("Selection Line")]
    public Image selectionLine;
    public Color lineColor = new Color(0.3f, 0.92f, 1f, 0.85f);

    private List<WheelLetter> letters = new List<WheelLetter>();
    private List<WheelLetter> selectedLetters = new List<WheelLetter>();
    private string currentWord = "";
    private char[] wheelChars;

    private LineRenderer lineRenderer;
    private Sprite circleSprite;

    private void Awake()
    {
        Instance = this;
        SetupLineRenderer();
        circleSprite = UISpriteGenerator.CreateCircle(128, Color.white);
    }

    private void SetupLineRenderer()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.startWidth = 8f;
        lineRenderer.endWidth = 8f;
        lineRenderer.positionCount = 0;
        lineRenderer.sortingOrder = 10;
    }

    public void SetupWheel(char[] chars)
    {
        ClearWheel();
        wheelChars = chars;

        List<char> uniqueChars = new List<char>();
        foreach (char c in chars)
        {
            if (!uniqueChars.Contains(c))
                uniqueChars.Add(c);
        }

        int count = uniqueChars.Count;
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = (angleStep * i - 90f) * Mathf.Deg2Rad;
            Vector2 position = new Vector2(
                Mathf.Cos(angle) * wheelRadius,
                Mathf.Sin(angle) * wheelRadius
            );

            GameObject letterObj = CreateLetterObject(uniqueChars[i], position, i);
            WheelLetter wl = letterObj.GetComponent<WheelLetter>();
            letters.Add(wl);
        }
    }

    private GameObject CreateLetterObject(char letter, Vector2 position, int index)
    {
        GameObject obj = new GameObject($"Letter_{letter}");
        obj.transform.SetParent(wheelCenter, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(letterSize, letterSize);

        GameObject shadowObj = new GameObject("Shadow");
        shadowObj.transform.SetParent(obj.transform, false);
        RectTransform sRect = shadowObj.AddComponent<RectTransform>();
        sRect.anchorMin = Vector2.zero;
        sRect.anchorMax = Vector2.one;
        sRect.sizeDelta = new Vector2(3, -3);
        sRect.anchoredPosition = new Vector2(2, -2);
        Image sImg = shadowObj.AddComponent<Image>();
        sImg.sprite = circleSprite;
        sImg.color = new Color(0, 0, 0, 0.3f);
        shadowObj.transform.SetAsFirstSibling();

        Image bg = obj.AddComponent<Image>();
        bg.sprite = circleSprite;
        bg.color = letterColors[index % letterColors.Length];

        // Add glossiness overlay (top half shine)
        GameObject glossObj = new GameObject("Gloss");
        glossObj.transform.SetParent(obj.transform, false);
        RectTransform gRect = glossObj.AddComponent<RectTransform>();
        gRect.anchorMin = new Vector2(0, 0.5f);
        gRect.anchorMax = new Vector2(1, 1);
        gRect.sizeDelta = Vector2.zero;
        gRect.anchoredPosition = Vector2.zero;
        Image gImg = glossObj.AddComponent<Image>();
        gImg.sprite = UISpriteGenerator.CreateGradient(64, 64, new Color(1, 1, 1, 0.35f), new Color(1, 1, 1, 0f));
        gImg.type = Image.Type.Simple;
        gImg.raycastTarget = false;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(obj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = letter.ToString();
        tmp.fontSize = 34;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        WheelLetter wl = obj.AddComponent<WheelLetter>();
        wl.letter = letter;
        wl.background = bg;
        wl.normalColor = bg.color;
        wl.index = index;

        return obj;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ClearSelection();
        CheckLetterUnderPointer(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        CheckLetterUnderPointer(eventData);
        UpdateLineRenderer();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SubmitWord();
        ClearSelection();
        UpdateLineRenderer();
    }

    private void CheckLetterUnderPointer(PointerEventData eventData)
    {
        foreach (WheelLetter wl in letters)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(
                wl.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera))
            {
                if (!selectedLetters.Contains(wl))
                {
                    if (selectedLetters.Count == 0 || IsAdjacent(selectedLetters[selectedLetters.Count - 1], wl))
                    {
                        SelectLetter(wl);
                    }
                }
                break;
            }
        }
    }

    private bool IsAdjacent(WheelLetter a, WheelLetter b)
    {
        float dist = Vector2.Distance(
            a.GetComponent<RectTransform>().anchoredPosition,
            b.GetComponent<RectTransform>().anchoredPosition
        );
        return dist < letterSize * 2.5f;
    }

    private void SelectLetter(WheelLetter wl)
    {
        selectedLetters.Add(wl);
        wl.SetSelected(true);
        UpdateCurrentWord();
        UpdateLineRenderer();
    }

    private void ClearSelection()
    {
        foreach (WheelLetter wl in selectedLetters)
        {
            wl.SetSelected(false);
        }
        selectedLetters.Clear();
        currentWord = "";
        UIManager.Instance?.UpdateCurrentWord("");
    }

    private void UpdateCurrentWord()
    {
        currentWord = "";
        foreach (WheelLetter wl in selectedLetters)
        {
            currentWord += wl.letter;
        }
        UIManager.Instance?.UpdateCurrentWord(currentWord);
    }

    private void UpdateLineRenderer()
    {
        if (selectedLetters.Count < 2)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        lineRenderer.positionCount = selectedLetters.Count;
        for (int i = 0; i < selectedLetters.Count; i++)
        {
            Vector3 pos = selectedLetters[i].GetComponent<RectTransform>().anchoredPosition;
            lineRenderer.SetPosition(i, new Vector3(pos.x, pos.y, 0));
        }
    }

    private void SubmitWord()
    {
        if (string.IsNullOrEmpty(currentWord) || currentWord.Length < 3)
        {
            if (selectedLetters.Count > 0)
                UIManager.Instance?.ShowMessage("Need at least 3 letters!");
            return;
        }

        if (WordValidator.Instance != null && WordValidator.Instance.IsValidWord(currentWord))
        {
            if (WordValidator.Instance.IsTargetWord(currentWord))
            {
                LevelManager.Instance.WordFound(currentWord);
                AudioManager.Instance?.PlayWordFound();
            }
            else
            {
                UIManager.Instance?.ShowMessage("Not a target word!");
            }
        }
        else
        {
            UIManager.Instance?.ShowMessage("Invalid word!");
        }
    }

    public void RevealWord(string word)
    {
        UIManager.Instance?.ShowMessage($"Word: {word}");
    }

    public void ShuffleLetters()
    {
        List<char> chars = new List<char>();
        foreach (WheelLetter wl in letters)
            chars.Add(wl.letter);

        for (int i = chars.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }

        float angleStep = 360f / letters.Count;
        for (int i = 0; i < letters.Count; i++)
        {
            float angle = (angleStep * i - 90f) * Mathf.Deg2Rad;
            Vector2 position = new Vector2(
                Mathf.Cos(angle) * wheelRadius,
                Mathf.Sin(angle) * wheelRadius
            );
            letters[i].GetComponent<RectTransform>().anchoredPosition = position;
            letters[i].letter = chars[i];
            letters[i].GetComponentInChildren<TextMeshProUGUI>().text = chars[i].ToString();
        }

        AudioManager.Instance?.PlayShuffle();
    }

    private void ClearWheel()
    {
        foreach (WheelLetter wl in letters)
        {
            if (wl != null)
                Destroy(wl.gameObject);
        }
        letters.Clear();
        selectedLetters.Clear();
    }
}

public class WheelLetter : MonoBehaviour
{
    public char letter;
    public Image background;
    public Color normalColor;
    public Color selectedColor = new Color(1f, 1f, 1f, 0.95f);
    public int index;

    public void SetSelected(bool selected)
    {
        if (background != null)
        {
            background.color = selected ? selectedColor : normalColor;
        }
        transform.localScale = selected ? Vector3.one * 1.18f : Vector3.one;
    }
}
