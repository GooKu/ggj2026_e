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

    [Header("Statistics")]
    private int totalCustomers;
    private int perfectServicesCount;

    [Header("UI Reference")]
    public DailyResultUI dailyResultUI;
    public CustomerResultUI customerResultUI;

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UIManager ui = FindFirstObjectByType<UIManager>();
        if (ui != null)
        {
            onTipsChanged.AddListener(ui.UpdateTips);
            onDayTimeRemainingChanged.AddListener(ui.UpdateDayTimer);
        }
        StartGame();
    }

    public void StartGame()
    {
        remainingDayTime = totalDaySeconds;
        totalTips = 0;
        totalCustomers = 0;
        perfectServicesCount = 0;
        isDayActive = true;
        isPaused = false;
        onTipsChanged?.Invoke(totalTips);
        
        StartNextCustomer();
    }

    private void Update()
    {
        if (!isDayActive || isPaused) return;

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

        if (CurrentCustomer != null && CurrentCustomer.gameObject.activeInHierarchy)
        {
            CurrentCustomer.FinishCustomer();
        }
        else
        {
            ShowDailyResult();
        }
    }

    public void OnCustomerFinished(int tipsEarned, bool isPerfect)
    {
        totalCustomers++;
        if (isPerfect) perfectServicesCount++;
        
        AddTips(tipsEarned);
        
        if (customerResultUI != null)
        {
            StartCoroutine(ShowResultAndContinue(tipsEarned, isPerfect));
        }
        else
        {
            Debug.LogWarning("CustomerResultUI is not assigned! Skipping settlement pause.");
            HandleAfterSettlement();
        }
    }

    private IEnumerator ShowResultAndContinue(int tips, bool isPerfect)
    {
        isPaused = true;
        
        if (customerResultUI != null)
        {
            customerResultUI.ShowResult(tips, isPerfect);
            yield return new WaitForSecondsRealtime(customerResultUI.displayDuration);
        }

        isPaused = false;
        HandleAfterSettlement();
    }

    private void HandleAfterSettlement()
    {
        if (isDayActive)
        {
            StartNextCustomer();
        }
        else
        {
            ShowDailyResult();
        }
    }

    private void ShowDailyResult()
    {
        if (dailyResultUI != null)
        {
            dailyResultUI.gameObject.SetActive(true);
            dailyResultUI.Setup(totalTips, perfectServicesCount, totalCustomers);
        }
        else
        {
            Debug.LogWarning("DailyResultUI is not assigned!");
        }
    }
}
