using UnityEngine;

public class RotatingPlatformManager:MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 60.0f;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        rb.angularVelocity = rotateSpeed;
    }
}
