using UnityEngine;

public class GameScript : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float flipSpeed = 10f;

    private float targetRotationZ = 0f;
    private float targetScaleX;

    public GameScript()
    {
        targetScaleX = 0.261144f;
    }

    void Update()
    {
        // Обработка ввода — задаём цели
        if (Input.GetKeyDown(KeyCode.D))
        {
            targetRotationZ = -12f;
            targetScaleX = -0.261144f;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            targetRotationZ = 12f;
            targetScaleX = -0.261144f;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            targetRotationZ = -12f;
            targetScaleX = 0.261144f;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            targetRotationZ = 12f;
            targetScaleX = 0.261144f;
        }

        Quaternion targetRot = Quaternion.Euler(0, 0, targetRotationZ);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );

        Vector3 currentScale = transform.localScale;
        currentScale.x = Mathf.Lerp(currentScale.x, targetScaleX, flipSpeed * Time.deltaTime);
        transform.localScale = currentScale;
    }
}