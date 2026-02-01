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
    public UnityEvent<string> onDialogueChanged;

    [Header("Dialogues")]
    public List<string> moodDialoguesHappy;
    public List<string> moodDialoguesAngry;
    public List<string> moodDialoguesMelancholy;

    [Header("Spawning Config")]
    public float spawnInterval = 2.0f; // 穴道生成間隔
    private float nextSpawnTime;
    private int lastAcupointIndex = -1;

    private bool isActive = false;
    private float elapsedCustomerTime = 0f;
    private int consecutiveMisses = 0;

    public void InitializeCustomer()
    {
        currentAnger = startAnger;
        currentMelancholy = startMelancholy;
        elapsedCustomerTime = 0f;
        consecutiveMisses = 0;
        isActive = true;
        
        onEmotionsChanged?.Invoke(currentAnger / maxAnger, currentMelancholy / maxMelancholy);
        onFaceChanged?.Invoke(GetIdleFace());
        
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

        // 如果 A 或 M 達到上限，立即結束並扣 20 元
        if (currentAnger >= maxAnger || currentMelancholy >= maxMelancholy)
        {
            FinishCustomer(true);
            return;
        }

        if (currentAnger <= 0 && currentMelancholy <= 0)
        {
            FinishCustomer(false);
        }
        else if (reactionCoroutine == null)
        {
            // 如果不在點擊反應中，依據當前數值更新表情
            onFaceChanged?.Invoke(GetIdleFace());
        }
    }

    public void RegisterHit()
    {
        consecutiveMisses = 0;
    }

    public void RegisterMiss()
    {
        consecutiveMisses++;
        if (consecutiveMisses > GameManager.Instance.missThreshold)
        {
            float penalty = GameManager.Instance.missPenalty;
            // 負面條上升 y (對這款遊戲來說增加 A/M 是負面的，所以 ApplyEmotionImpact 傳入負值)
            // ApplyEmotionImpact(type, amount) 內部是 Clamp(current - amount)
            // 所以要上升的話要減去負值，或者直接修改 ApplyEmotionImpact
            // 這裡直接呼叫 ApplyEmotionImpact 傳入 -penalty 即可增加數值
            ApplyEmotionImpact(EmotionType.Anger, -penalty);
            ApplyEmotionImpact(EmotionType.Melancholy, -penalty);
            Debug.Log($"Consecutive misses: {consecutiveMisses}. Penalty applied: +{penalty} A/M");
        }
    }

    public void FinishCustomer(bool isMaxPenalty = false)
    {
        if (!isActive) return;
        isActive = false;

        // 隱藏所有穴位
        foreach (var acupoint in acupoints)
        {
            if (acupoint != null) acupoint.DeactivateAcupoint();
        }

        int finalTip = 0;
        bool isPerfect = false;

        if (isMaxPenalty)
        {
            finalTip = -20;
            isPerfect = false;
            Debug.Log("Customer reached max A/M! Penalty: -20");
        }
        else
        {
            // 計算小費
            int baseTip = 10;
            float remainingTime = customerTimer - elapsedCustomerTime;
            int timeBonus = Mathf.FloorToInt(Mathf.Max(0, remainingTime) * 2f);
            
            isPerfect = currentAnger <= 0 && currentMelancholy <= 0;
            finalTip = isPerfect ? (baseTip + timeBonus) : 0;
        }

        GameManager.Instance.OnCustomerFinished(finalTip, isPerfect);
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    private Coroutine reactionCoroutine;
    public void ShowReaction(bool isPerfect)
    {
        Sprite selectedSprite = isPerfect ? happyFace : normalFace;
        List<string> selectedList = isPerfect ? moodDialoguesHappy : null; // 平靜表情暫時沒有專屬對話

        if (reactionCoroutine != null) StopCoroutine(reactionCoroutine);
        reactionCoroutine = StartCoroutine(ReactionRoutine(selectedSprite, ""));
    }

    public Sprite GetIdleFace()
    {
        if (currentAnger <= 0 && currentMelancholy <= 0) return happyFace;
        
        if (currentAnger > currentMelancholy) return angryFace;
        if (currentMelancholy > currentAnger) return melancholyFace;
        
        // Equal and > 0
        return Random.value > 0.5f ? angryFace : melancholyFace;
    }

    private IEnumerator ReactionRoutine(Sprite reactionSprite, string reactionText)
    {
        onFaceChanged?.Invoke(reactionSprite);
        if (!string.IsNullOrEmpty(reactionText)) 
            onDialogueChanged?.Invoke(reactionText);
            
        yield return new WaitForSeconds(1.0f);
        onFaceChanged?.Invoke(GetIdleFace());
        reactionCoroutine = null;
    }
}
