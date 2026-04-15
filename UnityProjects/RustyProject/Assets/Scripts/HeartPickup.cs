using UnityEngine;

/// <summary>
/// Сердце: подбирается игроком для получения дополнительной жизни.
/// Вешается на объект с Collider2D (isTrigger = true).
/// </summary>
public class HeartPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [Tooltip("Количество жизней, добавляемых при подборе")]
    public int lifeAmount = 1;

    [Header("Visual Settings")]
    [Tooltip("Цвет вспышки при подборе")]
    public Color flashColor = Color.white;

    [Tooltip("Время вспышки (секунды)")]
    public float flashDuration = 0.2f;

    [Tooltip("Время исчезновения (секунды)")]
    public float disappearDuration = 0.5f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isPickedUp = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPickedUp)
        {
            PickupHeart(other);
        }
    }

    private void PickupHeart(Collider2D playerCollider)
    {
        isPickedUp = true;

        // Добавляем жизни через GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddLife(lifeAmount);
            Debug.Log($"HeartPickup: Игрок получил {lifeAmount} жизнь(и)");
        }

        // Визуальный эффект подбора
        FlashVisual();

        // Исчезновение сердца
        StartCoroutine(DisappearSmoothly());
    }

    private void FlashVisual()
    {
        if (spriteRenderer == null) return;

        // Вспышка
        spriteRenderer.color = flashColor;
        Invoke(nameof(ResetColor), flashDuration);
    }

    private void ResetColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    private System.Collections.IEnumerator DisappearSmoothly()
    {
        if (spriteRenderer == null)
        {
            Destroy(gameObject);
            yield break;
        }

        // Плавное исчезновение
        Color startColor = spriteRenderer.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        float elapsedTime = 0f;
        while (elapsedTime < disappearDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsedTime / disappearDuration);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
    }
}
