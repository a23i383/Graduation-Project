using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Integrations.Match3;

public class CharacterAgent : Agent
{
    private Rigidbody2D rb;
    private PlayerController playerController;
    private AIInputProvider inputProvider;

    private float holizontalMove;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
        inputProvider = new AIInputProvider();
        playerController.SetInputProvider(inputProvider);

        holizontalMove = 0.0f;
    }

    public override void OnEpisodeBegin()
    {
        transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        rb.linearVelocity = Vector2.zero;

        playerController.SetInputProvider(inputProvider);
        inputProvider.ResetInput();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position.x);
        sensor.AddObservation(transform.position.y);
        sensor.AddObservation(rb.linearVelocity.x);
        sensor.AddObservation(rb.linearVelocity.y);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int action = actions.DiscreteActions[0];

        switch(action)
        {
            case 0: //何もしない.
                inputProvider.move = 0.0f;
                inputProvider.jumpHold = false;
                inputProvider.dushHold = false;
                break;
            case 1: //右移動.
                inputProvider.move = 1.0f;
                inputProvider.jumpHold = false;
                inputProvider.dushHold = false;
                break;
            case 2: //左移動.
                inputProvider.move = -1.0f;
                inputProvider.jumpHold = false;
                inputProvider.dushHold = false;
                break;
            case 3: //ジャンプ.
                inputProvider.move = 0.0f;
                inputProvider.jumpHold = true;
                inputProvider.dushHold = false;
                break;
            case 4: //右移動+ダッシュ.
                inputProvider.move = 1.0f;
                inputProvider.jumpHold = false;
                inputProvider.dushHold = true;
                break;
            case 5: //左移動+ダッシュ.
                inputProvider.move = -1.0f;
                inputProvider.jumpHold = false;
                inputProvider.dushHold = true;
                break;
            case 6: //右移動+ジャンプ.
                inputProvider.move = 1.0f;
                inputProvider.jumpHold = true;
                inputProvider.dushHold = false;
                break;
            case 7: //左移動+ジャンプ.
                inputProvider.move = -1.0f;
                inputProvider.jumpHold = true;
                inputProvider.dushHold = false;
                break;
            case 8: //右移動+ジャンプ+ダッシュ.
                inputProvider.move = 1.0f;
                inputProvider.jumpHold = true;
                inputProvider.dushHold = true;
                break;
            case 9: //左移動+ジャンプ+ダッシュ.
                inputProvider.move = -1.0f;
                inputProvider.jumpHold = true;
                inputProvider.dushHold = true;
                break;
        }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;

        holizontalMove = Input.GetAxisRaw("Horizontal");
        bool jumpHold = Input.GetKey(KeyCode.Space);
        bool dushHold = Input.GetKey(KeyCode.LeftShift);

        if (holizontalMove == 0.0f && jumpHold == false && dushHold == false) discreteActions[0] = 0;
        else if (holizontalMove == 1.0f && jumpHold == false && dushHold == false) discreteActions[0] = 1;
        else if (holizontalMove == -1.0f && jumpHold == false && dushHold == false) discreteActions[0] = 2;
        else if (holizontalMove == 0.0f && jumpHold == true && dushHold == false) discreteActions[0] = 3;
        else if (holizontalMove == 1.0f && jumpHold == false && dushHold == true) discreteActions[0] = 4;
        else if (holizontalMove == -1.0f && jumpHold == false && dushHold == true) discreteActions[0] = 5;
        else if (holizontalMove == 1.0f && jumpHold == true && dushHold == false) discreteActions[0] = 6;
        else if (holizontalMove == -1.0f && jumpHold == true && dushHold == false) discreteActions[0] = 7;
        else if (holizontalMove == 1.0f && jumpHold == true && dushHold == true) discreteActions[0] = 8;
        else if (holizontalMove == -1.0f && jumpHold == true && dushHold == true) discreteActions[0] = 9;
    }
}