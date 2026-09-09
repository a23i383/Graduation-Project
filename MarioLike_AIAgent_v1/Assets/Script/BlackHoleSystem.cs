using System.Collections;
using UnityEngine;

public class BlackHoleSystem : MonoBehaviour
{
    [SerializeField] private float destroyTime = 5.0f;
    [SerializeField] private LayerMask groundLayer;
    void Start()
    {
        StartCoroutine(DestroyTimer());
    }
    private IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyBase enemyBase=collision.GetComponentInParent<EnemyBase>();
        if (enemyBase != null) enemyBase.isBlackHoleStay = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        EnemyBase enemyBase = collision.GetComponentInParent<EnemyBase>();
        if (enemyBase != null)
        {
            enemyBase.isBlackHoleStay = false;
            if (collision.gameObject.GetComponent<Collider2D>().IsTouchingLayers(groundLayer)) enemyBase.isStart = true;
        }
    }
}
