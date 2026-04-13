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

    [Tooltip("Задержка после приземления перед возможностью прыжка (сек)")]
    public float jumpDelay = 0.1f;

    [Tooltip("Разрешить прыжок в воздухе (coyote/buffer)")]
    public bool allowAirJump = false;

    [Header("Step Assist")]
    [Tooltip("Автоматический подброс на ступеньки (0 = выкл)")]
    public float stepAssistForce = 2f;

    private Rigidbody2D rb;
    public bool IsGrounded { get; private set; }

    // Coyote time & jump buffer
    private float coyoteTimer;
    private float jumpBufferTimer;
    private float jumpDelayTimer;
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

        // Jump delay: после приземления ждём перед возможностью прыжка
        if (IsGrounded && !previousGrounded)
        {
            jumpDelayTimer = jumpDelay;
            Debug.Log($"[JumpDelay] Приземление — задержка {jumpDelay}с");
        }
        else if (jumpDelayTimer > 0)
        {
            jumpDelayTimer -= Time.deltaTime;
        }

        // Jump buffer: запоминаем нажатие W
        if (Input.GetKeyDown(KeyCode.W))
        {
            jumpBufferTimer = jumpBufferTime;
            Debug.Log($"[Jump] W нажат — буфер прыжка: {jumpBufferTimer:F2}с, grounded: {IsGrounded}, coyote: {coyoteTimer:F2}с, delay: {jumpDelayTimer:F2}с");
        }
        else if (jumpBufferTimer > 0)
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        // Прыжок: строгая проверка (только на земле) ИЛИ air jump
        bool canJump = IsGrounded && jumpDelayTimer <= 0;
        if (allowAirJump)
        {
            canJump = (coyoteTimer > 0 || jumpBufferTimer > 0 || (IsGrounded && jumpDelayTimer <= 0));
        }

        if (Input.GetKeyDown(KeyCode.W) || (jumpBufferTimer > 0 && (IsGrounded || allowAirJump)))
        {
            if (canJump)
            {
                Debug.Log($"[Jump] ПРЫЖОК! grounded: {IsGrounded}, coyote: {coyoteTimer:F2}с, buffer: {jumpBufferTimer:F2}с, delay: {jumpDelayTimer:F2}с");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpBufferTimer = 0f;
                coyoteTimer = 0f;
            }
            else
            {
                Debug.Log($"[Jump] НЕ МОГУ прыгнуть: grounded: {IsGrounded}, coyote: {coyoteTimer:F2}с, buffer: {jumpBufferTimer:F2}с, delay: {jumpDelayTimer:F2}с");
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
        // Сначала проверяем, является ли впереди полная стена
        Vector3 wallCheckPos = transform.position + new Vector3(moveDir * (forwardCheckDistance + 0.2f), 0f, 0f);
        bool wallAhead = Physics2D.OverlapCircle(wallCheckPos, groundCheckRadius * 0.5f, groudLayer);

        bool isFullWall = false;
        if (wallAhead)
        {
            Vector3 headCheckPos = transform.position + new Vector3(moveDir * (forwardCheckDistance + 0.2f), 0.8f, 0f);
            bool wallAbove = Physics2D.OverlapCircle(headCheckPos, groundCheckRadius * 0.5f, groudLayer);

            if (wallAbove)
            {
                isFullWall = true;
                Debug.Log($"[StepAssist] Full wall detected — climb blocked");
            }
        }

        // Если это полная стена — не помогаем совсем
        if (isFullWall) return;

        // Проверяем, есть ли земля немного впереди и чуть ниже (для небольших ступенек)
        Vector3 checkPos = transform.position + new Vector3(moveDir * forwardCheckDistance, groundCheckOffsetY * 0.5f, 0f);
        bool groundAhead = Physics2D.OverlapCircle(checkPos, groundCheckRadius, groudLayer);

        if (groundAhead && rb.linearVelocity.y <= 0)
        {
            Debug.Log($"[StepAssist] Ground ahead — подброс velY: {rb.linearVelocity.y:F2}");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 1.5f);
        }

        // Подброс на низкую ступеньку
        if (wallAhead && rb.linearVelocity.y <= 0.5f)
        {
            Debug.Log($"[StepAssist] Wall ahead (low) — подброс на ступеньку");
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
