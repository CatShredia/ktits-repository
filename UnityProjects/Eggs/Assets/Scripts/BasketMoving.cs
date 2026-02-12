using UnityEngine;

public class BasketMoving : MonoBehaviour
{
    void Update()
    {
        HandleMirror();
    }

    // Заменяет вращение: по нажатию клавиш объект зеркалится по X (flip)
    void HandleMirror()
    {
        Vector3 scale = transform.localScale;

        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.A))
        {
            Debug.Log("Mirror Left...");
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        else if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.D))
        {
            Debug.Log("Mirror Right...");
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }
}
