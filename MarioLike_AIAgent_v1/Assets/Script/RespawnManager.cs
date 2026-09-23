using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [HideInInspector] public static RespawnManager instance;
    [HideInInspector] public Vector2 spawnPosition;
    [HideInInspector] public bool isOnRespawn = false;
    [HideInInspector] public bool isFirstGame = true;
    [HideInInspector] public List<installedBlocks> installBlockList = new List<installedBlocks>();
    [HideInInspector] public List<installedEnemys> installEnemyList = new List<installedEnemys>();
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
        public GameObject enemy;
        public Vector3 rocation;
    }

    private void Awake()
    {
        if(instance == null)
        {
            StageManager stageManager = FindAnyObjectByType<StageManager>();
            normalBlockQuantity = stageManager.baseNormalBlockQuantity;
            fallBlockQuantity = stageManager.baseFallBlockQuantity;
            blackHoleQuantity = stageManager.baseBlackHoleQuantity;
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
}
