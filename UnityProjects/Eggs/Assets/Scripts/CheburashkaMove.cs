using UnityEngine;

public class CheburashkaMove : MonoBehaviour
{
    private Vector3 targetPosition;
    private bool isMoving = false;
    public float speed = 5f;

    void Update()
    {
        // Управление: задаём цель при нажатии
        if (Input.GetKeyDown(KeyCode.D))
        {
            SetTarget(new Vector3(6, -3, 0));
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            SetTarget(new Vector3(5, -2, 0));
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            SetTarget(new Vector3(-5, -2, 0));
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            SetTarget(new Vector3(-6, -3, 0));
        }

        // Движение к цели (если движемся)
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            
            // Остановка при достижении цели
            if (transform.position == targetPosition)
                isMoving = false;
        }
    }

    void SetTarget(Vector3 pos)
    {
        targetPosition = pos;
        isMoving = true;
    }
}
