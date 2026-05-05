using Spine.Unity;
using UnityEngine;
using UnityEngine.Windows;

public abstract class State
{
    SoundManager soundManager;

    protected StateMachine stateMachine;
    protected string animBoolName;
    protected SkeletonAnimation animator;
    protected Player player;

    public State(Player player, StateMachine stateMachine, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
        animator = player.skeletonAnimation;

    }
    public virtual void Enter()
    {
        //animator.AnimationState.SetAnimation(0, animBoolName, true); 

    }

    public virtual void Update()
    {

    }
    public virtual void Exit()
    {
        //animator.AnimationState.ClearTrack(0);
    }

}
