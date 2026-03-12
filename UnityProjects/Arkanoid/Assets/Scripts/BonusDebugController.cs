
// Attach to: GameManager or any persistent GameObject in scene
// Only ONE instance should exist in the scene
using UnityEngine;

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
        }
    }

    void ApplyBallSpeedChange(float multiplier)
    {
        var balls = FindObjectsOfType<BallController>();
        foreach (var ball in balls)
            ball?.ChangeSpeed(multiplier);
        Debug.Log($"[Debug] Ball speed changed by x{multiplier:F2}");
        BonusUIManager.Instance?.ShowEffect(multiplier > 1 ? "Ball Speed Up" : "Ball Speed Down", 10f);
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
                BonusUIManager.Instance?.ShowEffect("Platform Expand", 10f);
            }
            else
            {
                platform.ShrinkPlatform(platformExpandAmount);
                Debug.Log($"[Debug] Platform shrunk (-{platformExpandAmount})");
                BonusUIManager.Instance?.ShowEffect("Platform Shrink", 10f);
            }
        }
    }
}