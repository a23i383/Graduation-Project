using UnityEngine;
using System.Collections;

public class PillarOpenCloseSystem : MonoBehaviour
{
    [SerializeField] private SwitchSystem switchSystem;
    [SerializeField] private float duration = 1.0f;

    private SpriteRenderer[] renderers;
    private bool previousSwitch = false;
    private bool isDoing = false;

    private void Start()
    {
        renderers=GetComponentsInChildren<SpriteRenderer>(true);
    }

    void Update()
    {
        if ((switchSystem.isSwitch != previousSwitch) && !isDoing) 
        {
            previousSwitch = switchSystem.isSwitch;
            if (switchSystem.isSwitch) StartCoroutine(Close());
            else StartCoroutine(Open());
        }
    }

    private IEnumerator Close()
    {
        isDoing=true;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float alpha = Mathf.Lerp(1.0f, 0.0f, t);
            SetAlpha(alpha);
            yield return null;
        }
        GetComponent<Rigidbody2D>().simulated = false;
        isDoing = false;
    }

    private IEnumerator Open()
    {
        isDoing = true;
        GetComponent<Rigidbody2D>().simulated = true;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float alpha = Mathf.Lerp(0.0f, 1.0f, t);
            SetAlpha(alpha);
            yield return null;
        }
        isDoing = false;
    }

    private void SetAlpha(float alpha)
    {
        foreach (SpriteRenderer renderer in renderers) 
        {

            Color color = renderer.color;
            color.a = alpha;
            renderer.color = color;
        }
    }
}
