using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 7f;
    public LayerMask groudLayer;

    [Header("Ground Check")]
    [Tooltip("Радиус проверки земли")]
    public float groundCheckRadius = 0.3f;

    [Tooltip("Смещение точек проверки относительно центра (X)")]
    public float groundCheckOffsetX = 0.35f;

    [Tooltip("Смещение точек проверки по вертикали (Y)")]
    public float groundCheckOffsetY = -0.4f;

    [Tooltip("Дополнительная проверка впереди (для ступенек)")]
    public float forwardCheckDistance = 0.5f;

    [Header("Coyote Time")]
    [Tooltip("Время после ухода с платформы, когда ещё можно прыгнуть (сек)")]
    public float coyoteTime = 0.15f;

    [Tooltip("Время после нажатия W, когда прыжок сработает при приземлении")]
    public float jumpBufferTime = 0.15f;

    [Header("Step Assist")]
    [Tooltip("Автоматический подброс на ступеньки (0 = выкл)")]
    public float stepAssistForce = 2f;

    private Rigidbody2D rb;
    public bool IsGrounded { get; private set; }

    // Coyote time & jump buffer
    private float coyoteTimer;
    private float jumpBufferTimer;
    private bool wasGroundedLastFrame;

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

        // Проверка земли: центр + лево + право + впереди
        bool previousGrounded = IsGrounded;
        IsGrounded = CheckGrounded();

        // Coyote time: запоминаем когда ушли с земли
        if (IsGrounded)
        {
            coyoteTimer = coyoteTime;
        }
        else if (coyoteTimer > 0)
        {
            coyoteTimer -= Time.deltaTime;
        }

        // Jump buffer: запоминаем нажатие W
        if (Input.GetKeyDown(KeyCode.W))
        {
            jumpBufferTimer = jumpBufferTime;
            Debug.Log($"[Jump] W нажат — буфер прыжка: {jumpBufferTimer:F2}с, grounded: {IsGrounded}, coyote: {coyoteTimer:F2}с");
        }
        else if (jumpBufferTimer > 0)
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        // Прыжок: если есть coyote time ИЛИ jump buffer ИЛИ только что приземлились
        bool canJump = coyoteTimer > 0 || jumpBufferTimer > 0 || (IsGrounded && !previousGrounded);
        if (Input.GetKeyDown(KeyCode.W) || (jumpBufferTimer > 0 && IsGrounded))
        {
            if (canJump)
            {
                Debug.Log($"[Jump] ПРЫЖОК! grounded: {IsGrounded}, coyote: {coyoteTimer:F2}с, buffer: {jumpBufferTimer:F2}с");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpBufferTimer = 0f;
                coyoteTimer = 0f;
            }
            else
            {
                Debug.Log($"[Jump] НЕ МОГУ прыгнуть: grounded: {IsGrounded}, coyote: {coyoteTimer:F2}с, buffer: {jumpBufferTimer:F2}с");
            }
        }

        // Помощь при подъёме на ступеньки
        if (!IsGrounded && moveInput != 0)
        {
            Vector3 checkPos = transform.position + new Vector3(moveInput * forwardCheckDistance, groundCheckOffsetY * 0.5f, 0f);
            bool groundAhead = Physics2D.OverlapCircle(checkPos, groundCheckRadius, groudLayer);

            Vector3 wallCheckPos = transform.position + new Vector3(moveInput * (forwardCheckDistance + 0.2f), 0f, 0f);
            bool wallAhead = Physics2D.OverlapCircle(wallCheckPos, groundCheckRadius * 0.5f, groudLayer);

            Debug.Log($"[StepAssist] grounded: {IsGrounded}, groundAhead: {groundAhead}, wallAhead: {wallAhead}, velY: {rb.linearVelocity.y:F2}");

            TryStepAssist(moveInput);
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

    // --- Ground Check ---
    private bool CheckGrounded()
    {
        Vector3 center = transform.position + new Vector3(0f, groundCheckOffsetY, 0f);

        // Центр
        if (Physics2D.OverlapCircle(center, groundCheckRadius, groudLayer)) return true;

        // Лево и право
        if (Physics2D.OverlapCircle(center + new Vector3(-groundCheckOffsetX, 0f, 0f), groundCheckRadius, groudLayer)) return true;
        if (Physics2D.OverlapCircle(center + new Vector3(groundCheckOffsetX, 0f, 0f), groundCheckRadius, groudLayer)) return true;

        return false;
    }

    // --- Step Assist ---
    private void TryStepAssist(float moveDir)
    {
        // Проверяем, есть ли земля немного впереди и чуть ниже
        Vector3 checkPos = transform.position + new Vector3(moveDir * forwardCheckDistance, groundCheckOffsetY * 0.5f, 0f);
        bool groundAhead = Physics2D.OverlapCircle(checkPos, groundCheckRadius, groudLayer);

        // Если впереди есть земля и игрок чуть в воздухе — небольшой подброс
        if (groundAhead && rb.linearVelocity.y <= 0)
        {
            Debug.Log($"[StepAssist] Ground ahead — подброс velY: {rb.linearVelocity.y:F2}");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 1.5f);
        }

        // Проверяем стенку впереди — если есть, пробуем подняться
        Vector3 wallCheckPos = transform.position + new Vector3(moveDir * (forwardCheckDistance + 0.2f), 0f, 0f);
        bool wallAhead = Physics2D.OverlapCircle(wallCheckPos, groundCheckRadius * 0.5f, groudLayer);

        // Если впереди стена и игрок на земле или почти на земле — подбрасываем вверх
        if (wallAhead && rb.linearVelocity.y <= 0.5f)
        {
            Debug.Log($"[StepAssist] Wall ahead — подброс на ступеньку");
            rb.linearVelocity = new Vector2(moveDir * speed * 0.5f, jumpForce * 0.6f);
        }
    }

    void OnDrawGizmos()
    {
        // Визуализация точек проверки
        Gizmos.color = IsGrounded ? Color.green : Color.red;

        Vector3 center = transform.position + new Vector3(0f, groundCheckOffsetY, 0f);
        Gizmos.DrawWireSphere(center, groundCheckRadius);
        Gizmos.DrawWireSphere(center + new Vector3(-groundCheckOffsetX, 0f, 0f), groundCheckRadius);
        Gizmos.DrawWireSphere(center + new Vector3(groundCheckOffsetX, 0f, 0f), groundCheckRadius);
    }
}
