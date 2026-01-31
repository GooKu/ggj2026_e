using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomerController : MonoBehaviour
{
    public enum EmotionType { Anger, Melancholy }

    [Header("Customer Stats")]
    public float maxAnger = 100f;
    public float maxMelancholy = 100f;
    public float startAnger = 50f;
    public float startMelancholy = 50f;
    public float currentAnger{get; private set;}
    public float currentMelancholy{get; private set;}
    public float customerTimer = 15f; // 客人停留總時間

    private List<Acupoint> acupoints = new List<Acupoint>();

    private void Awake()
    {
        // 自动抓取底下所有的 (包含 inactive 的) Acupoint
        Acupoint[] foundAcupoints = GetComponentsInChildren<Acupoint>(true);
        acupoints = new List<Acupoint>(foundAcupoints);
    }

    public UnityEvent<float, float> onEmotionsChanged; // (Anger, Melancholy)
    public UnityEvent<float> onTimerChanged;

    [Header("Visuals")]
    public Sprite normalFace;
    public Sprite angryFace;
    public Sprite melancholyFace;
    public Sprite happyFace;

    public UnityEvent<Sprite> onFaceChanged;

    [Header("Spawning Config")]
    public float spawnInterval = 2.0f; // 穴道生成間隔
    private float nextSpawnTime;
    private int lastAcupointIndex = -1;

    private bool isActive = false;
    private float elapsedCustomerTime = 0f;

    public void InitializeCustomer()
    {
        currentAnger = startAnger;
        currentMelancholy = startMelancholy;
        elapsedCustomerTime = 0f;
        isActive = true;
        
        onEmotionsChanged?.Invoke(currentAnger / maxAnger, currentMelancholy / maxMelancholy);
        onFaceChanged?.Invoke(normalFace);
        
        // 初始化穴道
        foreach (var acupoint in acupoints)
        {
            if (acupoint != null)
            {
                acupoint.Setup(this);
                // 確保所有穴道初始狀態為不顯示
                acupoint.gameObject.SetActive(false);
            }
        }
        
        nextSpawnTime = 0.5f; // 客人出現後 0.5 秒生成第一個穴道
        lastAcupointIndex = -1;
    }

    private void Update()
    {
        if (!isActive) return;

        elapsedCustomerTime += Time.deltaTime;
        float remainingTime = customerTimer - elapsedCustomerTime;
        onTimerChanged?.Invoke(remainingTime);

        if (remainingTime <= 0)
        {
            FinishCustomer();
            return;
        }

        // 每隔一段時間生成一個穴道
        if (elapsedCustomerTime >= nextSpawnTime)
        {
            SpawnNextAcupoint();
            nextSpawnTime = elapsedCustomerTime + spawnInterval;
        }
    }

    private void SpawnNextAcupoint()
    {
        if (acupoints == null || acupoints.Count == 0) return;

        int index;
        if (acupoints.Count > 1)
        {
            // 確保不與上一個重複
            do
            {
                index = Random.Range(0, acupoints.Count);
            } while (index == lastAcupointIndex);
        }
        else
        {
            index = 0;
        }

        lastAcupointIndex = index;
        if (acupoints[index] != null)
        {
            acupoints[index].gameObject.SetActive(true);
            acupoints[index].Setup(this); // 重設狀態
        }
    }

    public void ApplyEmotionImpact(EmotionType type, float amount)
    {
        if (type == EmotionType.Anger)
            currentAnger = Mathf.Clamp(currentAnger - amount, 0, maxAnger);
        else
            currentMelancholy = Mathf.Clamp(currentMelancholy - amount, 0, maxMelancholy);

        onEmotionsChanged?.Invoke(currentAnger / maxAnger, currentMelancholy / maxMelancholy);

        if (currentAnger <= 0 && currentMelancholy <= 0)
        {
            FinishCustomer();
        }
    }

    private void FinishCustomer()
    {
        if (!isActive) return;
        isActive = false;

        // 隱藏所有穴位
        foreach (var acupoint in acupoints)
        {
            if (acupoint != null) acupoint.DeactivateAcupoint();
        }

        // 計算小費
        int baseTip = 10;
        float remainingTime = customerTimer - elapsedCustomerTime;
        int timeBonus = Mathf.FloorToInt(Mathf.Max(0, remainingTime) * 2f);
        int finalTip = (currentAnger <= 0 && currentMelancholy <= 0) ? (baseTip + timeBonus) : 0;

        GameManager.Instance.OnCustomerFinished(finalTip);
        
        gameObject.SetActive(false);
    }

    private Coroutine reactionCoroutine;
    public void ShowReaction(float impactA, float impactM)
    {
        Sprite selectedSprite = normalFace;

        if (impactA < 0 || impactM < 0)
        {
            if (impactA < impactM)
            {
                selectedSprite = angryFace;
            }
            else if (impactM < impactA)
            {
                selectedSprite = melancholyFace;
            }
            else
            {
                // Both negative and equal, pick randomly
                selectedSprite = Random.value > 0.5f ? angryFace : melancholyFace;
            }
        }
        else
        {
            // All positive or zero
            selectedSprite = happyFace;
        }

        if (reactionCoroutine != null) StopCoroutine(reactionCoroutine);
        reactionCoroutine = StartCoroutine(ReactionRoutine(selectedSprite));
    }

    private IEnumerator ReactionRoutine(Sprite reactionSprite)
    {
        onFaceChanged?.Invoke(reactionSprite);
        yield return new WaitForSeconds(1.0f);
        onFaceChanged?.Invoke(normalFace);
        reactionCoroutine = null;
    }
}
