using UnityEngine;

public class KeyboardInputProvider : IInputProvider
{
    public float GetMoveInput()
    {
        return Input.GetAxisRaw("Horizontal");
    }
    public bool GetJumpDown()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }
    public bool GetJumpHold()
    {
        return Input.GetKey(KeyCode.Space);
    }
    public bool GetDushHold()
    {
        return Input.GetKey(KeyCode.LeftShift);
    }
}
