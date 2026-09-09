using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;

public class FallBlockSystem : MonoBehaviour
{
    private Rigidbody2D rb;
    private TextMeshPro countText;
    private bool isFall = false;
    [SerializeField] private float countSpace = 1.0f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        countText = GetComponentInChildren<TextMeshPro>();
        countText.text = "";
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && !isFall)  
        {
            Debug.Log("í Ç¡ÇΩ");
            StartCoroutine(TextCountDown());
            isFall = true;
        }
    }
    private IEnumerator TextCountDown()
    {
        countText.text = "3";
        yield return new WaitForSeconds(countSpace);
        countText.text = "2";
        yield return new WaitForSeconds(countSpace);
        countText.text = "1";
        yield return new WaitForSeconds(countSpace);
        countText.text = "0";

        //Ç±Ç±ÇÕóéÇ∆Ç∑èàóùÇ≈Ç∑
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
