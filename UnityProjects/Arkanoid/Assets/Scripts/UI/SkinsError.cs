using UnityEngine;
using TMPro;
using System.Collections;

public class SkinsError : MonoBehaviour
{
    public static SkinsError Instance { get; private set; }

    [Header("=== UI References ===")]
    [SerializeField] private TextMeshProUGUI errorText;

    [Header("=== Animation ===")]
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float displayDuration = 2f;

    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;
    private Color originalColor;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (errorText != null)
        {
            originalColor = errorText.color;
            errorText.gameObject.SetActive(false);
        }
    }

    // ! Показать ошибку покупки
    // Вызывается из ShopController.PurchaseSkin
    public void ShowPurchaseError(PurchaseErrorCode errorCode, string customMessage = null)
    {
        string message = customMessage ?? GetPurchaseErrorMessage(errorCode);
        ShowError(message);
    }

    // ! Показать ошибку экипировки
    // Вызывается из ShopController.EquipSkin
    public void ShowEquipError(EquipErrorCode errorCode, string customMessage = null)
    {
        string message = customMessage ?? GetEquipErrorMessage(errorCode);
        ShowError(message);
    }

    // ! Показать общее сообщение об ошибке
    // Вызывается из ShowPurchaseError, ShowEquipError
    public void ShowError(string message, Color? color = null)
    {
        if (errorText == null)
        {
            Debug.LogError("[SkinsError] errorText not assigned!");
            return;
        }

        // Остановить предыдущую анимацию
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        errorText.text = message;
        errorText.color = color ?? originalColor;
        errorText.gameObject.SetActive(true);

        fadeCoroutine = StartCoroutine(ShowErrorSequence());
    }

    private IEnumerator ShowErrorSequence()
    {
        // Плавное появление
        float elapsed = 0f;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;

        // Ожидание показа
        yield return new WaitForSecondsRealtime(displayDuration);

        // Плавное исчезновение
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        errorText.gameObject.SetActive(false);
        fadeCoroutine = null;
    }

    private string GetPurchaseErrorMessage(PurchaseErrorCode code)
    {
        return code switch
        {
            PurchaseErrorCode.AlreadyOwned => "Этот скин уже есть у вас",
            PurchaseErrorCode.InsufficientCoins => "Недостаточно монет",
            PurchaseErrorCode.SkinNotFound => "Скин не найден",
            PurchaseErrorCode.SkinNotAvailable => "Скин недоступен для покупки",
            PurchaseErrorCode.UserNotFound => "Пользователь не найден",
            _ => "Ошибка покупки"
        };
    }

    private string GetEquipErrorMessage(EquipErrorCode code)
    {
        return code switch
        {
            EquipErrorCode.AlreadyEquipped => "Этот скин уже экипирован",
            EquipErrorCode.SkinNotFound => "Скин не найден",
            EquipErrorCode.SkinNotOwned => "Скин не принадлежит вам",
            EquipErrorCode.SkinDataNotFound => "Данные скина не найдены",
            _ => "Ошибка экипировки"
        };
    }
}

public enum PurchaseErrorCode
{
    None = 0,
    SkinNotFound = 1,
    SkinNotAvailable = 2,
    AlreadyOwned = 3,
    InsufficientCoins = 4,
    UserNotFound = 5
}

public enum EquipErrorCode
{
    None = 0,
    SkinNotFound = 1,
    SkinNotOwned = 2,
    AlreadyEquipped = 3,
    SkinDataNotFound = 4
}
