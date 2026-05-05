using UnityEngine;

public class ShootState : State
{
    public ShootState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {

        player.skeletonAnimation.AnimationState.SetAnimation(0, animBoolName, true);

        player.currentZone = player.nextZone;

        ShipManager.Instance.StartWorkInZone(TaskType.Gun, player.currentZone);
        base.Enter();
    }

    public override void Exit()
    {
        ShipManager.Instance.StopWorkInZone(TaskType.Gun, player.currentZone);
        base.Exit();
    }

    public override void Update()
    {
        if (!player.TaskAvailable())
        { 
            player.stateMachine.ChangeState(player.idleState);
            player.ClearCurrentTask();
        }
        base.Update();
    }
}
