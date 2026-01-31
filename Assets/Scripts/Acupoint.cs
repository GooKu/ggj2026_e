using UnityEngine;
using UnityEngine.UI;

public class Acupoint : MonoBehaviour
{
    [Header("Emotion Impact Range (Positive = Reduce, Negative = Increase)")]
    public int minAngerImpact = 5;
    public int maxAngerImpact = 15;
    public int minMelancholyImpact = 0;
    public int maxMelancholyImpact = 0;
    public int perfectTipAmount = 1;
    
    [Header("Timing Mechanics")]
    public float spawnTime = 1f; // 該穴位在客人出現後幾秒出現
    public float lifetime = 2f;  // 出現後存在的時間
    public RectTransform approachCircle;
    
    private CustomerController master;
    private Button button;
    private float elapsedSinceActive = 0f;
    private bool hasTriggered = false;

    [SerializeField] GameObject perfectImg;
    [SerializeField] GameObject goodImg;
    [SerializeField] GameObject badImage;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnAcupointClicked);
        }
        // 初始隱藏
        gameObject.SetActive(false);
    }

    public void Setup(CustomerController controller)
    {
        master = controller;
        elapsedSinceActive = 0f;
        hasTriggered = false;
    }

    private void Update()
    {
        if (hasTriggered) return;

        elapsedSinceActive += Time.deltaTime;
        
        // 縮放圈圈邏輯 (視覺提示)
        if (approachCircle != null)
        {
            float scale = Mathf.Lerp(3f, 0.88f, elapsedSinceActive / lifetime);
            approachCircle.localScale = new Vector3(scale, scale, 1);
        }

        if (elapsedSinceActive >= lifetime)
        {
            // 時間到，未點擊即隱藏
            master.RegisterMiss();
            if (badImage != null) Instantiate(badImage, transform.position, Quaternion.identity, transform.parent);
            DeactivateAcupoint();
        }
    }

    private void OnAcupointClicked()
    {
        if (hasTriggered) return;

        // 計算精準度
        float accuracy = 1f - Mathf.Abs((elapsedSinceActive / lifetime) - 0.9f);
        float angerImpact = Random.Range(minAngerImpact, maxAngerImpact + 1) * accuracy;
        float melancholyImpact = Random.Range(minMelancholyImpact, maxMelancholyImpact + 1) * accuracy;

        if (Mathf.Abs(angerImpact) > 0.01f) master.ApplyEmotionImpact(CustomerController.EmotionType.Anger, angerImpact);
        if (Mathf.Abs(melancholyImpact) > 0.01f) master.ApplyEmotionImpact(CustomerController.EmotionType.Melancholy, melancholyImpact);
        
        master.RegisterHit();
        master.ShowReaction(angerImpact, melancholyImpact);

        if (accuracy > 0.9f)
        {
            GameManager.Instance.AddTips(perfectTipAmount);
            Debug.Log($"Perfect! Immediate tip: {perfectTipAmount}");
            Instantiate(perfectImg, transform.position, new Quaternion(0, 0, 0, 0), transform.parent);
        }
        else
        {
            Instantiate(goodImg, transform.position, new Quaternion(0, 0, 0, 0), transform.parent);
        }
        
        Debug.Log($"穴位點擊 - 憤怒影響: {angerImpact:F1}, 憂鬱影響: {melancholyImpact:F1} (精準度: {accuracy:F2})");
       
        DeactivateAcupoint();
    }

    public void DeactivateAcupoint()
    {
        hasTriggered = true;
        gameObject.SetActive(false);
    }
}
