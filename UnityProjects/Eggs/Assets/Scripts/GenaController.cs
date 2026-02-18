using UnityEngine;

public class GenaController : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private Transform pointSpawn;
    [SerializeField] private Transform pointLeft;
    [SerializeField] private Transform pointRight;
    [Range(0, 100)] private int chanceToCatch = 60;
    private Vector3 targetPosition;
    private GameObject currentTargetOrange;

    void Start()
    {
        transform.position = pointSpawn.position;
        targetPosition = transform.position;
    }

    void Update()
    {
        FindLowestOrange();

        if (currentTargetOrange != null)
        {
            float xPos = Mathf.Clamp(currentTargetOrange.transform.position.x, pointLeft.position.x, pointRight.position.x);
            targetPosition = new Vector3(xPos, transform.position.y, transform.position.z);
        }

        if (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            Flip(targetPosition.x > transform.position.x);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
    }

    void FindLowestOrange()
    {
        GameObject[] oranges = GameObject.FindGameObjectsWithTag("Orange");
        if (oranges.Length > 0)
        {
            GameObject lowest = oranges[0];
            foreach (GameObject o in oranges)
            {
                if (o.transform.position.y < lowest.transform.position.y)
                    lowest = o;
            }
            currentTargetOrange = lowest;
        }
    }

    public bool TryCatch(GameObject orange)
    {
        if (currentTargetOrange != orange) return false;

        return Random.Range(0, 100) < chanceToCatch;
    }

    public bool IsTargeting(GameObject orange)
    {
        return currentTargetOrange == orange;
    }

    private void Flip(bool lookLeft)
    {
        Vector3 scale = transform.localScale;
        if (lookLeft)
            scale.x = -Mathf.Abs(scale.x);
        else
            scale.x = Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}