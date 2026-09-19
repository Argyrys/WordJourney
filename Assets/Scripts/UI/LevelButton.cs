using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI levelText;
    public Image[] starsImages;
    public Button button;
    public Image lockImage;

    [Header("Colors")]
    public Color availableColor = Color.white;
    public Color lockedColor = Color.gray;
    public Color completedColor = Color.green;
    public Color starActiveColor = Color.yellow;
    public Color starInactiveColor = Color.gray;

    private int levelNumber;
    private bool isUnlocked;
    private Image backgroundImage;

    private void Awake()
    {
        backgroundImage = GetComponent<Image>();
        if (button == null) button = GetComponent<Button>();
    }

    public void Setup(int level, int stars, bool unlocked)
    {
        levelNumber = level;
        isUnlocked = unlocked;

        if (levelText != null) levelText.text = level.ToString();

        if (lockImage != null)
        {
            lockImage.gameObject.SetActive(!unlocked);
        }

        if (button != null) button.interactable = unlocked;

        if (backgroundImage != null)
        {
            if (unlocked)
            {
                backgroundImage.color = stars > 0 ? completedColor : availableColor;
            }
            else
            {
                backgroundImage.color = lockedColor;
            }
        }

        if (starsImages != null)
        {
            for (int i = 0; i < starsImages.Length; i++)
            {
                if (starsImages[i] != null)
                {
                    starsImages[i].color = i < stars ? starActiveColor : starInactiveColor;
                    starsImages[i].gameObject.SetActive(stars > 0 || unlocked);
                }
            }
        }

        if (button != null) button.onClick.AddListener(OnLevelClicked);
    }

    private void OnLevelClicked()
    {
        if (isUnlocked && GameManager.Instance != null)
        {
            GameManager.Instance.StartLevel(levelNumber);
        }
    }
}
