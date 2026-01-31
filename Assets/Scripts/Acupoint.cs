using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Acupoint : MonoBehaviour
{
    public CustomerController.EmotionType type;
    public float reductionAmount = 10f;
    
    [Header("Timing Mechanics")]
    public float spawnTime = 1f; // 該穴位在客人出現後幾秒出現
    public float lifetime = 2f;  // 出現後存在的時間
    public RectTransform approachCircle;
    
    private CustomerController master;
    private Button button;
    private float elapsedSinceActive = 0f;
    private bool hasTriggered = false;

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
        
        // 根據類型設置視覺效果 (如果是 Image)
        Image img = GetComponent<Image>();
        if (img != null)
        {
            img.color = (type == CustomerController.EmotionType.Anger) ? Color.red : Color.blue;
        }
    }

    private void Update()
    {
        if (hasTriggered) return;

        elapsedSinceActive += Time.deltaTime;
        
        // 縮放圈圈邏輯 (視覺提示)
        if (approachCircle != null)
        {
            float scale = Mathf.Lerp(3f, 1f, elapsedSinceActive / lifetime);
            approachCircle.localScale = new Vector3(scale, scale, 1);
        }

        if (elapsedSinceActive >= lifetime)
        {
            // 時間到，未點擊即隱藏
            DeactivateAcupoint();
        }
    }

    private void OnAcupointClicked()
    {
        if (hasTriggered) return;
        
        // 計算精準度 (可選)
        float accuracy = 1f - Mathf.Abs((elapsedSinceActive / lifetime) - 0.9f);
        
        master.ReduceEmotion(type, reductionAmount * (accuracy > 0.8f ? 1.2f : 1f));
        DeactivateAcupoint();
    }

    public void DeactivateAcupoint()
    {
        hasTriggered = true;
        gameObject.SetActive(false);
    }
}
