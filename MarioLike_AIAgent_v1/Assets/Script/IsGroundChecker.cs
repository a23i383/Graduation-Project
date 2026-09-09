using UnityEngine;

public class IsGroundChecker : MonoBehaviour
{
    [HideInInspector] public bool isGround = false;
    private bool crossRecognition = false;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !crossRecognition) 
        {
            isGround = true;
            crossRecognition = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = false;
            crossRecognition = false;
        }
    }
    
}
