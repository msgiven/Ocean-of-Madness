using UnityEngine;

public class FailState : State
{
    private float timer = 1.5f; 
    public FailState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

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
        if (timer > 0) { 
            timer -= Time.deltaTime;
        }
        else
        {
            player.stateMachine.ChangeState(player.idleState);
        }
    }
}
