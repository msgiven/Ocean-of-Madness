using UnityEngine;

public class IdleState : State
{
    public IdleState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName) { }
    public override void Enter()
    {
        player.skeletonAnimation.AnimationState.SetAnimation(0, animBoolName, true);
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }
}
