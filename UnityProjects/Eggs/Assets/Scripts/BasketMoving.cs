using UnityEngine;

public class BasketMoving : MonoBehaviour
{
    [Header("Assign the Basket Transform (only this will be moved)")]
    [SerializeField] private Transform basket;
    [Tooltip("Assign the player's root Transform here to mirror the whole player. If empty, the basket will be mirrored.")]
    [SerializeField] private Transform playerToMirror;

    [Header("Per-key target positions (X,Y)")]
    [SerializeField] private Vector2 positionQ = new Vector2(-2f, 0f);
    [SerializeField] private Vector2 positionA = new Vector2(-1f, 0f);
    [SerializeField] private Vector2 positionE = new Vector2(1f, 0f);
    [SerializeField] private Vector2 positionD = new Vector2(2f, 0f);
    [Space]
    [Tooltip("If true, positions are applied to basket.localPosition. If false, positions are world space (basket.position).")]
    [SerializeField] private bool useLocalPosition = true;

    void Update()
    {
        if (basket == null && playerToMirror == null)
        {
            Debug.LogWarning("BasketMoving: neither 'basket' nor 'playerToMirror' is assigned — nothing will be moved.");
            return;
        }

        HandleMirror();
        HandlePositionInput();
    }

    // Заменяет вращение: по нажатию клавиш объект зеркалится по X (flip)
    void HandleMirror()
    {
        Transform target = playerToMirror != null ? playerToMirror : basket;
        if (target == null) return;

        Vector3 scale = target.localScale;

        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.A))
        {
            scale.x = Mathf.Abs(scale.x);
            target.localScale = scale;
        }
        else if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.D))
        {
            scale.x = -Mathf.Abs(scale.x);
            target.localScale = scale;
        }
    }

    // Каждая кнопка задаёт конкретную позицию корзины (по X и Y), Z сохраняется
    void HandlePositionInput()
    {
        if (basket == null) return; // positions apply only to the basket

        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetPosition(positionQ);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            SetPosition(positionA);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            SetPosition(positionE);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            SetPosition(positionD);
        }
    }

    void SetPosition(Vector2 pos)
    {
        if (useLocalPosition)
        {
            basket.localPosition = new Vector3(pos.x, pos.y, basket.localPosition.z);
        }
        else
        {
            basket.position = new Vector3(pos.x, pos.y, basket.position.z);
        }
    }
}
