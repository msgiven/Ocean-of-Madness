using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class Cannon : ShipTask
{
    //[Tooltip("Ссылка на объект-подсветку")]
    //public GameObject highlightObject;
    public Transform animationAnchor;

    [Header("Эффекты Выстрела")]
    [Tooltip("Перетащите сюда префаб эффекта взрыва")]
    [SerializeField] private GameObject explosionVFXPrefab;
    [Tooltip("Перетащите сюда дочерний объект 'MuzzlePoint'")]
    [SerializeField] private Transform pointVFX;
    [SerializeField] private SoundDataSO explosionSoundData;

    protected override void Awake()
    {
        base.Awake();
        //highlightObject.SetActive(false);
        failureTime = 10000;
        IsResolved = false;
        if (animationAnchor == null)
        {
            animationAnchor = this.transform;
        }

    }

    public override IEnumerator HandleTentacleSequence(TentacleAnimator tentacle)
    {
        IsResolved = false;
        tentacle.skeletonAnimation.AnimationState.SetAnimation(0, tentacleAnimName.animName, false);
        yield return new WaitForSeconds(taskCreationTiming);
        ActivateTask();
        parentZone.AddTask(this);

        tentacle.skeletonAnimation.AnimationState.TimeScale = 0f;

        yield return new WaitUntil(() => IsResolved);

        soundEmitter.Stop();
        SoundManager.Instance.Get().Initialize(explosionSoundData).Play();

        StopFix();
        if (explosionVFXPrefab != null && pointVFX != null)
        {
            GameObject explosionVFX = Instantiate(explosionVFXPrefab, pointVFX.position, pointVFX.rotation);

            Destroy(explosionVFX, 0.5f);
        }

        tentacle.skeletonAnimation.AnimationState.TimeScale = 1f;

        yield return StartCoroutine(WaitForDeleteTentacle(tentacle));

        ResetTask();
    }

    protected override void Complete()
    {
        if (IsResolved) return;
        IsResolved = true;
        parentZone.ClearTask(this);
        ShipManager.Instance.ChangeHealth(-currentDamage);
        ShipManager.Instance.CollectCoins(coinsForTask);
    }
    public override void ResetTask()
    {
        //highlightObject.SetActive(false);
        ResetVariables();
        IsResolved = false;
    }

}