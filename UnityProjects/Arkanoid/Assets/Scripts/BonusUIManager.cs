using UnityEngine;
using TMPro;

// Attach to: SystemCanvas or UI_Manager
// Required: Reference to EffectText (TextMeshPro)
public class BonusUIManager : MonoBehaviour
{
    public static BonusUIManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI effectText;
    [SerializeField] private float displayDuration = 2f;

    private float hideTimer = 0f;

    void Awake() => Instance = this;

    void Start() => ClearEffectText();

    void Update()
    {
        if (hideTimer > 0)
        {
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0)
                ClearEffectText();
        }
    }

    public void ShowEffect(string effectName, float duration)
    {
        if (effectText != null)
        {
            effectText.text = $"{effectName}: {duration}s";
            hideTimer = displayDuration;
        }
    }

    public void ClearEffectText()
    {
        if (effectText != null)
            effectText.text = "";
    }
}