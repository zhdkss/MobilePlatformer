using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Движение")]
    public float moveSpeed = 6f;
    public float jumpForce = 13f;

    [Header("Земля")]
    public Transform groundCheck;
    public float groundRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Input System")]
    public InputActionReference moveAction;   // Player/Move
    public InputActionReference jumpAction;   // Player/Jump
    public AudioClip jumpSound;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;
    float inputX;
    bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
        jumpAction.action.performed += OnJump;   // прыжок по нажатию
    }

    void OnDisable()
    {
        jumpAction.action.performed -= OnJump;
    }

    void Update()
    {
        // Джойстик и клавиатура дают одно и то же действие Move
        inputX = moveAction.action.ReadValue<Vector2>().x;

        // Разворот спрайта по направлению
        if (inputX > 0.01f) sr.flipX = false;
        else if (inputX < -0.01f) sr.flipX = true;

        // Анимации без переходов: просто включаем нужный клип
        if (!isGrounded) anim.Play("Jump");
        else if (Mathf.Abs(inputX) > 0.1f) anim.Play("Run");
        else anim.Play("Idle");
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);
    }

    void OnJump(InputAction.CallbackContext ctx)
    {
        if (isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            AudioSource.PlayClipAtPoint(jumpSound, Camera.main.transform.position);
    }

    public bool FacingLeft => sr.flipX;   // пригодится для стрельбы
}