using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public float totalDaySeconds = 180f; // 3 mins
    public float customerDuration = 15f;

    [Header("Game State")]
    public float remainingDayTime;
    public int totalTips = 0;
    public bool isDayActive = false;

    public UnityEvent<int> onTipsChanged;
    public UnityEvent<float> onDayTimeRemainingChanged;
    public UnityEvent onDayEnded;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        remainingDayTime = totalDaySeconds;
        totalTips = 0;
        isDayActive = true;
        onTipsChanged?.Invoke(totalTips);
        
        StartNextCustomer();
    }

    private void Update()
    {
        if (!isDayActive) return;

        remainingDayTime -= Time.deltaTime;
        onDayTimeRemainingChanged?.Invoke(remainingDayTime);

        if (remainingDayTime <= 0)
        {
            EndDay();
        }
    }

    public void AddTips(int amount)
    {
        totalTips += amount;
        onTipsChanged?.Invoke(totalTips);
    }

    private void StartNextCustomer()
    {
        if (!isDayActive) return;
        
        Debug.Log("Next customer arriving...");
        // Logic to spawn/activate a customer would go here
    }

    private void EndDay()
    {
        isDayActive = false;
        remainingDayTime = 0;
        onDayEnded?.Invoke();
        Debug.Log("Day ended! Total Tips: " + totalTips);
    }

    public void OnCustomerFinished(int tipsEarned)
    {
        AddTips(tipsEarned);
        if (isDayActive)
        {
            StartNextCustomer();
        }
    }
}
