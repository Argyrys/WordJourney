using UnityEngine;

#if IAP_INSTALLED
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
#endif

#if IAP_INSTALLED
public class IAPManager : MonoBehaviour, IDetailedStoreListener
#else
public class IAPManager : MonoBehaviour
#endif
{
    public static IAPManager Instance { get; private set; }

#if IAP_INSTALLED
    private IStoreController controller;
    private IExtensionProvider extensions;
#endif

    private const string REMOVE_ADS = "com.wordjourney.removeads";
    private const string COINS_SMALL = "com.wordjourney.coins.small";
    private const string COINS_MEDIUM = "com.wordjourney.coins.medium";
    private const string COINS_LARGE = "com.wordjourney.coins.large";
    private const string HINTS_SMALL = "com.wordjourney.hints.small";
    private const string HINTS_MEDIUM = "com.wordjourney.hints.medium";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePurchasing();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePurchasing()
    {
#if IAP_INSTALLED
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(REMOVE_ADS, ProductType.NonConsumable);
        builder.AddProduct(COINS_SMALL, ProductType.Consumable);
        builder.AddProduct(COINS_MEDIUM, ProductType.Consumable);
        builder.AddProduct(COINS_LARGE, ProductType.Consumable);
        builder.AddProduct(HINTS_SMALL, ProductType.Consumable);
        builder.AddProduct(HINTS_MEDIUM, ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);
#else
        Debug.Log("IAP SDK not installed. Enable IAP_INSTALLED define or install Unity IAP package.");
#endif
    }

#if IAP_INSTALLED
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        this.controller = controller;
        this.extensions = extensions;
        Debug.Log("IAP Initialized");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError("IAP Initialization failed: " + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"IAP Initialization failed: {error} - {message}");
    }
#endif

    public void BuyProduct(string productId)
    {
#if IAP_INSTALLED
        if (controller != null)
        {
            controller.InitiatePurchase(productId);
        }
        else
        {
            Debug.LogError("IAP Controller not initialized");
        }
#else
        Debug.Log("IAP not available - SDK not installed");
#endif
    }

#if IAP_INSTALLED
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        string productId = args.purchasedProduct.definition.id;

        switch (productId)
        {
            case REMOVE_ADS:
                HandleRemoveAds();
                break;
            case COINS_SMALL:
                HandleCoinsPurchase(100);
                break;
            case COINS_MEDIUM:
                HandleCoinsPurchase(500);
                break;
            case COINS_LARGE:
                HandleCoinsPurchase(1500);
                break;
            case HINTS_SMALL:
                HandleHintsPurchase(5);
                break;
            case HINTS_MEDIUM:
                HandleHintsPurchase(15);
                break;
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.LogError($"Purchase failed: {product.definition.id} - {failureDescription.message}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError($"Purchase failed: {product.definition.id} - {failureReason}");
    }
#endif

    private void HandleRemoveAds()
    {
        PlayerPrefs.SetInt("AdsRemoved", 1);
        PlayerPrefs.Save();

        if (AdsManager.Instance != null)
        {
            AdsManager.Instance.HideBannerAd();
        }

        UIManager.Instance?.ShowMessage("Ads removed!");
    }

    private void HandleCoinsPurchase(int amount)
    {
        GameManager.Instance?.AddCoins(amount);
        UIManager.Instance?.UpdateCurrencyUI();
        UIManager.Instance?.ShowMessage($"+{amount} coins purchased!");
    }

    private void HandleHintsPurchase(int amount)
    {
        GameManager.Instance?.AddHints(amount);
        UIManager.Instance?.UpdateCurrencyUI();
        UIManager.Instance?.ShowMessage($"+{amount} hints purchased!");
    }

    public bool IsProductOwned(string productId)
    {
#if IAP_INSTALLED
        if (controller != null)
        {
            Product product = controller.products.WithID(productId);
            return product != null && product.hasReceipt;
        }
#endif
        return false;
    }

    public bool AreAdsRemoved()
    {
        return PlayerPrefs.GetInt("AdsRemoved", 0) == 1;
    }

    public string GetLocalizedPrice(string productId)
    {
#if IAP_INSTALLED
        if (controller != null)
        {
            Product product = controller.products.WithID(productId);
            if (product != null)
            {
                return product.metadata.localizedPriceString;
            }
        }
#endif
        return "N/A";
    }
}
