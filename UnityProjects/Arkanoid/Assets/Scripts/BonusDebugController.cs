
// GameManager
using UnityEngine;
using System.Reflection;

public class BonusDebugController : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 1.3f;
    [SerializeField] private float platformExpandAmount = 0.5f;

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                Debug.Log("Debug: Ball Speed Up (Ctrl+F1)");
                ApplyBallSpeedChange(speedMultiplier);
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                Debug.Log("Debug: Ball Speed Down (Ctrl+F2)");
                ApplyBallSpeedChange(1f / speedMultiplier);
            }
            else if (Input.GetKeyDown(KeyCode.F3))
            {
                Debug.Log("Debug: Platform Expand (Ctrl+F3)");
                ApplyPlatformChange(true);
            }
            else if (Input.GetKeyDown(KeyCode.F4))
            {
                Debug.Log("Debug: Platform Shrink (Ctrl+F4)");
                ApplyPlatformChange(false);
            }
            else if (Input.GetKeyDown(KeyCode.F5))
            {
                Debug.Log("Debug: Load Previous Level (Ctrl+F5)");
                LoadPreviousLevel();
            }
            else if (Input.GetKeyDown(KeyCode.F6))
            {
                Debug.Log("Debug: Load Next Level (Ctrl+F6)");
                LoadNextLevel();
            }
            else if (Input.GetKeyDown(KeyCode.F7))
            {
                Debug.Log("Debug: Reload Current Level (Ctrl+F7)");
                ReloadCurrentLevel();
            }
        }
    }

    void LoadNextLevel()
    {
        LevelController.Instance?.LoadNextLevel();
    }

    void LoadPreviousLevel()
    {
        if (LevelController.Instance == null) return;

        int currentIndex = LevelController.Instance.GetCurrentLevelIndex();
        int levelCount = GetLevelCount();
        int previousIndex = (currentIndex - 1 + levelCount) % levelCount;
        LevelController.Instance?.LoadLevel(previousIndex);
    }

    int GetLevelCount()
    {
        if (LevelController.Instance == null) return 0;

        var levelControllerType = typeof(LevelController);
        var field = levelControllerType.GetField("levelPrefabs", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var prefabs = field?.GetValue(LevelController.Instance) as GameObject[];
        return prefabs?.Length ?? 0;
    }

    void ReloadCurrentLevel()
    {
        int currentIndex = LevelController.Instance?.GetCurrentLevelIndex() ?? 0;
        LevelController.Instance?.LoadLevel(currentIndex);
    }

    void ApplyBallSpeedChange(float multiplier)
    {
        var balls = FindObjectsOfType<BallController>();
        foreach (var ball in balls)
        {
            ball?.ChangeSpeed(multiplier);
        }
        Debug.Log($"[Debug] Ball speed changed by x{multiplier:F2}");
        BonusUIManager.Instance?.ShowEffect(multiplier > 1 ? "Ускорение мяча" : "Замедление мяча", 10f);
    }

    void ApplyPlatformChange(bool expand)
    {
        var platform = FindObjectOfType<PlatformController>();
        if (platform != null)
        {
            if (expand)
            {
                platform.ExpandPlatform(platformExpandAmount);
                Debug.Log($"[Debug] Platform expanded (+{platformExpandAmount})");
                BonusUIManager.Instance?.ShowEffect("Увеличение платформы", 10f);
            }
            else
            {
                platform.ShrinkPlatform(platformExpandAmount);
                Debug.Log($"[Debug] Platform shrunk (-{platformExpandAmount})");
                BonusUIManager.Instance?.ShowEffect("Уменьшение платформы", 10f);
            }
        }
    }
}