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
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float speed = new Vector2(horizontal, vertical).magnitude;
        animator.SetFloat("Speed", speed);

        // Анимация прыжка
        bool isJumping = !playerMovement.IsGrounded;
        animator.SetBool("IsJumping", isJumping);
    }
}