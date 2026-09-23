using UnityEngine;

public class RespawnPointSystem : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private Sprite offFlag;
    [SerializeField] private AudioClip passFlagSound;
    [SerializeField] private StageManager stageManager;
    private void Start()
    {
        if (!stageManager.isOnRespawn)
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
        if (collision.gameObject.CompareTag("Player") && !stageManager.isOnRespawn)
        {
            GetComponentInChildren<SpriteRenderer>().sprite = offFlag;
            audioSource.PlayOneShot(passFlagSound);
            stageManager.spawnPosition = transform.position + Vector3.up;
            stageManager.isOnRespawn = true;
        }
    }
}
