using UnityEngine;

public class BasketTopLink : MonoBehaviour
{
    public GameObject ParentObject;

    // Добавляем поле для выбора, кто владеет этой корзиной
    public GameController.Collector collectorType = GameController.Collector.Gena;

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

        float thickness = 1f;
        transform.localScale = Vector3.one;
        transform.SetParent(ParentObject.transform, false);

        Vector2 topLocal2D = parentCollider.offset + Vector2.up * (parentCollider.size.y / 2f + thickness / 2f);
        transform.localPosition = new Vector3(topLocal2D.x, topLocal2D.y, transform.localPosition.z);

        var triggerCollider = GetComponent<BoxCollider2D>();
        if (triggerCollider == null) triggerCollider = gameObject.AddComponent<BoxCollider2D>();

        triggerCollider.isTrigger = true;
        triggerCollider.size = new Vector2(parentCollider.size.x, thickness);
        triggerCollider.offset = Vector2.zero;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Orange"))
        {
            Debug.Log($"Orange entered {collectorType}'s basket!");

            if (GameController.Instance != null)
            {
                // Передаем тип коллектора из поля
                GameController.Instance.OnOrangeCollected(collectorType);
                GameController.Instance.OnOrangeDestroyed();
            }

            Destroy(other.gameObject);
        }
    }
}