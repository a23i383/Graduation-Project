using UnityEngine;
using System.Collections;
using JetBrains.Annotations;

public abstract class EnemyBase : MonoBehaviour, IConveyorAddSpeeder
{
    [HideInInspector] public float AddSpeed { get; set; } = 0.0f;
    [SerializeField] protected float moveSpeed = 2.0f;
    [SerializeField] protected float destroyTime = 0.1f;
    public int damage = 1;

    protected Rigidbody2D rb;
    protected Animator anim;
    protected int direction = 1;
    /*
    protected RaycastHit2D hitWall;
    protected RaycastHit2D hitGround;
    */
    protected bool isRight = true;
    protected bool isDie = false;
    [HideInInspector] public bool isStart = false;
    [HideInInspector] public bool isBlackHoleStay = false;

    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    public virtual void Update()
    {
        DoActive();
        if (!DoActive()) return;

        if (isDie) moveSpeed = 0.0f;

        Move();

        FlipCheck();
    }

    public abstract bool DoActive();
    public abstract void Move();
    public abstract void FlipCheck();

    public IEnumerator ThisDestroy()
    {
        anim.SetTrigger("dieEffect");
        isDie = true;
        yield return new WaitForSeconds(destroyTime);
        Destroy(this.gameObject);
    }
    protected void Flip()
    {
        direction *= -1;
        Vector3 scale = transform.localScale;
        scale.x *= -1.0f;
        transform.localScale = scale;
    }
}
