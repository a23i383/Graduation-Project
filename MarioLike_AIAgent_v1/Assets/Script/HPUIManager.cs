using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPUIManager:MonoBehaviour
{
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform heartPannel;
    [SerializeField] private PlayerController player;

    private List<GameObject> hearts=new List<GameObject> ();
    private int previousHP;

    private void Start()
    {
        previousHP = player.currentHp;
        SetMaxHP (previousHP);
    }
    private void Update()
    {
        int currentHp=player.currentHp;
        if(currentHp < previousHP)
        {
            for(int i = currentHp; i < previousHP; i++)
            {
                hearts[i].GetComponent<Animator>().SetTrigger("Pop");
            }
        }
        previousHP = currentHp;
    }

    private void SetMaxHP(int maxHP)
    {
        foreach(var heart in hearts)
        {
            Destroy(heart);
        }
        hearts.Clear();

        for(int i = 0; i < maxHP; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartPannel);
            hearts.Add(heart);
        }
    }



}
