using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController_v2 : MonoBehaviour
{
    //移動スピードとジャンプの強さ
    [Header("Move&Jump")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    //落下が早くなる、ジャンプが低くなる
    [Header("Jump special settings")]
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    //コヨーテタイムの変数
    [SerializeField] private float coyoteTime = 0.1f;
    private float coyoteTimeCounter;

    //ジャンプバッファの変数
    [SerializeField] private float jumpBufferTime = 0.1f;
    private float jumpBufferTimeCounter;

    [Header("HP&Damage motion")]
    [SerializeField] private float invincibleDuration = 2.0f;
    [SerializeField] private int hp = 3;

    [Header("Other")]
    [SerializeField] private PauseUIManager PauseUI;
    public int currentHp => hp;
    public bool isGrounded = false;

    public bool canControl = true;

    private bool isInvincible = false;
    private Rigidbody2D rb;
    private Animator anim;
    private float move;
    private Camera mainCamera;

    public enum PlayerState
    {
        Idle,
        Walking,
        Running,
        Jump,
        Invincible,
        Dead
    }
    public PlayerState state;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        state = PlayerState.Idle;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        SetState();
        if (!canControl)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUI.TogglePause();
        }

        move = Input.GetAxis("Horizontal");

        anim.SetBool("Invincible", isInvincible);

        //ジャンプバッファのカウント
        jumpBufferTimeCounter = TimeCount(
            Trigger: Input.GetKeyDown(KeyCode.Space),
            Counter: jumpBufferTimeCounter,
            ConstantTime: jumpBufferTime,
            Timer: Time.deltaTime);

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        SetState();
        if (!canControl)
        {
            return;
        }

        BetterMove();

        //コヨーテタイムのカウント
        coyoteTimeCounter = TimeCount(
            Trigger: isGrounded,
            Counter: coyoteTimeCounter,
            ConstantTime: coyoteTime,
            Timer: Time.fixedDeltaTime);

        BetterJump();
    }


    public void TakeDamage(int damage)
    {
        Debug.Log("TakeDamage");
        if (isInvincible || !canControl)
        {
            return;
        }

        hp = hp < damage ? 0 : hp - damage;
        Debug.Log("HP:" + hp);
        if (hp <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(InvincibleCoroutine());
    }
    private void Die()
    {
        Debug.Log("Game Over...");
        canControl = false;
    }
    private IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }
    private float TimeCount(bool Trigger, float Counter, float ConstantTime, float Timer)
    {
        if (Trigger)
        {
            Counter = ConstantTime;
        }
        else
        {
            Counter -= Timer;
        }
        return Counter;
    }
    private void BetterMove()
    {
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);
    }
    private void BetterJump()
    {
        if (coyoteTimeCounter > 0 && jumpBufferTimeCounter > 0)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            coyoteTimeCounter = 0;
            jumpBufferTimeCounter = 0;
            isGrounded = false;
        }

        //落下速度の制御
        if (rb.linearVelocity.y < -20)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -20);
        }
        else if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }

        if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }
    private void SetState()
    {

    }
}
