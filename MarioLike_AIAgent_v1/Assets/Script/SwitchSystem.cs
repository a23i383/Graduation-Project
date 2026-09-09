using UnityEngine;

public class SwitchSystem : MonoBehaviour
{
    [HideInInspector] public bool isSwitch = false;
    private Animator anim;
    private AudioSource audioSource;
    private Collider2D myCollider;
    [SerializeField] private AudioClip buttonSE;
    [SerializeField] private LayerMask detectionLayer;

    private void Start()
    {
        anim = GetComponentInParent<Animator>();
        audioSource = GetComponent<AudioSource>();
        myCollider = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //ç≈èâÇ…isSwitchÇ™trueÇ…Ç»Ç¡ÇΩÇ∆Ç´ÇÃÇ›ifÇí ÇÈ
        if (!isSwitch)
        {
            isSwitch = true;
            audioSource.PlayOneShot(buttonSE);
            anim.SetBool("isSwitch", isSwitch);
            Debug.Log("ì¸Ç¡ÇΩÅI");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Collider2D hitCollider = Physics2D.OverlapBox(transform.position, myCollider.bounds.size, 0.0f, detectionLayer);
        if (hitCollider == null)
        {
            isSwitch = false;
            audioSource.PlayOneShot(buttonSE);
            anim.SetBool("isSwitch", isSwitch);
            Debug.Log("Ç≈ÇΩÅI");
        }
    }
}
