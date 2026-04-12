using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 7f;
    public LayerMask groudLayer;

    private Rigidbody2D rb;
    public bool IsGrounded { get; private set; }

    // Knockback
    private bool isKnockedBack = false;
    private float knockbackEndTime;

    // Pushing
    private bool isPushing = false;
    private PushableBlock currentPushBlock;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    void Update()
    {
        // Если активен откат — не управляем горизонтально
        if (isKnockedBack && Time.time < knockbackEndTime)
        {
            // Только проверяем землю и разворот, скорость не трогаем
        }
        else if (isPushing)
        {
            // При толкании двигаемся вместе с блоком, WASD заблокирован
            if (!Input.GetKey(KeyCode.E))
            {
                // Отпустили E — перестаём толкать
                if (currentPushBlock != null)
                {
                    currentPushBlock.StopPush();
                }
            }
        }
        else
        {
            if (isKnockedBack)
            {
                isKnockedBack = false;
            }

            float move = Input.GetAxisRaw("Horizontal");
            rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);
        }

        float moveInput = Input.GetAxisRaw("Horizontal");
        if (moveInput > 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        if (moveInput < 0) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        IsGrounded = Physics2D.OverlapCircle(transform.position, 0.4f, groudLayer);

        if (Input.GetKeyDown(KeyCode.W) && IsGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        isKnockedBack = true;
        knockbackEndTime = Time.time + 0.3f; // 0.3 секунды без контроля игрока
        rb.linearVelocity = new Vector2(direction.x * force, 3f);
    }

    public void StartPushing(PushableBlock block)
    {
        isPushing = true;
        currentPushBlock = block;
    }

    public void StopPushing()
    {
        isPushing = false;
        currentPushBlock = null;
    }
}
