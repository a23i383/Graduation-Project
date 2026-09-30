using Unity.VisualScripting;
using UnityEngine;

public class AIInputProvider : IInputProvider
{
    public float move = 0.0f;
    private bool jumpHoldPrev = false;
    public bool jumpHold = false;
    public bool dushHold = false;
    public float scroll = 0.0f;
    public Vector3 blockPos = Vector3.zero;
    public bool blockInsDown = false;


    public void ResetInput()
    {
        move = 0.0f;
        jumpHoldPrev = false;
        jumpHold = false;
        dushHold = false;
    }

    public float GetMoveInput()
    {
        return move;
    }
    public bool GetJumpDown()
    {
        if (jumpHold == true && jumpHoldPrev == false)
        {
            jumpHoldPrev = true;
            return true;
        }
        else if (jumpHold == false && jumpHoldPrev == true)
        {
            jumpHoldPrev = false;
            return false;
        }
        else
        {
            return false;
        }
    }
    public bool GetJumpHold()
    {
        return jumpHold;
    }
    public bool GetDushHold()
    {
        return dushHold;
    }
    public float GetMouseScroll()
    {
        return scroll;
    }
    public Vector3 GetBlockPos()
    {
        return blockPos;
    }
    public bool GetBlockInsDown()
    {
        return blockInsDown;
    }
}
