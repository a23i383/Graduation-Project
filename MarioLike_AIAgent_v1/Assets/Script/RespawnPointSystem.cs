using UnityEngine;

public class RespawnPointSystem : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private Sprite offFlag;
    [SerializeField] private AudioClip passFlagSound;
    private void Start()
    {
        if (!RespawnManager.instance.isOnRespawn)
        {
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            GetComponentInChildren<SpriteRenderer>().sprite = offFlag;
            Destroy(this);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !RespawnManager.instance.isOnRespawn)
        {
            GetComponentInChildren<SpriteRenderer>().sprite = offFlag;
            audioSource.PlayOneShot(passFlagSound);
            RespawnManager.instance.spawnPosition = transform.position + Vector3.up;
            RespawnManager.instance.isOnRespawn = true;
        }
    }
}
