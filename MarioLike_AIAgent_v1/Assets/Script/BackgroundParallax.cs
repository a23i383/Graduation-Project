using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    [Header("camera")]
    private Transform cameraTransform;
    [SerializeField] private float parallaxEffect = 0.9f;
    private Transform background;

    private Vector3 lastCameraPosition;
    private Vector3 delta;

    private void Start()
    {
        background= GetComponent<Transform>();
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
    }
    private void LateUpdate()
    {
        delta = cameraTransform.position - lastCameraPosition;
        ParallaxEffect(background, parallaxEffect);
        lastCameraPosition = cameraTransform.position;

    }
    private void ParallaxEffect(Transform BG, float PE)
    {
        BG.position += new Vector3(delta.x * PE, 0, 0);
    }
}
