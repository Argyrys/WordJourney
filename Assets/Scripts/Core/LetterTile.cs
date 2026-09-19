using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LetterTile : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    public Text letterText;
    public Image backgroundImage;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;
    public Color foundColor = Color.green;

    [HideInInspector]
    public char letter;
    [HideInInspector]
    public int gridX;
    [HideInInspector]
    public int gridY;
    [HideInInspector]
    public bool isFound = false;

    private bool isSelected = false;

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
        transform.localScale = Vector3.one * 1.1f;
    }

    public void Deselect()
    {
        isSelected = false;
        if (!isFound)
        {
            backgroundImage.color = normalColor;
        }
        transform.localScale = Vector3.one;
    }

    public void HighlightFound()
    {
        isFound = true;
        isSelected = false;
        backgroundImage.color = foundColor;
        transform.localScale = Vector3.one;
    }

    public void HighlightInvalid()
    {
        StartCoroutine(ShakeAnimation());
    }

    private System.Collections.IEnumerator ShakeAnimation()
    {
        Vector3 originalPos = transform.position;
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-0.1f, 0.1f);
            transform.position = originalPos + new Vector3(x, 0, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
    }
}
