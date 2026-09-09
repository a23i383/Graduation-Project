using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, IConveyorAddSpeeder
{
    [HideInInspector] public float AddSpeed { get; set; } = 0.0f;
    //移動スピードとジャンプの強さ
    [Header("Move&Jump")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float dashMultiplier = 3;
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
    [SerializeField] private float gameOverDelay = 2.0f;
    [SerializeField] private float deedlyHeight = -5.0f;

    [Header("Sound")]
    private AudioSource audioSource;
    [SerializeField] private AudioClip jumpSE;
    [SerializeField] private AudioClip putSE;

    [Header("Other")]
    [SerializeField] private PauseUIManager PauseUI;
    [SerializeField] private ItemSelectSystem itemSelectSystem;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private IsGroundChecker isGroundChecker;
    [SerializeField] private CameraFllow cameraFllow;
    [SerializeField] private IInputProvider inputProvider;

    [SerializeField] public GameObject normalBlock;
    [SerializeField] private GameObject normalDesireBlock;
    [SerializeField] public GameObject fallBlock;
    [SerializeField] private GameObject fallDesireBlock;
    [SerializeField] public GameObject blackHole;
    [SerializeField] private GameObject DesireBlackHole;
    [SerializeField] private TextMeshProUGUI activeBlockQuantity;

    [SerializeField] private LayerMask playerStompLayer;
    [SerializeField] private LayerMask blackHoleLayer;
    [HideInInspector] public int currentHp => hp;
    //[HideInInspector] public bool isGrounded = false;

    [HideInInspector] public bool canControl = true;

    private bool isInvincible = false;
    [HideInInspector] public bool isJump = false;
    private Rigidbody2D rb;
    private Animator anim;
    private float targetMove;
    private Vector3 playerScale;
    private Camera mainCamera;
    private bool stayDesireBlock = false;
    private GameObject desireBlockIns;
    private int currentIndex = 0;

    private float gravity = 3.0f;
    private float sensitivity = 3.0f;
    private float prevMove = 0.0f;

    private enum PlayerState
    {
        Idle,
        Walking,
        Running,
        Jump,
        Invincible,
        Dead
    }
    private PlayerState state;

    private struct BlockSet
    {
        public GameObject block;
        public GameObject desire;
    }
    private List<BlockSet> blocks= new List<BlockSet>();
    [HideInInspector] public List<int> blockQuantity = new List<int>();

    private void Start()
    {
        InitBlocks();

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        state = PlayerState.Idle;
        playerScale = transform.localScale;
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
        inputProvider = new KeyboardInputProvider();
    }

    private void Update()
    {
        if (!canControl)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUI.TogglePause();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (cameraFllow.currentCameraState == CameraFllow.CameraState.MainMode)
            {
                cameraFllow.currentCameraState = CameraFllow.CameraState.RoamMode;
            }
            else if(cameraFllow.currentCameraState == CameraFllow.CameraState.RoamMode)
            {
                cameraFllow.currentCameraState = CameraFllow.CameraState.MainMode;
            }
        }

        targetMove = inputProvider.GetMoveInput();
        if (prevMove * targetMove < 0.0f) prevMove = 0.0f;
        float rate = (targetMove == 0.0f) ? gravity : sensitivity;
        prevMove = Mathf.MoveTowards(prevMove, targetMove, rate * Time.deltaTime);

        anim.SetBool("Invincible", isInvincible);
        anim.SetBool("isJump", !isGroundChecker.isGround);

        //ジャンプバッファのカウント
        jumpBufferTimeCounter = TimeCount(
            Trigger: inputProvider.GetJumpDown(),
            Counter: jumpBufferTimeCounter,
            ConstantTime: jumpBufferTime,
            Timer: Time.deltaTime);

        if (inputProvider is KeyboardInputProvider) CreateBlock();

        FallHole();
    }

    private void FixedUpdate()
    {
        if (!canControl)
        {
            return;
        }

        BetterMove();

        //コヨーテタイムのカウント
        coyoteTimeCounter = TimeCount(
            Trigger: isGroundChecker.isGround,
            Counter: coyoteTimeCounter,
            ConstantTime: coyoteTime,
            Timer: Time.fixedDeltaTime);


        BetterJump();
    }

    private void FallHole()
    {
        if (transform.position.y < deedlyHeight)
        {
            isInvincible = false;
            TakeDamage(hp);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || !canControl)
        {
            return;
        }

        hp = hp < damage ? 0 : hp - damage;
        if (hp <= 0)
        {
            StartCoroutine(Die());
            return;
        }

        StartCoroutine(InvincibleCoroutine());
    }
    private IEnumerator Die()
    {
        stageManager.GameOverShowText();
        canControl = false;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(gameOverDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
    public void BetterMove()
    {
        bool DushHold = inputProvider.GetDushHold();
        rb.linearVelocity = new Vector2(prevMove * moveSpeed * (DushHold ? dashMultiplier : 1) + AddSpeed, rb.linearVelocity.y);
        if (rb.linearVelocity.x > 0)
        {
            transform.localScale = playerScale;
        }
        else if (rb.linearVelocity.x < 0)
        {
            transform.localScale = new Vector3(-playerScale.x, playerScale.y, playerScale.z);
        }
        anim.SetBool("isWalk", rb.linearVelocity.x != 0);

    }
    private void BetterJump()
    {
        if (coyoteTimeCounter > 0 && jumpBufferTimeCounter > 0)
        {
            audioSource.PlayOneShot(jumpSE);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            coyoteTimeCounter = 0;
            jumpBufferTimeCounter = 0;
            isGroundChecker.isGround = false;
            isJump = true;
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

        bool jumpKeyHold = inputProvider.GetJumpHold();
        if (rb.linearVelocity.y > 0 && !jumpKeyHold)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }
    private void CreateBlock()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 intMousePos = new Vector2(Mathf.Floor(mousePos.x), Mathf.Floor(mousePos.y));
        Vector2 cellCentrePos = new Vector2(intMousePos.x + 0.5f, intMousePos.y + 0.5f);

        if (currentIndex != itemSelectSystem.currentIndex)
        {
            currentIndex = itemSelectSystem.currentIndex;

            Destroy(desireBlockIns);

            if (stayDesireBlock) desireBlockIns = blockQuantity[currentIndex] > 0 ? Instantiate(blocks[currentIndex].desire, intMousePos, Quaternion.identity) : null;
        }
        if (Input.GetMouseButtonDown(1)) 
        {
            stayDesireBlock = !stayDesireBlock;
            if (stayDesireBlock)
            {
                desireBlockIns = blockQuantity[currentIndex] > 0 ? Instantiate(blocks[currentIndex].desire, intMousePos, Quaternion.identity) : null;
            }
            else
            {
                Destroy(desireBlockIns);
            }
        }
        if (stayDesireBlock && Physics2D.OverlapBox(cellCentrePos, new Vector2(0.9f, 0.9f), 0.0f, ~(playerStompLayer|blackHoleLayer)) == null)
        {
            if (desireBlockIns != null) desireBlockIns.transform.position = intMousePos;
            if (Input.GetMouseButtonDown(0) && blockQuantity[currentIndex] > 0) 
            {
                audioSource.PlayOneShot(putSE);
                Instantiate(blocks[currentIndex].block, intMousePos, Quaternion.identity);

                if (!RespawnManager.instance.isOnRespawn) RespawnManager.instance.installBlockList.Add(new RespawnManager.installedBlocks { block = blocks[currentIndex].block, rocation = intMousePos });

                if (blockQuantity[currentIndex] > 0)
                {
                    blockQuantity[currentIndex]--;
                    activeBlockQuantity.text = "×" + blockQuantity[currentIndex];
                }
                if(blockQuantity[currentIndex] <= 0)
                {
                    Destroy(desireBlockIns);
                }
            }
        }
    }
    private void InitBlocks()
    {
        blocks.Clear();

        if (RespawnManager.instance.normalBlockQuantity > 0)
        {
            blocks.Add(new BlockSet { block = normalBlock, desire = normalDesireBlock });
            blockQuantity.Add(stageManager.normalBlockQuantity);
        }

        if (RespawnManager.instance.fallBlockQuantity > 0)
        {
            blocks.Add(new BlockSet { block = fallBlock, desire = fallDesireBlock });
            blockQuantity.Add(stageManager.fallBlockQuantity);
        }

        if (RespawnManager.instance.blackHoleQuantity > 0)
        {
            blocks.Add(new BlockSet { block = blackHole, desire = DesireBlackHole });
            blockQuantity.Add(stageManager.blackHoleQuantity);
        }
    }
    public void SetInputProvider(IInputProvider provider)
    {
        inputProvider = provider;
    }
}
