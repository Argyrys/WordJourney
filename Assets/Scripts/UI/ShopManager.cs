using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("References")]
    public MenuManager menuManager;

    [Header("IAP Buttons")]
    public Button removeAdsButton;
    public Button coinsSmallButton;
    public Button coinsMediumButton;
    public Button coinsLargeButton;
    public Button hintsSmallButton;
    public Button hintsMediumButton;

    [Header("Price Texts")]
    public TextMeshProUGUI removeAdsPrice;
    public TextMeshProUGUI coinsSmallPrice;
    public TextMeshProUGUI coinsMediumPrice;
    public TextMeshProUGUI coinsLargePrice;
    public TextMeshProUGUI hintsSmallPrice;
    public TextMeshProUGUI hintsMediumPrice;

    [Header("Status")]
    public GameObject adsRemovedBadge;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI coinsBalanceText;

    private void OnEnable()
    {
        UpdatePrices();
        UpdateStatus();
    }

    private void Start()
    {
        SetupButtons();
        UpdatePrices();
        UpdateStatus();
    }

    private void SetupButtons()
    {
        removeAdsButton?.onClick.AddListener(OnRemoveAdsClicked);
        coinsSmallButton?.onClick.AddListener(OnCoinsSmallClicked);
        coinsMediumButton?.onClick.AddListener(OnCoinsMediumClicked);
        coinsLargeButton?.onClick.AddListener(OnCoinsLargeClicked);
        hintsSmallButton?.onClick.AddListener(OnHintsSmallClicked);
        hintsMediumButton?.onClick.AddListener(OnHintsMediumClicked);
    }

    private void UpdatePrices()
    {
        if (IAPManager.Instance != null)
        {
            removeAdsPrice.text = IAPManager.Instance.GetLocalizedPrice("com.wordjourney.removeads");
            coinsSmallPrice.text = IAPManager.Instance.GetLocalizedPrice("com.wordjourney.coins.small");
            coinsMediumPrice.text = IAPManager.Instance.GetLocalizedPrice("com.wordjourney.coins.medium");
            coinsLargePrice.text = IAPManager.Instance.GetLocalizedPrice("com.wordjourney.coins.large");
            hintsSmallPrice.text = IAPManager.Instance.GetLocalizedPrice("com.wordjourney.hints.small");
            hintsMediumPrice.text = IAPManager.Instance.GetLocalizedPrice("com.wordjourney.hints.medium");
        }
    }

    private void UpdateStatus()
    {
        if (adsRemovedBadge != null)
        {
            bool adsRemoved = PlayerPrefs.GetInt("AdsRemoved", 0) == 1;
            adsRemovedBadge.SetActive(adsRemoved);
            removeAdsButton.interactable = !adsRemoved;
        }
        if (coinsBalanceText != null && GameManager.Instance != null)
        {
            coinsBalanceText.text = $"\u25CF {GameManager.Instance.coins}";
        }
    }

    private void OnRemoveAdsClicked()
    {
        IAPManager.Instance?.BuyProduct("com.wordjourney.removeads");
        UpdateStatus();
    }

    private void OnCoinsSmallClicked()
    {
        IAPManager.Instance?.BuyProduct("com.wordjourney.coins.small");
    }

    private void OnCoinsMediumClicked()
    {
        IAPManager.Instance?.BuyProduct("com.wordjourney.coins.medium");
    }

    private void OnCoinsLargeClicked()
    {
        IAPManager.Instance?.BuyProduct("com.wordjourney.coins.large");
    }

    private void OnHintsSmallClicked()
    {
        IAPManager.Instance?.BuyProduct("com.wordjourney.hints.small");
    }

    private void OnHintsMediumClicked()
    {
        IAPManager.Instance?.BuyProduct("com.wordjourney.hints.medium");
    }

    public void BackToMenu()
    {
        menuManager?.BackToMainMenu();
    }
}
