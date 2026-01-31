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
    public float currentAnger;
    public float currentMelancholy;
    public float timer = 15f;

    [Header("Settings")]
    public GameObject acupointPrefab;
    public Transform backArea; // Area where acupoints spawn

    public UnityEvent<float, float> onEmotionsChanged; // (Anger, Melancholy)
    public UnityEvent<float> onTimerChanged;

    private bool isActive = false;

    public void InitializeCustomer()
    {
        currentAnger = maxAnger;
        currentMelancholy = maxMelancholy;
        timer = 15f;
        isActive = true;
        
        onEmotionsChanged?.Invoke(currentAnger / maxAnger, currentMelancholy / maxMelancholy);
        
        SpawnInitialAcupoints();
    }

    private void Update()
    {
        if (!isActive) return;

        timer -= Time.deltaTime;
        onTimerChanged?.Invoke(timer);

        if (timer <= 0)
        {
            FinishCustomer();
        }
    }

    private void SpawnInitialAcupoints()
    {
        // For demonstration, spawn a few random ones
        for (int i = 0; i < 5; i++)
        {
            SpawnAcupoint();
        }
    }

    public void SpawnAcupoint()
    {
        if (acupointPrefab == null) return;
        
        Vector3 randomPos = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
        GameObject go = Instantiate(acupointPrefab, backArea.position + randomPos, Quaternion.identity, backArea);
        
        Acupoint acupoint = go.GetComponent<Acupoint>();
        if (acupoint != null)
        {
            acupoint.Setup(this, Random.value > 0.5f ? EmotionType.Anger : EmotionType.Melancholy);
        }
    }

    public void ReduceEmotion(EmotionType type, float amount)
    {
        if (type == EmotionType.Anger)
            currentAnger = Mathf.Max(0, currentAnger - amount);
        else
            currentMelancholy = Mathf.Max(0, currentMelancholy - amount);

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

        // Calculate tip: Base tip + bonus for remaining time
        int baseTip = 10;
        int timeBonus = Mathf.FloorToInt(timer * 2f);
        int finalTip = (currentAnger <= 0 && currentMelancholy <= 0) ? (baseTip + timeBonus) : 0;

        GameManager.Instance.OnCustomerFinished(finalTip);
        
        // Disable or destroy customer object
        gameObject.SetActive(false);
    }
}
