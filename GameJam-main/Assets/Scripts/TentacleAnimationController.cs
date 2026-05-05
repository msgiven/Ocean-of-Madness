using Spine.Unity;
using UnityEngine;

public class TentacleAnimationController : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    [SpineAnimation] private string hit = "Udar";
    void Start()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, hit, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
