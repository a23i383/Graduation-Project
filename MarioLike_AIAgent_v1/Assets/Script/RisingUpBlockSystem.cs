using UnityEngine;

public class RisingUpBlockSystem : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isRise = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && !isRise)
        {
            RisingUp();
            isRise = true;
        }
    }
    private void RisingUp()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = -1.0f;
        rb.mass = 7.5f;
    }
}
