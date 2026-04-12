using UnityEngine;

/// <summary>
/// Тестовый префаб уровня — простая платформа с монетами.
/// Используйте для проверки системы динамической загрузки.
/// </summary>
public class TestLevelPrefab : MonoBehaviour
{
    void Start()
    {
        Debug.Log($"TestLevelPrefab: Уровень '{gameObject.name}' активирован");
    }

    void OnDestroy()
    {
        Debug.Log($"TestLevelPrefab: Уровень '{gameObject.name}' уничтожен");
    }
}
