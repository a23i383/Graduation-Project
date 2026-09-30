using UnityEngine;

public interface IInputProvider
{
    float GetMoveInput();
    bool GetJumpDown();
    bool GetJumpHold();
    bool GetDushHold();
    float GetMouseScroll();
    Vector3 GetBlockPos();
    bool GetBlockInsDown();
}
