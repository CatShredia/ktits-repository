using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5f;

    private Camera cam;
    private float halfPlayerWidth;
    private float halfPlayerHeight;

    void Start()
    {
        cam = Camera.main;
        // Если у игрока SpriteRenderer:
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            halfPlayerWidth = sr.bounds.extents.x;
            halfPlayerHeight = sr.bounds.extents.y;
        }
    }

    void Update()
    {
        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        transform.Translate(input * speed * Time.deltaTime);
    }

    void LateUpdate()
    {
        // Ограничение по границам камеры
        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * cam.aspect;

        Vector3 pos = transform.position;

        // Учитываем размер игрока
        pos.x = Mathf.Clamp(pos.x, -horzExtent + halfPlayerWidth, horzExtent - halfPlayerWidth);
        pos.y = Mathf.Clamp(pos.y, -vertExtent + halfPlayerHeight, vertExtent - halfPlayerHeight);

        transform.position = pos;
    }
}