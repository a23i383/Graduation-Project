using System.Collections.Generic;
//using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using JetBrains.Annotations;
using TMPro;

public class ItemSelectSystem : MonoBehaviour
{
    [Header("Script")]
    [SerializeField] private PlayerController playerContoroller;

    [Header("Prefab")]
    [SerializeField] private GameObject normalIcon;
    [SerializeField] private GameObject fallIcon;
    [SerializeField] private GameObject blackHoleIcon;

    [Header("Figure")]
    [SerializeField] private float iconSpace = 150.0f;
    [SerializeField] private float duration = 0.1f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip selectSE;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI activeBlockQuantity;

    private List<GameObject> Icons = new List<GameObject>();

    private struct IconSlot
    {
        public GameObject obj;
        public RectTransform rect;
        public float basePosX;
        public float baseAlpha;
    }

    [HideInInspector] public int currentIndex;
    [HideInInspector] public bool isAnimation = false;
    [HideInInspector] public float scroll;

    private IconSlot leftSlot, centerSlot, rightSlot, addSlot;
    [Header("end")]
    [SerializeField] private int a;
    void Start()
    {
        SetUpSlots();
        activeBlockQuantity.text = "Å~" + FindAnyObjectByType<StageManager>().normalBlockQuantity;
    }

    void Update()
    {
        scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0 && !isAnimation)
        {
            isAnimation = true;
            StartCoroutine(AnimateScroll(scroll > 0.0f ? 1 : -1));
        }
    }

    private IEnumerator AnimateScroll(int direction)
    {
        audioSource.PlayOneShot(selectSE);

        int addIndex = direction > 0.0f ? Next(Next(currentIndex)) : Prev(Prev(currentIndex));
        float addPosX = direction > 0.0f ? iconSpace * 2 : -iconSpace * 2;
        addSlot = CreateSlot(addIndex, addPosX, 0.0f);
        SetAlpha(addSlot.obj, addSlot.baseAlpha);

        currentIndex = direction > 0.0f ? Next(currentIndex) : Prev(currentIndex);
        activeBlockQuantity.text = "Å~" + playerContoroller.blockQuantity[currentIndex];

        float totalMove = -iconSpace * direction;
        float elapsed = 0.0f;
        while (duration > elapsed)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            AnimateSlotPos(ref leftSlot, totalMove, t);
            AnimateSlotPos(ref centerSlot, totalMove, t);
            AnimateSlotPos(ref rightSlot, totalMove, t);
            AnimateSlotPos(ref addSlot, totalMove, t);

            AnimateSlotAlpha(ref leftSlot, direction, t);
            AnimateSlotAlpha(ref centerSlot, direction, t);
            AnimateSlotAlpha(ref rightSlot, direction, t);
            AnimateSlotAlpha(ref addSlot, direction, t);

            yield return null;
        }
        ResetSlots(direction);
        isAnimation = false;

    }
    private void AnimateSlotPos(ref IconSlot slot, float totalMove, float t)
    {
        float posX = slot.basePosX + Mathf.Lerp(0.0f, totalMove, t);
        slot.rect.anchoredPosition = new Vector2(posX, 0.0f);
    }
    private void AnimateSlotAlpha(ref IconSlot slot, int direction, float t)
    {
        float targetAlpha = GetTargetAlpha(slot.obj, direction);
        float alpha = Mathf.Lerp(slot.baseAlpha, targetAlpha, t);
        SetAlpha(slot.obj, alpha);
    }
    private float GetTargetAlpha(GameObject icon, int direction)
    {
        if ((direction > 0.0f && icon == rightSlot.obj) || (direction < 0.0f && icon == leftSlot.obj)) return 1.0f;
        if (icon == addSlot.obj) return 0.5f;
        if (icon == centerSlot.obj) return 0.5f;
        return 0.0f;
    }
    private void ResetSlots(int direction)
    {
        if (direction > 0.0f)
        {
            Destroy(leftSlot.obj);
            leftSlot = centerSlot;
            centerSlot = rightSlot;
            rightSlot = addSlot;
        }
        else
        {
            Destroy(rightSlot.obj);
            rightSlot = centerSlot;
            centerSlot = leftSlot;
            leftSlot = addSlot;
        }

        leftSlot.basePosX = -iconSpace;
        centerSlot.basePosX = 0.0f;
        rightSlot.basePosX = iconSpace;

        leftSlot.baseAlpha = 0.5f;
        centerSlot.baseAlpha = 1.0f;
        rightSlot.baseAlpha = 0.5f;
    }
    private void SetUpSlots()
    {
        InitIconList();

        currentIndex = 0;

        leftSlot = CreateSlot(Prev(currentIndex), -iconSpace, 0.5f);
        centerSlot = CreateSlot(currentIndex, 0.0f, 1.0f);
        rightSlot = CreateSlot(Next(currentIndex), iconSpace, 0.5f);

        SetAlpha(leftSlot.obj, leftSlot.baseAlpha);
        SetAlpha(centerSlot.obj, centerSlot.baseAlpha);
        SetAlpha(rightSlot.obj, rightSlot.baseAlpha);
    }

    private void InitIconList()
    {
        Icons.Clear();

        if (RespawnManager.instance.normalBlockQuantity > 0) Icons.Add(normalIcon);

        if (RespawnManager.instance.fallBlockQuantity > 0) Icons.Add(fallIcon);

        if (RespawnManager.instance.blackHoleQuantity > 0) Icons.Add(blackHoleIcon);
    }
    private IconSlot CreateSlot(int index, float PosX, float alpha)
    {
        GameObject obj = Instantiate(Icons[index], transform);

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(PosX, 0.0f);

        return new IconSlot { obj = obj, rect = rect, basePosX = PosX, baseAlpha = alpha };
    }
    private void SetAlpha(GameObject icon, float alpha)
    {
        Image image = icon.GetComponent<Image>();
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
    private int Next(int index) => (index + 1) % Icons.Count;
    private int Prev(int index) => (index - 1 + Icons.Count) % Icons.Count;
}

