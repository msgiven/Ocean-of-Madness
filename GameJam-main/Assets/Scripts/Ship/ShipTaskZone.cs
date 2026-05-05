using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine.Rendering;


public class ShipTaskZone : MonoBehaviour
{
    [Header("Настройка Сценки")]
    [Tooltip("Префаб анимированного щупальца")]
    public GameObject tentacleAnimatorPrefab;

    [System.Serializable]
    public class PrePlacedTask
    {
        public ShipTask taskObject; 
        public Transform animationAnchor;
    }
    [Tooltip("Заполните этот список всеми задачами, которые могут появиться в этой зоне")]
    public List<PrePlacedTask> prePlacedTasks;

    public List<ShipTask> TaskList { get; private set; }
    //[SerializeField, Min(1)] private int MaxTaskQuantity = 2;

    public Dictionary<TaskType, PrePlacedTask> taskRegistry {  get; private set; }
    public bool IsOccupied => TaskList.Count >= taskRegistry.Count;
    public bool IsTaskActive(TaskType type) => TaskList.Any(t => t.TaskType == type);
    private bool isSpawning = false; 

    void Awake()
    {
        TaskList = new List<ShipTask>();

        taskRegistry = new Dictionary<TaskType, PrePlacedTask>();
        foreach (var placement in prePlacedTasks)
        {
            if (placement.taskObject != null && !taskRegistry.ContainsKey(placement.taskObject.TaskType))
            {
                taskRegistry.Add(placement.taskObject.TaskType, placement);
            }
        }
    }

    public void AddTask(ShipTask task)
    {
        if (task.TaskType == TaskType.Fire) {return;}
        if (IsOccupied)
        {
            return;
        }
        
        TaskList.Add(task);
    }

    public void ClearTask(ShipTask task)
    {
        TaskList.Remove(task);
    }
    public void TrySpawnNewTask(ShipManager manager)
    {
        if (isSpawning || IsFull()) return;

        var availablePlacements = taskRegistry.Values
            .Where(p => !IsTaskActive(p.taskObject.TaskType))
            .ToList();

        if (availablePlacements.Count > 0)
        {
            PrePlacedTask chosen = availablePlacements[UnityEngine.Random.Range(0, availablePlacements.Count)];
            if (chosen.taskObject.TaskType == TaskType.Fire) { return; }
            StartCoroutine(SpawnTaskSequence(chosen));
        }
    }

    private IEnumerator SpawnTaskSequence(PrePlacedTask placement)
    {
        isSpawning = true;

        ShipTask taskInfoProvider = placement.taskObject;
        if (taskInfoProvider == null)
        {
            Debug.LogError($"PrePlacedTask не содержит taskObject");
            isSpawning = false;
            yield break;
        }

        Transform spawnPoint = placement.animationAnchor != null ? placement.animationAnchor : transform;

        GameObject tentacleObj = Instantiate(tentacleAnimatorPrefab, spawnPoint.position, spawnPoint.rotation);
        TentacleAnimator tentacle = tentacleObj.GetComponent<TentacleAnimator>();

       yield return StartCoroutine(placement.taskObject.HandleTentacleSequence(tentacle));

        isSpawning = false;
    }


    public void StartWork(TaskType task)
    {
        TaskList.Find(shipTask => shipTask.TaskType == task)?.StartWork();
    }

    public void StopWork(TaskType task)
    {
        TaskList.Find(shipTask => shipTask.TaskType == task)?.StopFix();
    }

    public bool IsFull()
    {
        int activeCount = TaskList.Count;
        int possibleCount = taskRegistry.Count;
        return activeCount >= possibleCount;
    }

}
