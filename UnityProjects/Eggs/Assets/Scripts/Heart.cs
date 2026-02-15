using UnityEngine;

public class Heart : MonoBehaviour
{
    void OnDestroy()
    {
        if (HeartsSystem.Instance != null)
        {
            HeartsSystem.Instance.RemoveHeart(gameObject);
        }
    }
}
