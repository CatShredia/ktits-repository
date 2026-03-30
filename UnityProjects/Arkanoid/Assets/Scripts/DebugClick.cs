using UnityEngine;
using UnityEngine.EventSystems;

public class UIClickDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;

            System.Collections.Generic.List<RaycastResult> results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            Debug.Log($"\n=== Клик в точке {Input.mousePosition} ===");
            foreach (var result in results)
            {
                Debug.Log($"  - {result.gameObject.name} ({result.gameObject.transform.parent?.name ?? "root"})");
            }
        }
    }
}