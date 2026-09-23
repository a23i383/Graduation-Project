using UnityEngine;
using System.Collections;

public class BeePatrolTmp : MonoBehaviour
{
    [SerializeField] private float moveSpeedX = 2.0f;
    [SerializeField] private float moveSpeedY = 5.0f;
    [SerializeField] private float waveHeight = 2.0f;
    [SerializeField] private float moveDistance = 3.0f;
    [SerializeField] private float destroyTime = 0.1f;
    [SerializeField] private float rayWallDistance = 0.8f;
    [SerializeField] private float contactWithoutMyself = 1.0f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask wallLayer;
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
        if (isDie) moveSpeedX = 0.0f;
        Vector3 currentPos = transform.position;
        float sinY = Mathf.Sin(Time.time * moveSpeedY);

        rb.linearVelocity = new Vector2(direction * moveSpeedX, sinY * waveHeight);
        Vector2 rayWallDirection = rb.linearVelocity.normalized;

        RaycastHit2D hitWall = Physics2D.Raycast(currentPos + new Vector3(contactWithoutMyself * direction, 0), rayWallDirection, rayWallDistance, wallLayer | enemyLayer);
        Debug.DrawRay(currentPos + new Vector3(contactWithoutMyself * direction, 0), rayWallDirection * rayWallDistance, Color.red);

        if (hitWall.collider != null) Flip();
        if ((currentPos.x - startX) * direction > moveDistance) Flip();
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
}
