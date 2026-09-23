using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private GameObject infoUI;
    [SerializeField] private TextMeshProUGUI endText;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject normalBlockInfo;
    [SerializeField] private GameObject fallBlockInfo;
    [SerializeField] private GameObject blackHoleInfo;
    [SerializeField] private float waitTime = 3.0f;
    [SerializeField] private Transform blockGroup;
    [SerializeField] private AudioSource BGMSource;


    [HideInInspector] public Vector2 spawnPosition;
    [HideInInspector] public bool isOnRespawn = false;
    [HideInInspector] public bool isFirstGame = true;
    [HideInInspector] public List<installedBlocks> installBlockList = new List<installedBlocks>();
    [HideInInspector] public List<installedEnemys> installEnemyList = new List<installedEnemys>();
    [HideInInspector] public List<GameObject> destroyBlockList = new List<GameObject>();


    public int baseNormalBlockQuantity = 1;
    public int baseFallBlockQuantity = 1;
    public int baseBlackHoleQuantity = 1;

    [HideInInspector] public int normalBlockQuantity;
    [HideInInspector] public int fallBlockQuantity;
    [HideInInspector] public int blackHoleQuantity;

    [HideInInspector] public int playerHp;


    public struct installedBlocks
    {
        public GameObject block;
        public Vector3 rocation;
    }
    public struct installedEnemys
    {
        public GameObject prefab;
        public Vector3 rocation;
    }

    private void Start()
    {
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        if (RespawnManager.instance.isFirstGame) StartCoroutine(PlayIntro());
        else
        {
            infoUI.SetActive(false);
            BGMSource.Play();
        }
        endText.text = "";
        SetStageInfo();
}
    private IEnumerator PlayIntro()
    {
        playerController.canControl = false;
        Time.timeScale = 0.0f;
        InstallationUIBlock();
        //ShowStageInfo();
        yield return new WaitForSecondsRealtime(waitTime);
        infoUI.SetActive(false);
        Time.timeScale = 1.0f;
        playerController.canControl = true;
        BGMSource.Play();
    }
    private void InstallationUIBlock()
    {
        if (baseNormalBlockQuantity > 0) SetTextInfo(Instantiate(normalBlockInfo, blockGroup), baseNormalBlockQuantity);
        if (baseFallBlockQuantity > 0) SetTextInfo(Instantiate(fallBlockInfo, blockGroup), baseFallBlockQuantity);
        if (baseBlackHoleQuantity > 0) SetTextInfo(Instantiate(blackHoleInfo, blockGroup), baseBlackHoleQuantity);
    }
    private void SetTextInfo(GameObject blockInfo,int quantity)
    {
        blockInfo.GetComponentInChildren<TextMeshProUGUI>().text = "×" + quantity;
    }
    public void GoalShowText()
    {
        endText.text = "Goal!!!";
    }
    public void GameOverShowText()
    {
        endText.text = "Game Over...";
    }
    private void StageInit()
    {
        foreach(RespawnManager.installedBlocks installedBlocks in RespawnManager.instance.installBlockList)
        {
            if (installedBlocks.block != playerController.blackHole) Instantiate(installedBlocks.block, installedBlocks.rocation, Quaternion.identity);

            if (installedBlocks.block == playerController.normalBlock) normalBlockQuantity--;
            else if(installedBlocks.block==playerController.fallBlock) fallBlockQuantity--;
            else if(installedBlocks.block==playerController.blackHole) blackHoleQuantity--;
        }

        if (RespawnManager.instance.isOnRespawn)
        {
            playerController.gameObject.transform.position = RespawnManager.instance.spawnPosition;
            Camera.main.transform.position= RespawnManager.instance.spawnPosition;
        }
    }
    public void SetStageInfo()
    {
        playerHp = playerController.hp;

        normalBlockQuantity = baseNormalBlockQuantity;
        fallBlockQuantity = baseFallBlockQuantity;
        blackHoleQuantity = baseBlackHoleQuantity;

        EnemyBase[] enemys = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
        foreach (EnemyBase enemy in enemys)
        {
            EnemyIdentifier id = enemy.GetComponent<EnemyIdentifier>();
            if (id == null) continue;

            installedEnemys EnemyData = new installedEnemys()
            {
                prefab = id.sourcePrefab,
                rocation = id.transform.position,
            };
            installEnemyList.Add(EnemyData);
        }
    }
    public void ResetStage()
    {
        //エネミーの再配置.
        EnemyBase[] enemys = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
        foreach (EnemyBase enemy in enemys)
        {
            Destroy(enemy.gameObject);
        }
        foreach (installedEnemys enemyData in installEnemyList)
        {
            Instantiate(enemyData.prefab, enemyData.rocation, Quaternion.identity);
        }

        //ブロックの再配置(再配置ではない).
        if (!isOnRespawn)
        {
            foreach (GameObject destroyBlock in destroyBlockList)
            {
                if (destroyBlock != null) Destroy(destroyBlock);
            }
        }
        destroyBlockList.Clear();
        playerController.InitBlocks();

        //プレイヤーの再配置.
        PlayerController player = FindFirstObjectByType<PlayerController>();
        player.transform.position = new Vector3(0, 0, 0);
        player.ResetPlayer();

        //UIのリセット.
        endText.text = string.Empty;
    }
}
