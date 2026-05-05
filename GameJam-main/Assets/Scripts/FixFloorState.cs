using UnityEngine;

public class FixFloorState : State
{
    public FixFloorState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {

        player.currentZone = player.nextZone;

        player.skeletonAnimation.AnimationState.SetAnimation(0, animBoolName, true);
        ShipManager.Instance.StartWorkInZone(
            TaskType.FloorHole, player.currentZone);
        base.Enter();
    }

    public override void Exit()
    {
        ShipManager.Instance.StopWorkInZone(
            TaskType.FloorHole, player.currentZone);
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (!player.TaskAvailable())
        {
            player.stateMachine.ChangeState(player.idleState);
        }
    }
}
