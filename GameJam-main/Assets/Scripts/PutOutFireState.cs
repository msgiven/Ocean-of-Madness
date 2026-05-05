using UnityEngine;

public class PutOutFireState : State
{
    public PutOutFireState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        player.currentZone = player.nextZone;

        /*player.shipManager.StartWorkInZone(
            TaskType.Fire, player.currentZone - 1);*/
        base.Enter();
    }

    public override void Exit()
    {
        /*player.shipManager.StopWorkInZone(
            //TaskType.Fire, player.currentZone - 1);*/
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }
}
