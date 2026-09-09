using UnityEngine;

public class BeltConveyorSystem : MonoBehaviour,IConveyorAddSpeeder
{
    [HideInInspector]public float AddSpeed {  get; set; }
    [SerializeField] private float conveyorSpeed = 3.0f;
    private Animator[] animators;

    private void Start()
    {
        animators = GetComponentsInChildren<Animator>();
        foreach(Animator animator in animators)
        {
            animator.SetFloat("conveyorSpeed", conveyorSpeed);
        }
        conveyorSpeed *= transform.localScale.x;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        Rigidbody2D rb=collision.gameObject.GetComponentInParent<Rigidbody2D>();
        if (rb != null && rb.bodyType == RigidbodyType2D.Dynamic) 
        {
            if (collision.gameObject.GetComponentInParent<IConveyorAddSpeeder>() == null) rb.linearVelocity = new Vector2(conveyorSpeed, 0.0f);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponentInParent<Rigidbody2D>();
        if (rb != null && rb.bodyType == RigidbodyType2D.Dynamic)
        {
            IConveyorAddSpeeder conveyorAddSpeeder = collision.gameObject.GetComponentInParent<IConveyorAddSpeeder>();
            if (conveyorAddSpeeder != null) conveyorAddSpeeder.AddSpeed += conveyorSpeed;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponentInParent<Rigidbody2D>();
        if (rb != null && rb.bodyType == RigidbodyType2D.Dynamic)
        {
            IConveyorAddSpeeder conveyorAddSpeeder = collision.gameObject.GetComponentInParent<IConveyorAddSpeeder>();
            if (conveyorAddSpeeder != null) conveyorAddSpeeder.AddSpeed -= conveyorSpeed;
        }
    }
}
