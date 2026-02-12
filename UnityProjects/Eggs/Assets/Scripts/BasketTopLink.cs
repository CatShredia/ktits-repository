using UnityEngine;

public class BasketTopLink : MonoBehaviour
{
    public GameObject ParentObject;

    void Start()
    {
        if (ParentObject == null)
        {
            Debug.LogWarning("BasketTopLink: ParentObject is not assigned.");
            return;
        }

        var parentCollider = ParentObject.GetComponent<BoxCollider2D>();
        if (parentCollider == null)
        {
            Debug.LogWarning("BasketTopLink: ParentObject has no BoxCollider2D.");
            return;
        }

        // Толщина триггера по вертикали (Y)
        float thickness = 1f;

        // В 2D не меняем localScale, используем collider.size/offset
        transform.localScale = Vector3.one;

        // Сделаем объект дочерним ParentObject, чтобы локальная позиция и offset совпадали
        transform.SetParent(ParentObject.transform, false);

        // Позиция — на верхней грани родителя (локальные координаты относительно родителя)
        // Учтём offset и size из BoxCollider2D
        Vector2 topLocal2D = parentCollider.offset + Vector2.up * (parentCollider.size.y / 2f + thickness / 2f);
        transform.localPosition = new Vector3(topLocal2D.x, topLocal2D.y, transform.localPosition.z);

        // Настрой коллайдер триггера 2D: разместим коллайдер в центре этого объекта (offset = zero)
        var triggerCollider = GetComponent<BoxCollider2D>();
        if (triggerCollider == null) triggerCollider = gameObject.AddComponent<BoxCollider2D>();
        triggerCollider.isTrigger = true;
        triggerCollider.size = new Vector2(parentCollider.size.x, thickness);
        triggerCollider.offset = Vector2.zero;
    }

    void Update()
    {
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("12211221Orange entered the top trigger!");
        if (other.CompareTag("Orange"))
        {
            Debug.Log("Orange entered the top trigger!");
        }
    }
}
