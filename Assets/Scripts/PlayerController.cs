using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public PlayerStats stats;               // числа баланса лежат в ассете

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
    float coyoteTimer, jumpBufferTimer;
    Transform platform;                       // движущаяся платформа под ногами
    Vector3 lastPlatformPos;

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
        jumpAction.action.performed += OnJumpPressed;
        jumpAction.action.canceled += OnJumpReleased;
    }

    void OnDisable()
    {
        jumpAction.action.performed -= OnJumpPressed;
        jumpAction.action.canceled -= OnJumpReleased;
    }

    void Update()
    {
        // Джойстик и клавиатура дают одно и то же действие Move.
        // На телефоне трудно держать стик ровно, поэтому: мёртвая зона, а дальше сразу полная скорость
        float raw = moveAction.action.ReadValue<Vector2>().x;
        inputX = Mathf.Abs(raw) < stats.stickDeadZone ? 0f : Mathf.Sign(raw);

        // Разворот спрайта по направлению
        if (inputX > 0.01f) sr.flipX = false;
        else if (inputX < -0.01f) sr.flipX = true;

        // Прыжок: земля "помнится" coyoteTime секунд, нажатие — jumpBufferTime секунд
        coyoteTimer = isGrounded ? stats.coyoteTime : coyoteTimer - Time.deltaTime;
        jumpBufferTimer -= Time.deltaTime;
        if (jumpBufferTimer > 0f && coyoteTimer > 0f) Jump();

        // Анимации без переходов: просто включаем нужный клип
        if (!isGrounded) anim.Play("Jump");
        else if (Mathf.Abs(inputX) > 0.1f) anim.Play("Run");
        else anim.Play("Idle");
    }

    void FixedUpdate()
    {
        Collider2D ground = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        isGrounded = ground != null;

        // Стоим на движущейся платформе (кинематическое тело) — едем вместе с ней
        Transform current = null;
        if (ground != null && ground.attachedRigidbody != null &&
            ground.attachedRigidbody.bodyType == RigidbodyType2D.Kinematic)
            current = ground.transform;

        if (current != null && current == platform)
            rb.position += (Vector2)(current.position - lastPlatformPos);

        platform = current;
        if (current != null) lastPlatformPos = current.position;

        rb.linearVelocity = new Vector2(inputX * stats.moveSpeed, rb.linearVelocity.y);
    }

    void OnJumpPressed(InputAction.CallbackContext ctx)
    {
        jumpBufferTimer = stats.jumpBufferTime;     // сам прыжок случится в Update
    }

    void OnJumpReleased(InputAction.CallbackContext ctx)
    {
        // отпустили кнопку, пока летим вверх — срезаем высоту
        if (rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * stats.jumpCutMultiplier);
    }

    void Jump()
    {
        jumpBufferTimer = 0f;
        coyoteTimer = 0f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, stats.jumpForce);
        if (jumpSound) AudioSource.PlayClipAtPoint(jumpSound, Camera.main.transform.position);
    }

    public bool FacingLeft => sr.flipX;   // пригодится для стрельбы
}
