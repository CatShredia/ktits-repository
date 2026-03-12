using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

// SystemCanvas
public class BonusUIManager : MonoBehaviour
{
    public static BonusUIManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI effectText;

    private Dictionary<string, EffectData> activeEffects = new Dictionary<string, EffectData>();

    [System.Serializable]
    public class EffectData
    {
        public float duration;
        public int count;

        public EffectData(float duration, int count)
        {
            this.duration = duration;
            this.count = count;
        }
    }

    void Awake() => Instance = this;

    void Start() => ClearEffectText();

    void Update()
    {
        foreach (var kvp in activeEffects.ToList())
        {
            kvp.Value.duration -= Time.deltaTime;
            if (kvp.Value.duration <= 0)
            {
                OnEffectExpired(kvp.Key);
                activeEffects.Remove(kvp.Key);
            }
        }

        UpdateEffectText();
    }

    void OnEffectExpired(string effectName)
    {
        if (effectName == "Увеличение платформы" || effectName == "Уменьшение платформы")
        {
            var platform = FindObjectOfType<PlatformController>();
            platform?.ResetPlatform();
        }
    }

    public void ShowEffect(string effectName, float duration)
    {
        string oppositeEffect = GetOppositeEffect(effectName);

        if (!string.IsNullOrEmpty(oppositeEffect) && activeEffects.ContainsKey(oppositeEffect))
        {
            activeEffects.Remove(oppositeEffect);

            if (effectName.Contains("Platform"))
            {
                var platform = FindObjectOfType<PlatformController>();
                platform?.ResetPlatform();
            }
        }

        if (activeEffects.ContainsKey(effectName))
        {
            activeEffects[effectName].duration = duration;
            activeEffects[effectName].count++;
        }
        else
        {
            activeEffects.Add(effectName, new EffectData(duration, 1));
        }

        foreach (var kvp in activeEffects)
        {
            Debug.Log($"  - {kvp.Key}: {kvp.Value.duration:F1}s (x{kvp.Value.count})");
        }

        UpdateEffectText();
    }

    string GetOppositeEffect(string effectName)
    {
        switch (effectName)
        {
            case "Ускорение мяча": return "Замедление мяча";
            case "Замедление мяча": return "Ускорение мяча";
            case "Увеличение платформы": return "Уменьшение платформы";
            case "Уменьшение платформы": return "Увеличение платформы";
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

            string countText = kvp.Value.count > 1 ? $" (x{kvp.Value.count})" : "";
            text += $"{kvp.Key}: {kvp.Value.duration:F1}s{countText}";
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
