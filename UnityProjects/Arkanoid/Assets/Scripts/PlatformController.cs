using UnityEngine;

public class PlatformController : MonoBehaviour
{

    public float playerVelocity = 0.5f;
    public float boundary = 10f;
    private Vector3 playerPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerPosition = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition.x += Input.GetAxis("Horizontal") * playerVelocity;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        transform.position = playerPosition;

        if (playerPosition.x < -boundary)
        {
            transform.position = new Vector3(-boundary, transform.position.y, transform.position.z);
        }
        if (playerPosition.x > boundary)
        {
            transform.position = new Vector3(boundary, transform.position.y, transform.position.z);
        }
    }
}
