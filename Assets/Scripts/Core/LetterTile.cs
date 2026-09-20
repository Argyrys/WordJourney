using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class LetterTile : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    public TextMeshProUGUI letterText;
    public Image backgroundImage;

    [HideInInspector]
    public char letter;
    [HideInInspector]
    public int gridX;
    [HideInInspector]
    public int gridY;
    [HideInInspector]
    public bool isFound = false;

    private bool isSelected = false;
    private Vector3 originalScale;
    private Color normalColor = new Color(0.95f, 0.95f, 1f);
    private Color selectedColor = new Color(0.3f, 0.7f, 1f);
    private Color foundColor = new Color(0.3f, 0.85f, 0.4f);

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void Initialize(char _letter, int x, int y)
    {
        letter = _letter;
        gridX = x;
        gridY = y;
        letterText.text = letter.ToString();
        backgroundImage.color = normalColor;
    }

    public void UpdateLetter(char newLetter)
    {
        letter = newLetter;
        letterText.text = letter.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isFound)
        {
            PuzzleGrid.Instance.OnLetterClicked(this);
        }
    }

    public void Select()
    {
        isSelected = true;
        backgroundImage.color = selectedColor;
        letterText.color = Color.white;
        StartCoroutine(ScaleAnimation(1.12f, 0.1f));
    }

    public void Deselect()
    {
        isSelected = false;
        if (!isFound)
        {
            backgroundImage.color = normalColor;
            letterText.color = new Color(0.15f, 0.15f, 0.2f);
        }
        StartCoroutine(ScaleAnimation(1f, 0.1f));
    }

    public void HighlightFound()
    {
        isFound = true;
        isSelected = false;
        backgroundImage.color = foundColor;
        letterText.color = Color.white;
        StartCoroutine(ScaleAnimation(1.15f, 0.15f));
    }

    public void HighlightInvalid()
    {
        StartCoroutine(ShakeAnimation());
    }

    private IEnumerator ScaleAnimation(float targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.one * targetScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t);
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        transform.localScale = endScale;
    }

    private IEnumerator ShakeAnimation()
    {
        Vector3 originalPos = transform.localPosition;
        float duration = 0.3f;
        float elapsed = 0f;
        backgroundImage.color = new Color(1f, 0.3f, 0.3f);

        while (elapsed < duration)
        {
            float x = Random.Range(-5f, 5f);
            transform.localPosition = originalPos + new Vector3(x, 0, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
        backgroundImage.color = isSelected ? selectedColor : normalColor;
    }
}
