using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerLeafBaseState
{
    public PlayerIdleState(PlayerSuperBaseState parentState, PlayerContext playerCtx, PlayerStateMachine hfsm) : base(parentState, playerCtx, hfsm)
    {
    }

    public override void Enter(object data = null)
{
    base.Enter();
    _playerStateMachine.Footsteps.StopSmooth(); // <-- только так
}
}
