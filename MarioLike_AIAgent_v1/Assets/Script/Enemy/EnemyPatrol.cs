using UnityEngine;
using System.Collections;

public class EnemyPatrol : MonoBehaviour,IConveyorAddSpeeder
{
    [HideInInspector] public float AddSpeed { get; set; } = 0.0f;
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float moveDistance = 3.0f;
    [SerializeField] private float destroyTime = 0.1f;
    [SerializeField] private float rayWallDistance = 0.8f;
    [SerializeField] private float rayGroundDistance = 1.5f;
    [SerializeField] private float contactWithoutMyself = 1.0f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask enemyLayer;
    public int damage = 1;

    private Rigidbody2D rb;
    private Animator anim;
    private float startX;
    private int direction = 1;

    private bool isRight = true;
    private bool isDie = false;
    [HideInInspector] public bool isStart = false;
    [HideInInspector] public bool isBlackHoleStay = false;

    private void Start()
    {
        startX = transform.position.x;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (!isStart || isBlackHoleStay)
        {
            isStart = false;
            return;
        }
        if (isDie) moveSpeed = 0.0f;
        Vector3 currentPos = transform.position;
        rb.linearVelocity = new Vector2(direction * moveSpeed + AddSpeed, rb.linearVelocity.y);
        Vector2 rayWallDirection = rb.linearVelocity.normalized;
        Vector2 rayGroundDirection = Vector2.down;
        RaycastHit2D hitWall = Physics2D.Raycast(currentPos + new Vector3(contactWithoutMyself * direction, 0), rayWallDirection, rayWallDistance, groundLayer | enemyLayer);
        RaycastHit2D hitGround = Physics2D.Raycast(currentPos + new Vector3(direction * 0.5f, 0.0f), rayGroundDirection, rayGroundDistance, groundLayer);
        Debug.DrawRay(currentPos + new Vector3(contactWithoutMyself, 0), rayWallDirection * rayWallDistance, Color.red);
        Debug.DrawRay(currentPos + new Vector3(direction * 0.5f, 0.0f), rayGroundDirection * rayGroundDistance, Color.red);

        if (hitWall.collider!=null) Flip();

        if(hitGround.collider == null)Flip();
    }

    public IEnumerator ThisDestroy()
    {
        anim.SetTrigger("dieEffect");
        isDie = true;
        yield return new WaitForSeconds(destroyTime);
        Destroy(this.gameObject);
    }
    private void Flip()
    {
        direction *= -1;
        Vector3 scale = transform.localScale;
        scale.x *= -1.0f;
        transform.localScale = scale;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground") 
        {
            isStart = true;
        }
    }
}
