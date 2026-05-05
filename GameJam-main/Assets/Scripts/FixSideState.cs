using UnityEngine;

public class FixSideState : State
{
    public FixSideState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {

        player.currentZone = player.nextZone;

        player.animatorEffects.gameObject.SetActive(true);

        player.skeletonAnimation.AnimationState.SetAnimation(0, animBoolName, true);
        ShipManager.Instance.StartWorkInZone(
            TaskType.SideHole, player.currentZone);
        base.Enter();
    }

    public override void Exit()
    {
        player.animatorEffects.gameObject.SetActive(false);
        //player.skeletonAnimation.AnimationState.SetAnimation(0, "bort", false);
        ShipManager.Instance.StopWorkInZone(
            TaskType.SideHole, player.currentZone);
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (!player.TaskAvailable())
        {
            player.stateMachine.ChangeState(player.idleState);
            player.ClearCurrentTask(); 
        }
    }

 
}
