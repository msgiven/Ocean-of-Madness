using Spine.Unity;
using System.Collections;
using UnityEngine;

public class TentacleAnimator : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation { get; private set; }

    protected string stateName;
    void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }

}
