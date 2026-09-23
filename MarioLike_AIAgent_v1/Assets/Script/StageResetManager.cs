using System.Collections.Generic;
using UnityEngine;

public class StageResetManager : MonoBehaviour
{
    [HideInInspector] public static StageResetManager instance;
    [HideInInspector] public Vector2 spawnPosition;
    [HideInInspector] public bool isOnRespawn = false;
    [HideInInspector] public bool isFirstGame = true;
    [HideInInspector] public List<installedBlocks> installBlockList = new List<installedBlocks>();
    [HideInInspector] public List<installedEnemys> installEnemyList = new List<installedEnemys>();
    [HideInInspector] public List<GameObject> destroyBlockList = new List<GameObject>();
    [HideInInspector] public int normalBlockQuantity;
    [HideInInspector] public int fallBlockQuantity;
    [HideInInspector] public int blackHoleQuantity;

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

    private void Awake()
    {
        if (instance == null)
        {
            StageManager stageManager = FindAnyObjectByType<StageManager>();
            normalBlockQuantity = stageManager.baseNormalBlockQuantity;
            fallBlockQuantity = stageManager.baseFallBlockQuantity;
            blackHoleQuantity = stageManager.baseBlackHoleQuantity;

            EnemyBase[] enemys = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
            foreach(EnemyBase enemy in enemys)
            {
                EnemyIdentifier id = enemy.GetComponent<EnemyIdentifier>();
                if (id == null) continue;

                installedEnemys data = new installedEnemys()
                {
                    prefab = id.sourcePrefab,
                    rocation = id.transform.position,
                };
                installEnemyList.Add(data);
            }


            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            if (!instance.isOnRespawn) instance.installBlockList.Clear();
            instance.isFirstGame = false;
        }
    }

    public void SetStageInfo()
    {
        StageManager stageManager = FindAnyObjectByType<StageManager>();
        normalBlockQuantity = stageManager.baseNormalBlockQuantity;
        fallBlockQuantity = stageManager.baseFallBlockQuantity;
        blackHoleQuantity = stageManager.baseBlackHoleQuantity;

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
        foreach(installedEnemys enemyData in installEnemyList)
        {
            Instantiate(enemyData.prefab, enemyData.rocation, Quaternion.identity);
        }

        //ブロックの再配置(再配置ではない).
        if (!isOnRespawn)
        {
            foreach(GameObject destroyBlock in destroyBlockList)
            {
                if (destroyBlock != null) Destroy(destroyBlock);
            }
        }
        destroyBlockList.Clear();

        //プレイヤーの再配置.
        PlayerController player=FindFirstObjectByType<PlayerController>();
        player.transform.position = new Vector3(0, 0, 0);
    }
}
