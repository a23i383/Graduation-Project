using UnityEngine;

public class PlayerStomp:MonoBehaviour
{
    [SerializeField] private float bounceForce=10.0f;
    [SerializeField] private AudioClip stompSE;
    private AudioSource audioSource;
    private Rigidbody2D playerRb;
    private EnemyBase enemyBase;
    private void Start()
    {
        playerRb= GetComponentInParent<Rigidbody2D>();
        audioSource = GetComponentInParent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "EnemyStompDetection")
        {
            audioSource.PlayOneShot(stompSE, 1.0f);
            Vector2 currentVec = playerRb.linearVelocity;

            enemyBase = collision.gameObject.GetComponentInParent<EnemyBase>();
            if (enemyBase != null) StartCoroutine(enemyBase.ThisDestroy());

            playerRb.linearVelocity = new Vector2(currentVec.x, bounceForce);
        }
    }
}
