using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Audio;

public enum TaskType
{
    Gun,
    FloorHole,
    SideHole,
    Fire
}

public class ShipTask : MonoBehaviour
{
    [Header("Тип Задачи")]
    [SerializeField]
    private TaskType taskType;
    public TaskType TaskType => taskType;

    [Header("Параметры Задачи")]

    [SerializeField] protected int coinsForTask = 5;

    [Tooltip("Сколько времени (в секундах) нужно, чтобы выполнить задачу")]
    [SerializeField] protected float timeToComplete = 5.0f;

    [Tooltip("Какой урон в секунду наносит эта задача в состоянии 'Madness'")]
    [SerializeField] protected float baseDamageInMadness = 1f;
    [Tooltip("Базовая скорость, с которой эта задача заполняет полосу Безумия (ед/сек)")]
    [SerializeField] protected float baseMadnessRate = 0.5f;

    [SerializeField] protected float normalDamage = 5f;
    [SerializeField] protected float madnessDamage = 5f;

    [Header("Параметры Провала Задачи")]
    [Tooltip("Время в секундах, по истечении которого задача 'проваливается'")]
    [SerializeField] protected float failureTime = 20f;
    [Tooltip("Единовременный штраф к Безумию при провале в обычном состоянии")]
    [SerializeField] protected float madnessPenalty = 10f;
    [Tooltip("Единовременный урон кораблю при провале в состоянии Безумия")]
    [SerializeField] protected float damagePenalty = 10f;
    [Tooltip("На сколько увеличивается 'влияние' этой задачи на Безумие после провала")]
    [SerializeField] protected float baseMadnessRateIncreaseOnFailure = 0.25f;
    [Tooltip("На сколько увеличивается урон этой задачи по кораблю после провала")]
    [SerializeField] protected float baseDamageInMadnessIncreaseOnFailure = 0.5f;


    [Header("Параметры Анимации")]
    [Tooltip("Примерная длительность анимации появления для этой задачи (в секундах)")]
    public float appearanceAnimationDuration = 4.0f;
    public float taskCreationTiming = 1.0f;
    [SerializeField] protected TentacleAnimName tentacleAnimName;
    private Animator taskAnimator;

    [SerializeField] protected GameObject highlightedObj;

    protected float currentFailureTimer;
    protected float currentbaseMadnessRate;
    protected float currentbaseDamageInMadness; 
    
    public float CurrentProgress { get; protected set; }
    private int crewWorkingOnTask = 0;
    protected bool isFailed = false;

    public ShipTaskZone parentZone { get; private set; }
    public bool IsResolved { get; protected set; }

    protected float currentDamage;

    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool isTaskActive => spriteRenderer.color.a > 0f;

    protected SoundEmitter soundEmitter;
    protected virtual void Awake()
    {
        taskAnimator = GetComponent<Animator>();
        parentZone = GetComponentInParent<ShipTaskZone>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected void Start()
    {
        ResetVariables();
        normalDamage = 10f;
        madnessDamage = 10f;
    }

    protected void Update()
    {
        if (isTaskActive && !isFailed)
        {
            HandleFailureTimer();
        }

        if (crewWorkingOnTask > 0)
        {
            CurrentProgress = Mathf.Min(timeToComplete, CurrentProgress + Time.deltaTime*crewWorkingOnTask);
            if (CurrentProgress >= timeToComplete)
            {
                Complete();
            }
        }
    }
    private void OnMouseEnter()
    {
        if (highlightedObj != null && ShipManager.Instance.CurrentState == ShipManager.ShipState.Madness)
            highlightedObj.SetActive(true);
    }

    private void OnMouseExit()
    {
        if (highlightedObj != null)
            highlightedObj.SetActive(false);
    }
    public virtual void ActivateTask()
    {
        currentDamage = ShipManager.Instance.CurrentState == ShipManager.ShipState.Normal ? normalDamage : madnessDamage;
        ShipManager.Instance.ChangeHealth(currentDamage);

        SoundManager.Instance.Get().Initialize(tentacleAnimName.breakSoundData).Play();

        gameObject.SetActive(true);
        if (highlightedObj != null)
            highlightedObj.SetActive(false);

        SetSpriteAlpha(1f);
        if (taskAnimator != null)
        {
            taskAnimator.SetBool("IsActive", true);
        }
    }

    private void SetSpriteAlpha(float alpha)
    {
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }

    public virtual IEnumerator HandleTentacleSequence(TentacleAnimator tentacle)
    {
        tentacle.skeletonAnimation.AnimationState.SetAnimation(0, tentacleAnimName.animName, false);
        yield return new WaitForSeconds(taskCreationTiming);
        ActivateTask();
        parentZone.AddTask(this);

        yield return StartCoroutine(WaitForDeleteTentacle(tentacle));

    }

    protected IEnumerator WaitForDeleteTentacle(TentacleAnimator tentacle)
    {
        float remainingAnimationTime = appearanceAnimationDuration - taskCreationTiming;
        if (remainingAnimationTime > 0)
        {
            yield return new WaitForSeconds(remainingAnimationTime);
        }

        if (tentacle.gameObject != null)
        {
            Destroy(tentacle.gameObject);
        }
    }

    public void StartWork()
    {
        if (crewWorkingOnTask == 0)
        {
            soundEmitter = SoundManager.Instance.Get().Initialize(tentacleAnimName.fixSoundData);
            soundEmitter.Play();
        }
        crewWorkingOnTask++;
    }

    public void StopFix()
    {
       // crewWorkingOnTask = 0;
        //soundEmitter.Stop();
    }

    protected void HandleFailureTimer()
    {
        currentFailureTimer = currentFailureTimer - Time.deltaTime;

        if (currentFailureTimer <= 0)
        {
            TriggerFailure();
        }
    }

    protected void TriggerFailure()
    {
        isFailed = true;
        IsResolved = true;

        parentZone.ClearTask(this);

        if (ShipManager.Instance.CurrentState == ShipManager.ShipState.Normal)
        {
           // ShipManager.Instance.IncreaseMadness(madnessPenalty);
            currentbaseMadnessRate += baseMadnessRateIncreaseOnFailure;
            Debug.Log($"Безумие увеличилось на {madnessPenalty}. Новое влияние задачи: {currentbaseMadnessRate}");
        }
        else
        {
           // ShipManager.Instance.ChangeHealth(damagePenalty);
            currentbaseDamageInMadness += baseDamageInMadnessIncreaseOnFailure;
            Debug.Log($"Корабль получил {damagePenalty} урона!");
        }
        StopFix();
        ResetTask();
    }

    protected virtual void Complete()
    {
        IsResolved = true;
        parentZone.ClearTask(this);

        ShipManager.Instance.ChangeHealth(-currentDamage);
        ShipManager.Instance.CollectCoins(coinsForTask);
        ShipManager.Instance.ChangeHealth(currentbaseDamageInMadness * Time.deltaTime);
        StopFix();
        ResetTask();
    }

    public virtual void ResetTask()
    {
        ResetVariables();
        soundEmitter.Stop();
    }

    protected virtual void ResetVariables()
    {
        SetSpriteAlpha(0f);

        if (highlightedObj != null)
            highlightedObj.SetActive(false);


        CurrentProgress = 0f;
        isFailed = false;
        IsResolved = false;
        currentFailureTimer = failureTime;
        currentbaseMadnessRate = baseMadnessRate;
        currentbaseDamageInMadness = baseDamageInMadness;
        crewWorkingOnTask = 0;
    }

    public float GetCurrentMadnessRate()
    {
        return currentbaseMadnessRate;
    }

    public float GetCurrentDamageInMadness()
    {
        return currentbaseDamageInMadness;
    }
}
