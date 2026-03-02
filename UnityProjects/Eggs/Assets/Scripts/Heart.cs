using UnityEngine;

public class Heart : MonoBehaviour
{
    private bool isBeingRemoved = false;

    public void DisableHeartListener()
    {
        isBeingRemoved = true;
    }

    void OnDestroy()
    {
        if (!isBeingRemoved && HeartsSystem.Instance != null)
        {
            isBeingRemoved = true;
            HeartsSystem.Instance.RemoveHeart(gameObject);
        }
    }
}
