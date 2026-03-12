using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

// Attach to: SystemCanvas or UI_Manager
// Required: Reference to EffectText (TextMeshPro)
public class BonusUIManager : MonoBehaviour
{
    public static BonusUIManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI effectText;

    private Dictionary<string, float> activeEffects = new Dictionary<string, float>();

    void Awake() => Instance = this;

    void Start() => ClearEffectText();

    void Update()
    {
        // Update timers and remove expired (using ToList to avoid modification during iteration)
        foreach (var kvp in activeEffects.ToList())
        {
            activeEffects[kvp.Key] -= Time.deltaTime;
            if (activeEffects[kvp.Key] <= 0)
            {
                OnEffectExpired(kvp.Key);
                activeEffects.Remove(kvp.Key);
            }
        }

        // Update UI
        UpdateEffectText();
    }

    void OnEffectExpired(string effectName)
    {
        // Reset platform effects
        if (effectName == "Platform Expand" || effectName == "Platform Shrink")
        {
            var platform = FindObjectOfType<PlatformController>();
            platform?.ResetPlatform();
        }
        // Ball speed effects don't have automatic reset
    }

    public void ShowEffect(string effectName, float duration)
    {
        // Check for opposite effects and cancel them
        string oppositeEffect = GetOppositeEffect(effectName);
        
        if (!string.IsNullOrEmpty(oppositeEffect) && activeEffects.ContainsKey(oppositeEffect))
        {
            // Remove opposite effect (they cancel each other)
            activeEffects.Remove(oppositeEffect);
            Debug.Log($"[BonusUI] {effectName} canceled {oppositeEffect}");
            
            // Reset platform for opposite effects
            if (effectName.Contains("Platform"))
            {
                var platform = FindObjectOfType<PlatformController>();
                platform?.ResetPlatform();
            }
        }

        // Add or update the effect
        if (activeEffects.ContainsKey(effectName))
            activeEffects[effectName] = duration;
        else
            activeEffects.Add(effectName, duration);

        UpdateEffectText();
    }

    string GetOppositeEffect(string effectName)
    {
        switch (effectName)
        {
            case "Ball Speed Up": return "Ball Speed Down";
            case "Ball Speed Down": return "Ball Speed Up";
            case "Platform Expand": return "Platform Shrink";
            case "Platform Shrink": return "Platform Expand";
            default: return null;
        }
    }

    void UpdateEffectText()
    {
        if (effectText == null) return;

        if (activeEffects.Count == 0)
        {
            effectText.text = "";
            return;
        }

        string text = "";
        foreach (var kvp in activeEffects)
        {
            if (!string.IsNullOrEmpty(text))
                text += "\n";
            text += $"{kvp.Key}: {kvp.Value:F1}s";
        }

        effectText.text = text;
    }

    public void ClearEffectText()
    {
        activeEffects.Clear();
        if (effectText != null)
            effectText.text = "";
    }
}
