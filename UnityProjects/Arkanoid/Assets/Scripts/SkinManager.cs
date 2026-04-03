using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Центральный менеджер скинов. Хранит текущие экипированные скины
/// и применяет их к объектам в игре (Platform, Ball).
/// Данные скинов сохраняются в PlayerPrefs между сессиями.
/// </summary>
public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance { get; private set; }

    [Header("=== Skin Sprites (название из БД → Sprite) ===")]
    [SerializeField] private SkinSpriteEntry[] skinSprites;

    [Header("=== Name Aliases (DB Name → Sprite Key) ===")]
    [Tooltip("Маппинг имён скинов из БД на ключи спрайтов. Нужно для разрешения старых записей PlayerPrefs.")]
    [SerializeField] private SkinNameAlias[] skinNameAliases;

    private Dictionary<string, Sprite> _skinSpriteLookup = new();

    // Маппинг: DB Name → Sprite Key (для обратной совместимости со старыми PlayerPrefs)
    private Dictionary<string, string> _nameToKeyMapping = new();

    // Текущие экипированные скины (ключ, совпадающий с spriteName в Inspector)
    private string _equippedPlatformSkin;
    private string _equippedBallSkin;

    private const string PLATFORM_SKIN_KEY = "EquippedPlatformSkin";
    private const string BALL_SKIN_KEY = "EquippedBallSkin";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Построить словарь для быстрого поиска спрайтов — ДО Start() других объектов
        _skinSpriteLookup.Clear();
        if (skinSprites != null)
        {
            foreach (var entry in skinSprites)
            {
                if (!string.IsNullOrEmpty(entry.spriteName) && entry.sprite != null)
                {
                    _skinSpriteLookup[entry.spriteName] = entry.sprite;
                    Debug.Log($"[SkinManager] Registered sprite '{entry.spriteName}'");
                }
            }
        }

        // Маппинг из Inspector (статический, загружается сразу)
        if (skinNameAliases != null)
        {
            foreach (var alias in skinNameAliases)
            {
                if (!string.IsNullOrEmpty(alias.dbSkinName) && !string.IsNullOrEmpty(alias.spriteKey))
                {
                    _nameToKeyMapping[alias.dbSkinName] = alias.spriteKey;
                }
            }
        }

        // Загрузить сохранённые скины — ДО Start() SkinApplier
        _equippedPlatformSkin = PlayerPrefs.GetString(PLATFORM_SKIN_KEY, string.Empty);
        _equippedBallSkin = PlayerPrefs.GetString(BALL_SKIN_KEY, string.Empty);

        Debug.Log($"[SkinManager] Awake — Platform skin: '{_equippedPlatformSkin}', Ball skin: '{_equippedBallSkin}'");
        Debug.Log($"[SkinManager] Loaded {_skinSpriteLookup.Count} skin sprites, {_nameToKeyMapping.Count} name aliases");
    }

    void Start()
    {
        Debug.Log("[SkinManager] Start — ready for skin application");
    }

    /// <summary>
    /// Экипировать скин для указанного типа (Platform / Ball).
    /// skinName — имя скина, совпадающее с ключом в skinSprites (из БД).
    /// </summary>
    public void EquipSkin(string skinType, string skinName)
    {
        if (string.IsNullOrEmpty(skinName))
        {
            Debug.LogWarning($"[SkinManager] Empty skin name for type {skinType}");
            return;
        }

        if (skinType == "Platform")
        {
            _equippedPlatformSkin = skinName;
            PlayerPrefs.SetString(PLATFORM_SKIN_KEY, skinName);
            PlayerPrefs.Save();
            Debug.Log($"[SkinManager] Equipped platform skin: {skinName}");
        }
        else if (skinType == "Ball")
        {
            _equippedBallSkin = skinName;
            PlayerPrefs.SetString(BALL_SKIN_KEY, skinName);
            PlayerPrefs.Save();
            Debug.Log($"[SkinManager] Equipped ball skin: {skinName}");
        }
        else
        {
            Debug.LogWarning($"[SkinManager] Unknown skin type: {skinType}");
            return;
        }

        // Применить скин ко всем объектам в сцене
        ApplySkinToAll(skinType, skinName);
    }

    /// <summary>
    /// Применить скин ко всем SkinApplier в сцене указанного типа.
    /// Вызывается при загрузке уровня или после экипировки.
    /// </summary>
    public void ApplySkinToAll(string skinType, string skinName)
    {
        Sprite sprite = GetSpriteForSkin(skinName);
        if (sprite == null)
        {
            Debug.LogWarning($"[SkinManager] Sprite not found for skin '{skinName}'");
            return;
        }

        var appliers = FindObjectsByType<SkinApplier>(FindObjectsSortMode.None);
        foreach (var applier in appliers)
        {
            if (applier.SkinType == skinType)
            {
                applier.ApplySprite(sprite);
            }
        }
    }

    /// <summary>
    /// Применить все сохранённые скины при старте игры.
    /// Вызывать при загрузке основной сцены.
    /// </summary>
    public void ApplyAllEquippedSkins()
    {
        if (!string.IsNullOrEmpty(_equippedPlatformSkin))
        {
            ApplySkinToAll("Platform", _equippedPlatformSkin);
        }
        if (!string.IsNullOrEmpty(_equippedBallSkin))
        {
            ApplySkinToAll("Ball", _equippedBallSkin);
        }
    }

    /// <summary>
    /// Получить текущий экипированный скин для типа.
    /// </summary>
    public string GetEquippedSkin(string skinType)
    {
        return skinType switch
        {
            "Platform" => _equippedPlatformSkin,
            "Ball" => _equippedBallSkin,
            _ => string.Empty
        };
    }

    /// <summary>
    /// Зарегистрировать маппинг DB Name → Sprite Key.
    /// Вызывается из ShopController при загрузке скинов из API.
    /// Позволяет разрешать старые записи в PlayerPrefs (по DB Name).
    /// </summary>
    public void RegisterSkinNameMapping(string dbSkinName, string spriteKey)
    {
        if (!string.IsNullOrEmpty(dbSkinName) && !string.IsNullOrEmpty(spriteKey))
        {
            _nameToKeyMapping[dbSkinName] = spriteKey;
        }
    }

    /// <summary>
    /// Получить Sprite для указанного имени скина.
    /// </summary>
    public Sprite GetSpriteForSkin(string skinName)
    {
        if (string.IsNullOrEmpty(skinName)) return null;

        // Прямой поиск
        if (_skinSpriteLookup.TryGetValue(skinName, out var sprite))
        {
            return sprite;
        }

        // Попытка разрешить через маппинг (DB Name → Sprite Key)
        if (_nameToKeyMapping.TryGetValue(skinName, out var resolvedKey))
        {
            if (_skinSpriteLookup.TryGetValue(resolvedKey, out var resolvedSprite))
            {
                Debug.Log($"[SkinManager] Resolved '{skinName}' → '{resolvedKey}'");
                return resolvedSprite;
            }
        }

        // Diagnostic: show available keys to help debug
        Debug.LogWarning($"[SkinManager] Sprite not found for key '{skinName}'. Available keys: [{string.Join(", ", _skinSpriteLookup.Keys)}]");
        return null;
    }

    /// <summary>
    /// Сбросить скин к дефолтному (убрать экипировку).
    /// </summary>
    public void ResetSkin(string skinType)
    {
        if (skinType == "Platform")
        {
            _equippedPlatformSkin = string.Empty;
            PlayerPrefs.DeleteKey(PLATFORM_SKIN_KEY);
            PlayerPrefs.Save();
        }
        else if (skinType == "Ball")
        {
            _equippedBallSkin = string.Empty;
            PlayerPrefs.DeleteKey(BALL_SKIN_KEY);
            PlayerPrefs.Save();
        }
    }

    [System.Serializable]
    public class SkinSpriteEntry
    {
        [Tooltip("Ключ спрайта (совпадает с PrefabPath из БД), например 'Button-Photoroom_1'")]
        public string spriteName;
        public Sprite sprite;
    }

    [System.Serializable]
    public class SkinNameAlias
    {
        [Tooltip("Имя скина из БД (поле Name), например 'платформа 3'")]
        public string dbSkinName;
        [Tooltip("Ключ спрайта (совпадает с spriteName в skinSprites), например 'Button-Photoroom_1'")]
        public string spriteKey;
    }
}
