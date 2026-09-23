using UnityEngine;

public class SlimePatrol : EnemyBase
{
    [SerializeField] private float rayWallDistance = 0.01f;
    [SerializeField] private float rayGroundDistance = 1.65f;
    [SerializeField] private float contactWithoutMyself = 0.65f;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask enemyLayer;

    private RaycastHit2D hitWall;
    private RaycastHit2D hitGround;

    public override bool DoActive()
    {
        if (!isStart || isBlackHoleStay)
        {
            isStart = false;
            return false;
        }
        else return true;
    }

    public override void Move()
    {
        rb.linearVelocity = new Vector2(direction * moveSpeed + AddSpeed, rb.linearVelocity.y);
    }

    public override void FlipCheck()
    {
        Vector3 currentPos = transform.position;
        Vector2 rayWallDirection = direction * Vector2.right;
        Vector2 rayGroundDirection = Vector2.down;

        hitWall = Physics2D.Raycast(currentPos + new Vector3(contactWithoutMyself * direction, 0), rayWallDirection, rayWallDistance, groundLayer | enemyLayer);
        hitGround = Physics2D.Raycast(currentPos + new Vector3(direction * 0.2f, 0.0f), rayGroundDirection, rayGroundDistance, groundLayer);
        Debug.DrawRay(currentPos + new Vector3(contactWithoutMyself * direction, 0), rayWallDirection * rayWallDistance, Color.red);
        Debug.DrawRay(currentPos + new Vector3(direction * 0.2f, 0.0f), rayGroundDirection * rayGroundDistance, Color.red);

        if (hitWall.collider != null) 
        { 
            Flip();
        }

        if (hitGround.collider == null && GetComponentInChildren<IsGroundChecker>().isGround) 
        {
            Flip();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isStart = true;
        }
    }
}
