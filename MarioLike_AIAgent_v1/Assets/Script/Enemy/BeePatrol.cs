using UnityEngine;

public class BeePatrol : EnemyBase
{
    [SerializeField] private float moveSpeedX = 2.0f;
    [SerializeField] private float moveSpeedY = 5.0f;
    [SerializeField] private float waveHeight = 2.0f;
    [SerializeField] private float moveDistance = 5.0f;

    [SerializeField] private float rayWallDistance = 0.01f;
    [SerializeField] private float contactWithoutMyself = 0.65f;

    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask enemyLayer;

    private float startX;

    public override void Start()
    {
        startX = transform.position.x;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    public override bool DoActive()
    {
        if (isBlackHoleStay) return false;
        else return true;
    }

    public override void Move()
    {
        float sinY = Mathf.Sin(Time.time * moveSpeedY);
        rb.linearVelocity = new Vector2(direction * moveSpeedX, sinY * waveHeight);
    }

    public override void FlipCheck()
    {
        Vector3 currentPos = transform.position;

        Vector2 rayWallDirection = rb.linearVelocity.normalized;

        RaycastHit2D hitWall = Physics2D.Raycast(currentPos + new Vector3(contactWithoutMyself * direction, 0), rayWallDirection, rayWallDistance, wallLayer | enemyLayer);
        Debug.DrawRay(currentPos + new Vector3(contactWithoutMyself * direction, 0), rayWallDirection * rayWallDistance, Color.red);

        if (hitWall.collider != null) Flip();
        if ((currentPos.x - startX) * direction > moveDistance) Flip();
    }
}
