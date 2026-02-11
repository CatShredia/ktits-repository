using Unity.VisualScripting;
using UnityEngine;

public class GameScript : MonoBehaviour
{

    [SerializeField] private GameObject cheburashka;
    [SerializeField] private GameObject basket;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Vector3 scale = transform.localScale;

            scale.x *= -1f;

            if (scale.x < 0) { transform.localScale = scale; }

            Quaternion targetRotation = Quaternion.Euler(0, 0, -12);
            transform.rotation = targetRotation;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Vector3 scale = transform.localScale;

            scale.x *= -1f;

            if (scale.x < 0) { transform.localScale = scale; }

            Quaternion targetRotation = Quaternion.Euler(0, 0, 12);
            transform.rotation = targetRotation;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            Vector3 scale = transform.localScale;

            scale.x *= -1f;

            if (scale.x > 0) { transform.localScale = scale; }

            Quaternion targetRotation = Quaternion.Euler(0, 0, -12);
            transform.rotation = targetRotation;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Vector3 scale = transform.localScale;

            scale.x *= -1f;

            if (scale.x > 0) { transform.localScale = scale; }

            Quaternion targetRotation = Quaternion.Euler(0, 0, 12);
            transform.rotation = targetRotation;
        }
    }
}
