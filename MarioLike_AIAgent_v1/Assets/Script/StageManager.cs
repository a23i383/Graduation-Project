using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static RespawnManager;

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

    public int normalBlockQuantity = 1;
    public int fallBlockQuantity = 1;
    public int blackHoleQuantity = 1;
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
        StageInit();
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
        if (normalBlockQuantity > 0) SetTextInfo(Instantiate(normalBlockInfo, blockGroup), normalBlockQuantity);
        if (fallBlockQuantity > 0) SetTextInfo(Instantiate(fallBlockInfo, blockGroup), fallBlockQuantity);
        if (blackHoleQuantity > 0) SetTextInfo(Instantiate(blackHoleInfo, blockGroup), blackHoleQuantity);
    }
    private void SetTextInfo(GameObject blockInfo,int quantity)
    {
        blockInfo.GetComponentInChildren<TextMeshProUGUI>().text = "Å~" + quantity;
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
}
