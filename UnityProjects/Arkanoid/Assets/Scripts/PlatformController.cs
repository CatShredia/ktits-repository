using UnityEngine;

// Player platform
// Tag: Player
public class PlatformController : MonoBehaviour
{
    [SerializeField] private float playerVelocity = 0.5f;
    [SerializeField] private float boundary = 1.8f;

    private Vector3 playerPosition;
    private Vector3 originalScale;
    private float currentScaleMultiplier = 1f;

    void Start()
    {
        playerPosition = transform.position;
        originalScale = transform.localScale;

        Debug.Log($"[Platform] Start() - originalScale: {originalScale}");
    }

    void Update()
    {
        if (MenuController.Instance != null && !MenuController.Instance.IsGameStarted) return;

        // Мобильное управление: левая/правая часть экрана
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            float screenWidth = Screen.width;

            if (touch.position.x < screenWidth / 2)
            {
                // Левая часть экрана — движение влево
                playerPosition.x -= playerVelocity * 2f;
            }
            else
            {
                // Правая часть экрана — движение вправо
                playerPosition.x += playerVelocity * 2f;
            }
        }
        else
        {
            playerPosition.x += Input.GetAxis("Horizontal") * playerVelocity;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        transform.position = new Vector3(
            Mathf.Clamp(playerPosition.x, -boundary, boundary),
            playerPosition.y,
            playerPosition.z);

        playerPosition = transform.position;
    }

    // ! Умножить ширину платформы (мультипликативно к текущему масштабу скина)
    // Вызывается из BonusController при получении бонуса
    public void ExpandPlatform(float amount)
    {
        currentScaleMultiplier += amount;
        ApplyPlatformScale();
    }

    // ! Уменьшить ширину платформы (мультипликативно к текущему масштабу скина)
    // Вызывается из BonusController при получении бонуса
    public void ShrinkPlatform(float amount)
    {
        currentScaleMultiplier += amount; // amount отрицательный
        ApplyPlatformScale();
    }

    public void ResetPlatform()
    {
        currentScaleMultiplier = 1f;
        ApplyPlatformScale();
        Debug.Log($"[Platform] ResetPlatform() - scale: {transform.localScale}");
    }

    // ! Применить масштаб: originalScale (вкл. скин) × currentScaleMultiplier
    // Вызывается из ExpandPlatform, ShrinkPlatform, ResetPlatform
    private void ApplyPlatformScale()
    {
        transform.localScale = new Vector3(
            originalScale.x * currentScaleMultiplier,
            originalScale.y,
            originalScale.z
        );
    }
}
