using Unity.VisualScripting;
using UnityEngine;

public class EnemyDetectionManager : MonoBehaviour
{
    private Collider2D stompCollider;
    private EnemyBase enemyBase;
    private Collider2D thisCollider;
    private void Start()
    {
        enemyBase=GetComponentInParent<EnemyBase>();
        thisCollider= GetComponent<Collider2D>();
        stompCollider = GameObject.FindWithTag("StompCheck").GetComponent<Collider2D>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !stompCollider.bounds.Intersects(thisCollider.bounds)) 
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)   
            {
                player.TakeDamage(enemyBase.damage);
            }
        }
    }
}
