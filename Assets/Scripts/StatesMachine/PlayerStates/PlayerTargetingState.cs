using UnityEngine;

public class PlayerTargetingState : PlayerBaseState
{
    private bool _isTargeting;
    public PlayerTargetingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.InputReader.TargetEvent += OnTarget;

    }


    public override void Tick(float deltaTime)
    {
        Debug.Log(stateMachine.Targeter.CurrentTarget.name);
    }

    public override void Exit()
    {
        stateMachine.InputReader.TargetEvent -= OnTarget;

    }
    
    private void OnTarget(bool isTargeting)
    {
        if (!isTargeting)
        {
            stateMachine.Targeter.CancelTarget();
            stateMachine.SwitchState(new PlayerMoveState(stateMachine));
        }
    }
}
