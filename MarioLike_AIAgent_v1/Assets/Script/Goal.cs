using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Goal:MonoBehaviour
{
    [SerializeField] private float clearDelay = 2.0f;
    [SerializeField] private StageManager stageManager;
    private bool isCleared = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCleared)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            isCleared = true;
            PlayerController player=collision.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                player.canControl = false;
                player.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
            }

            StartCoroutine(ClearSequence());
        }
    }

    private IEnumerator ClearSequence()
    {
        Debug.Log("Clear!");
        stageManager.GoalShowText();
        yield return new WaitForSeconds(clearDelay);
        Destroy(RespawnManager.instance);
        SceneManager.LoadScene("MainMenu");
    }
}
