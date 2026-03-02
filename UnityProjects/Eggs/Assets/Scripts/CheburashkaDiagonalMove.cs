using System;
using UnityEngine;

public class CheburashkaDiagonalMove : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private Transform pointCenter;
    [SerializeField] private Transform pointTopLeft;
    [SerializeField] private Transform pointBottomLeft;
    [SerializeField] private Transform pointTopRight;
    [SerializeField] private Transform pointBottomRight;
    private Vector3 targetPosition;
    void Start()
    {
        if (pointCenter != null)
        {
            transform.position = pointCenter.position;
            targetPosition = pointCenter.position;
        }
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.Q)) targetPosition = pointTopLeft.position;
        else if (Input.GetKey(KeyCode.E)) targetPosition = pointTopRight.position;
        else if (Input.GetKey(KeyCode.A)) targetPosition = pointBottomLeft.position;
        else if (Input.GetKey(KeyCode.D)) targetPosition = pointBottomRight.position;
        else if (Input.GetKey(KeyCode.S)) targetPosition = pointCenter.position;
        else
        {
            targetPosition = transform.position;
        }
        if (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            Flip(targetPosition.x > transform.position.x);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
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