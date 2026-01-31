using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Acupoint : MonoBehaviour
{
    public CustomerController.EmotionType type;
    public float reductionAmount = 10f;
    
    [Header("OSU Mechanics")]
    public float lifetime = 2f;
    public RectTransform approachCircle;
    
    private CustomerController master;
    private float aliveTime = 0f;

    public void Setup(CustomerController controller, CustomerController.EmotionType emotionType)
    {
        master = controller;
        type = emotionType;
        aliveTime = 0f;
        
        // Visual feedback based on type
        // e.g., change color to Red for Anger, Blue for Melancholy
        GetComponent<SpriteRenderer>().color = (type == CustomerController.EmotionType.Anger) ? Color.red : Color.blue;
    }

    private void Update()
    {
        aliveTime += Time.deltaTime;
        
        // Approach circle scaling logic
        if (approachCircle != null)
        {
            float scale = Mathf.Lerp(3f, 1f, aliveTime / lifetime);
            approachCircle.localScale = new Vector3(scale, scale, 1);
        }

        if (aliveTime >= lifetime)
        {
            // Missed!
            DestroyAcupoint();
        }
    }

    private void OnMouseDown()
    {
        // In a real OSU game, we check the scale of approach circle for Perfect/Great/Miss
        // Here we simplify for the base code
        float accuracy = 1f - Mathf.Abs((aliveTime / lifetime) - 0.9f); // Best hit near 90% of lifetime
        
        if (accuracy > 0.7f)
        {
            master.ReduceEmotion(type, reductionAmount * (accuracy > 0.9f ? 1.5f : 1f));
            master.SpawnAcupoint(); // Spawn another one to keep the flow
        }
        
        DestroyAcupoint();
    }

    private void DestroyAcupoint()
    {
        Destroy(gameObject);
    }
}
