using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.Events;

public class ShipManager : MonoBehaviour
{
    public static ShipManager Instance;
    public enum ShipState { Normal, Madness }
    public ShipState CurrentState { get; private set; }
    public float TimeFromStart { get; private set; } = 0;

    [Header("SoundData")]
    [SerializeField] private SoundDataSO normalSoundData;
    [SerializeField] private SoundDataSO madnessSoundData;
    private SoundEmitter normalSE;
    private SoundEmitter madnessSE;
    public SoundDataSO CurrentSoundData { get; private set; }

    [Header("Настройки здоровья")]
    [Tooltip("Максимальное здоровье корабля")]
    [SerializeField] private float maxHealth = 100f;
    public float MaxHealth => maxHealth; 
    public float CurrentHealth { get; private set; }



    [Header("Настройки Безумия (Madness)")]
    [Tooltip("Максимальное значение полосы Безумия. При достижении этого значения корабль переходит в состояние Madness.")]
    [SerializeField] private float maxMadnessValue = 100f;
    public float MaxMadnessValue => maxMadnessValue;

    [Tooltip("Базовая скорость, с которой Безумие растет само по себе, даже без задач")]
    [SerializeField] private float madnessBaseIncreaseRate = 1.0f;
    [Tooltip("Насколько быстро полоса Безумия убывает в состоянии Madness (единиц в секунду).")]
    [SerializeField] private float madnessDecayRate = 0.001f;
    private ChangeAtmosphere sceneAtmosphere;


    public float CurrentMadness { get; private set; }

    [Header("Настройки Задач")]
    [Tooltip("Список всех зон, где могут появляться задачи")]
    [SerializeField] private List<ShipTaskZone> shipTaskZones; 
    public List<ShipTaskZone> ShipTaskZones => shipTaskZones;

    [Tooltip("Как часто (в секундах) игра пытается создать новую задачу")]
    public float taskSpawnInterval = 10f;
    private float taskSpawnTimer;

    [Header("Настройки Монет")]
    [Tooltip("Как часто (в секундах) игра добавляет монетки")]
    public float coinAddInterval = 30f;
    private float coinAddTimer;

    [Tooltip("Сколько монеток добавляется за интервал")]
    public int coinIncreaseRate = 1;
    public int CurrentCoins { get; private set; } = 0;


    public UnityEvent OnHealthChanged;
    public UnityEvent OnMadnessChanged;
    public UnityEvent OnCoinsAmountChanged;
    public UnityEvent OnGameEnd;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        CurrentHealth = maxHealth;
        CurrentState = ShipState.Normal;
        CurrentMadness = 0f;
        TimeFromStart = 0f;
        taskSpawnTimer = taskSpawnInterval;
        coinAddTimer = coinAddInterval;
        sceneAtmosphere = transform.GetComponent<ChangeAtmosphere>();

        normalSE = SoundManager.Instance.Get().Initialize(normalSoundData);
        normalSE.Play();
        CurrentSoundData = normalSoundData;
    }

    // Update is called once per frame
    void Update()
    {
        TimeFromStart += Time.deltaTime;

        HandleTaskSpawning();

        switch (CurrentState)
        {
            case ShipState.Normal:
                UpdateNormalState();
                break;
            case ShipState.Madness:
                UpdateMadnessState();
                break;
        }
        OnMadnessChanged?.Invoke();
        
    }

    private void HandleTaskSpawning()
    {
        taskSpawnTimer -= Time.deltaTime;
        if (taskSpawnTimer <= 0)
        {
            SpawnNewTask();
            taskSpawnTimer = taskSpawnInterval;
        }
    }

    public void CollectCoins(int collectedCoins)
    {
        CurrentCoins += collectedCoins;
        OnCoinsAmountChanged?.Invoke();
    }

    private void SpawnNewTask()
    {
       
        List<ShipTaskZone> availableZones = shipTaskZones.Where(zone => !zone.IsFull()).ToList();
        
        if (availableZones.Count > 0)
        {
            ShipTaskZone randomZone = availableZones[Random.Range(0, availableZones.Count)];
            

            randomZone.TrySpawnNewTask(this);
        }
    }

    private void UpdateNormalState()
    {
        float totalIncreaseRate = madnessBaseIncreaseRate;

        foreach (var zone in shipTaskZones)
        {
            foreach (var task in zone.TaskList)
            {
                totalIncreaseRate += task.GetCurrentMadnessRate();
            }
        }

        //CurrentMadness += totalIncreaseRate * Time.deltaTime;
        IncreaseMadness(totalIncreaseRate * Time.deltaTime);

        if (CurrentMadness >= maxMadnessValue)
        {
            CurrentMadness = maxMadnessValue;
            CurrentState = ShipState.Madness;
            normalSE.Stop();
            madnessSE = SoundManager.Instance.Get().Initialize(madnessSoundData);
            madnessSE.Play();
            CurrentSoundData = madnessSoundData;
            sceneAtmosphere.ToMadness();
            Debug.Log("КОРАБЛЬ ОХВАЧЕН БЕЗУМИЕМ!");
        }
    }
    
    private void UpdateMadnessState()
    {
        DecreaseMadness(madnessDecayRate * Time.deltaTime);

        if (CurrentMadness <= 0)
        {
            CurrentMadness = 0;
            CurrentState = ShipState.Normal;
            madnessSE.Stop();
            normalSE = SoundManager.Instance.Get().Initialize(normalSoundData);
            normalSE.Play();
            CurrentSoundData = normalSoundData;
            sceneAtmosphere.ToNormal();
            Debug.Log("Безумие отступило. Корабль в обычном состоянии.");
        }



    }

    public Vector3 GetInvertedShipTask(ShipTask shipTask)
    {
        int zoneIndex = shipTaskZones.IndexOf(shipTask.parentZone);
        int invertedZoneIndex = (zoneIndex + shipTaskZones.Count / 2) % shipTaskZones.Count;
        ShipTaskZone invertedTaskZone = shipTaskZones[invertedZoneIndex];
        
        foreach(var invShipTask in invertedTaskZone.prePlacedTasks)
        {
            if (invShipTask.taskObject.TaskType == shipTask.TaskType)
            {
                return invShipTask.taskObject.transform.position;
            }
        }
        return Vector3.zero;
    }

    public void ChangeHealth(float damage)
    {
        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
        OnHealthChanged?.Invoke();
        if (CurrentHealth <= 0)
        {
            OnGameEnd?.Invoke();
        }

    }

    public void IncreaseMadness(float amount)
    {
        if (CurrentState == ShipState.Normal)
        {
            CurrentMadness = Mathf.Min(maxMadnessValue, CurrentMadness + amount);
        }
    }

    public void DecreaseMadness(float amount)
    {
        CurrentMadness = Mathf.Max(0, CurrentMadness - amount);
    }


    public void StartWorkInZone(TaskType task, int zoneIndex)
    {
        if (zoneIndex < 0 || zoneIndex >= shipTaskZones.Count) return;

        shipTaskZones[zoneIndex].StartWork(task);
    }

    public void StopWorkInZone(TaskType task, int zoneIndex)
    {
        if (zoneIndex < 0 || zoneIndex >= shipTaskZones.Count) return;

        shipTaskZones[zoneIndex].StopWork(task);
    }

    public bool IsTaskActiveInZone(TaskType task, int zoneIndex)
    {
        if (zoneIndex < 0 || zoneIndex >= shipTaskZones.Count) return false;
        return shipTaskZones[zoneIndex].IsTaskActive(task);
    }
}
