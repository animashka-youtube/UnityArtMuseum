using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunState : PlayerLeafBaseState
{
    private CharacterController _controller;

    public PlayerRunState(PlayerSuperBaseState parentState, PlayerContext playerCtx, PlayerStateMachine hfsm) : base(parentState, playerCtx, hfsm)
    {
    }

    public override void Enter(object data = null)
    {
        base.Enter();

        _controller = _playerCtx.Controller;
        _playerStateMachine.Footsteps.PlayRun(_playerCtx.IsInside);
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        Vector2 moveDirection = _playerCtx.MoveDirection;

        Vector3 dir =
            _controller.transform.forward * moveDirection.y + 
            _controller.transform.right * moveDirection.x;

        _controller.Move(dir.normalized * 4f * dt + Vector3.down * dt);

        if (_playerCtx.MoveDirection == Vector2.zero)
        {
            _parentState.SwitchSubstate<PlayerIdleState>();
        }
    }

    public override void Exit()
    {
        _playerStateMachine.Footsteps.StopSmooth();
    }
}