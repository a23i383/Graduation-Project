using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Integrations.Match3;
using Unity.Jobs.LowLevel.Unsafe;

public class CharacterAgent : Agent
{
    private Rigidbody2D rb;
    private PlayerController playerController;
    private ItemSelectSystem itemSelectSystem;
    private Goal goal;
    private PlayerStomp playerStomp;
    private StageManager stageManager;
    private AIInputProvider inputProvider;

    private float holizontalMove;

    private int stepCount = 0;

    [Header("ïÒèVåWêî")]
    [SerializeField] private float timePenralty = 0.005f;
    [SerializeField] private float actionPenralty = 0.1f;
    [SerializeField] private float progressRewardScale = 0.01f;
    [SerializeField] private float damagePenalty = 0.5f;
    [SerializeField] private float deathPenalty = 1.0f;
    [SerializeField] private float goalReward = 1.0f;
    [SerializeField] private float stompReward = 1.0f;
    [SerializeField] private float blockInsCost = 0.1f;

    private float previousX;
    private int previousAction;

    [HideInInspector] public int blockInsResult = 0;
    [HideInInspector] public int currentBlockQuantity;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
        playerStomp = playerController.GetComponentInChildren<PlayerStomp>();

        itemSelectSystem = FindFirstObjectByType<ItemSelectSystem>();
        goal = FindFirstObjectByType<Goal>();
        stageManager = FindFirstObjectByType<StageManager>();
        inputProvider = new AIInputProvider();
        playerController.SetInputProvider(inputProvider);
        itemSelectSystem.SetInputProvider(inputProvider);

        holizontalMove = 0.0f;

        goal.OnGoalReached += HandleOnGoaled;
        playerController.IsDied += HandleDied;
        playerController.OnDamaged += HandleDamaged;
        playerStomp.isStomp += HandleStomped;
        playerController.BlockInsed += HandleBlockIns;
    }

    public override void OnEpisodeBegin()
    {
        stageManager.ResetStage();
        goal.isCleared = false;

        playerController.SetInputProvider(inputProvider);
        itemSelectSystem.SetInputProvider(inputProvider);
        inputProvider.ResetInput();

        previousX = transform.position.x;
        previousAction = 0;
        currentBlockQuantity = playerController.blockQuantity[0];
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position.x);
        sensor.AddObservation(transform.position.y);
        sensor.AddObservation(rb.linearVelocity.x);
        sensor.AddObservation(rb.linearVelocity.y);
        sensor.AddOneHotObservation(playerController.currentIndex, 1);
        sensor.AddObservation(playerController.blockQuantity.ConvertAll(bq => (float)bq));
        sensor.AddOneHotObservation(blockInsResult, 3);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int moveAction = actions.DiscreteActions[0];
        int dashAction = actions.DiscreteActions[1];
        int jumpAction = actions.DiscreteActions[2];
        int blockSelect = actions.DiscreteActions[3];
        int blockInst = actions.DiscreteActions[4];
        int offsetX = actions.DiscreteActions[5];
        int offsetY = actions.DiscreteActions[6];

        switch (moveAction)
        {
            case 0: inputProvider.move = 0.0f;  break;
            case 1: inputProvider.move = 1.0f;  break;
            case 2: inputProvider.move = -1.0f; break;
        }

        inputProvider.dushHold = (dashAction == 1);

        inputProvider.jumpHold = (jumpAction == 1);

        switch (blockSelect)
        {
            case 0: inputProvider.scroll = 0.0f; break;
            case 1: inputProvider.scroll = 1.0f; break;
            case 2: inputProvider.scroll = -1.0f; break;
        }

        inputProvider.blockInsDown = (blockInst == 1);

        inputProvider.blockPos = new Vector3(offsetX-8, offsetY-4);

        //ïÒèV.
        float deltaX = transform.position.x - previousX;
        deltaX = deltaX < 0 ? deltaX * 2 : deltaX;
        AddReward(progressRewardScale * deltaX);
        previousX = transform.position.x;

        AddReward(-timePenralty);

        //if (action != previousAction) AddReward(-actionPenralty);
        //previousAction = action;

        //stepCount++;
        blockInsResult = 0;
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
        EndEpisode();
    }
    public void HandleStomped()
    {
        AddReward(stompReward);
    }
    public void HandleBlockIns()
    {
        AddReward(-blockInsCost);
    }
}