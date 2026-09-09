using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class CameraFllow:MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform mainCameraTrs;
    [SerializeField] private Transform roamCameraTrs;
    [SerializeField] private Rigidbody2D targetRb;
    [SerializeField] private float moveSpeed = 50.0f;
    [SerializeField] private float dashMultiplier = 3.0f;

    [Header("Dead Zone")]
    [SerializeField] private float deadZoneWidth=1.2f;
    [SerializeField] private float deadZoneHeight = 1.2f;

    [Header("Look Ahead")]
    [SerializeField] private float lookAheadDistance = 3.0f;
    [SerializeField] private float lookAheadSmoothing = 3.0f;

    [Header("Smooth Damp")]
    [SerializeField] private float smoothTime = 0.2f;

    [Header("Clamp")]
    [SerializeField] private float maxX = 20.0f;
    [SerializeField] private float minX = 0.0f;
    [SerializeField] private float maxY = 5.0f;
    [SerializeField] private float minY = 0.0f;

    [Header("Zoom")]
    [SerializeField] private float beforeZoomSize = 7.0f;
    [SerializeField] private float afterZoomSize = 9.0f;
    [SerializeField] private float duration = 0.1f;

    [Header("Other")]
    [SerializeField] private float offsetY = 3.0f;

    private float currentLookAheadX = 0.0f;
    private Vector3 velocity;
    private Camera roamCamera;
    private Coroutine currentCoroutine;

    [HideInInspector] public enum CameraState
    {
        MainMode,
        RoamMode,
    }
    [HideInInspector] public CameraState currentCameraState;

    private void Start()
    {
        //transform.position = target.position;
        mainCameraTrs.position = target.position;
        currentCameraState = CameraState.MainMode;
        roamCamera=roamCameraTrs.GetComponent<Camera>();
        mainCameraTrs.gameObject.GetComponent<Camera>().enabled = true;
        roamCameraTrs.gameObject.GetComponent<Camera>().enabled = false;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        if(currentCameraState == CameraState.MainMode)
        {
            mainCameraTrs.gameObject.GetComponent<Camera>().enabled = true;
            roamCameraTrs.gameObject.GetComponent<Camera>().enabled = false;
            if (currentCoroutine != null)
            {
                currentCoroutine = null;
                Time.timeScale = 1.0f;
            }
            MainCameraSystem();
        }
        else if (currentCameraState == CameraState.RoamMode)
        {
            if (currentCoroutine == null)
            {
                currentCoroutine = StartCoroutine(RoamCameraSystem());
            }
        }

    }
    
    private void MainCameraSystem()
    {
        //Vector3 currentPos = transform.position;
        Vector3 currentPos = mainCameraTrs.position;
        Vector3 targetPos = target.position + Vector3.up * offsetY;

        float desiredX = currentPos.x;
        float desiredY = currentPos.y;

        float targetLookAhead = 0.0f;

        if (Mathf.Abs(targetRb.linearVelocity.x) > 0.1f)
        {
            targetLookAhead = Mathf.Sign(targetRb.linearVelocity.x) * lookAheadDistance;
        }

        currentLookAheadX = Mathf.Lerp(
            currentLookAheadX,
            targetLookAhead,
            lookAheadSmoothing * Time.deltaTime);



        float deltaX = (targetPos.x + currentLookAheadX) - currentPos.x;
        float deltaY = targetPos.y - currentPos.y;

        if (Mathf.Abs(deltaX) > deadZoneWidth)
        {
            desiredX = deltaX > 0.0f ?
                (targetPos.x + currentLookAheadX) - deadZoneWidth :
                (targetPos.x + currentLookAheadX) + deadZoneWidth;

        }
        if (Mathf.Abs(deltaY) > deadZoneHeight)
        {
            desiredY = deltaY > 0.0f ?
                targetPos.y - deadZoneHeight :
                targetPos.y + deadZoneHeight;
        }

        Vector3 desiredPosition = new Vector3(desiredX, desiredY, -15.0f);

        Vector3 smoothedPos = Vector3.SmoothDamp(
        currentPos,
        desiredPosition,
        ref velocity,
        smoothTime * Time.deltaTime);

        smoothedPos.x = Mathf.Clamp(smoothedPos.x, minX, maxX);
        smoothedPos.y = Mathf.Clamp(smoothedPos.y, minY, maxY);

        //transform.position = smoothedPos;
        mainCameraTrs.position = smoothedPos;
    }

    private IEnumerator RoamCameraSystem()
    {
        Time.timeScale = 0.0f;
        mainCameraTrs.gameObject.GetComponent<Camera>().enabled = false;
        roamCameraTrs.gameObject.GetComponent<Camera>().enabled = true;
        roamCameraTrs.position = mainCameraTrs.position;
        float elapsed = 0.0f;
        yield return null;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            roamCamera.orthographicSize = Mathf.Lerp(beforeZoomSize, afterZoomSize, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }
        while (true)
        {
            float horizontalValue= Input.GetAxisRaw("Horizontal");
            float verticalValue = Input.GetAxisRaw("Vertical");

            Vector3 newRoamCameraPos = roamCameraTrs.position + new Vector3(
                horizontalValue * Time.unscaledDeltaTime * moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? dashMultiplier : 1),
                verticalValue * Time.unscaledDeltaTime * moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? dashMultiplier : 1), 0);

            newRoamCameraPos.x = Mathf.Clamp(newRoamCameraPos.x, minX, maxX);
            newRoamCameraPos.y = Mathf.Clamp(newRoamCameraPos.y, minY, maxY);

            roamCameraTrs.position = newRoamCameraPos;
            yield return null;
        }
    }
}
