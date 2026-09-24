using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Integrations.Match3;

public class CharacterAgent : Agent
{
    private Rigidbody2D rb;
    private PlayerController playerController;
    private Goal goal;
    private PlayerStomp playerStomp;
    private StageManager stageManager;
    private AIInputProvider inputProvider;

    private float holizontalMove;

    private int stepCount = 0;

    [Header("報酬係数")]
    [SerializeField] private float timePenralty = 0.005f;
    [SerializeField] private float actionPenralty = 0.1f;
    [SerializeField] private float progressRewardScale = 0.01f;
    [SerializeField] private float damagePenalty = 0.5f;
    [SerializeField] private float deathPenalty = 1.0f;
    [SerializeField] private float goalReward = 1.0f;
    [SerializeField] private float stompReward = 1.0f;

    private float previousX;
    private int previousAction;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
        playerStomp = playerController.GetComponentInChildren<PlayerStomp>();
        goal = FindFirstObjectByType<Goal>();
        stageManager = FindFirstObjectByType<StageManager>();
        inputProvider = new AIInputProvider();
        playerController.SetInputProvider(inputProvider);

        holizontalMove = 0.0f;

        goal.OnGoalReached += HandleOnGoaled;
        playerController.IsDied += HandleDied;
        playerController.OnDamaged += HandleDamaged;
        playerStomp.isStomp += HandleStomped;
    }

    public override void OnEpisodeBegin()
    {
        stageManager.ResetStage();
        goal.isCleared = false;

        playerController.SetInputProvider(inputProvider);
        inputProvider.ResetInput();

        previousX = transform.position.x;
        previousAction = 0;
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

        //報酬.
        float deltaX = transform.position.x - previousX;
        AddReward(progressRewardScale * deltaX);

        AddReward(-timePenralty);

        //if (action != previousAction) AddReward(-actionPenralty);
        previousAction = action;

        //stepCount++;
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
    private void HandleDamaged(int damage)
    {
        AddReward(-damagePenalty*damage);
    }
    private void HandleDied()
    {
        AddReward(-deathPenalty);
        EndEpisode();
    }
    public void HandleOnGoaled()
    {
        AddReward(goalReward);
        //Debug.Log(stepCount);
        EndEpisode();
    }
    public void HandleStomped()
    {
        AddReward(stompReward);
    }
}