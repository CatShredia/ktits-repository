using UnityEngine;

public class PlayerScriptAnimations : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement playerMovement;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        bool isJumping = !playerMovement.IsGrounded;

        // Если прыгаем — не обновляем Speed (чтобы бег не перекрывал прыжок)
        if (!isJumping)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float speed = Mathf.Abs(horizontal);
            animator.SetFloat("Speed", speed);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }

        // Анимация прыжка
        animator.SetBool("IsJumping", isJumping);
    }
}