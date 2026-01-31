using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public float totalDaySeconds = 180f; // 3 mins

    [Header("Spawning")]
    public GameObject[] customerPrefabs;
    public Transform customerSpawnPoint;
    
    public CustomerController CurrentCustomer{get; private set;}

    [Header("Events")]
    public UnityEvent<int> onTipsChanged;
    public UnityEvent<float> onDayTimeRemainingChanged;
    public UnityEvent onDayEnded;

    private float remainingDayTime;
    private int totalTips;
    private bool isDayActive;

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
        
        // Destroy existing customer if any
        if (CurrentCustomer != null)
        {
            Destroy(CurrentCustomer.gameObject);
        }

        if (customerPrefabs == null || customerPrefabs.Length == 0)
        {
            Debug.LogWarning("No customer prefabs assigned!");
            return;
        }

        Debug.Log("Next customer arriving...");
        
        // Spawn random prefab
        int randomIndex = Random.Range(0, customerPrefabs.Length);
        GameObject prefab = customerPrefabs[randomIndex];
        GameObject instance = Instantiate(prefab, customerSpawnPoint.position, Quaternion.identity, customerSpawnPoint);
        
        CurrentCustomer = instance.GetComponent<CustomerController>();
        
        if (CurrentCustomer != null)
        {
            // Connect UI events
            UIManager ui = FindFirstObjectByType<UIManager>();
            if (ui != null)
            {
                CurrentCustomer.onEmotionsChanged.AddListener(ui.UpdateCustomerEmotions);
                CurrentCustomer.onTimerChanged.AddListener(ui.UpdateCustomerTimer);
                CurrentCustomer.onFaceChanged.AddListener(ui.UpdateFaceSprite);
            }

            CurrentCustomer.InitializeCustomer();
        }
    }

    private void EndDay()
    {
        isDayActive = false;
        remainingDayTime = 0;
        onDayEnded?.Invoke();
        Debug.Log("Day ended! Total Tips: " + totalTips);

        if (CurrentCustomer != null)
        {
            CurrentCustomer.gameObject.SetActive(false);
        }
    }

    public void OnCustomerFinished(int tipsEarned)
    {
        AddTips(tipsEarned);
        if (isDayActive)
        {
            // Small delay before next customer could be added here if needed
            StartNextCustomer();
        }
    }
}
