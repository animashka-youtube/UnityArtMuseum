using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGroundedState : PlayerSuperBaseState
{
    private CharacterController _controller;

    public PlayerGroundedState(PlayerSuperBaseState parentState, PlayerContext playerContext, PlayerStateMachine hfsm) 
        : base(parentState, playerContext, hfsm)
    {
        _subStates = new List<IPlayerState> 
        {
            new PlayerIdleState(this, playerContext, hfsm),
            new PlayerWalkState(this, playerContext, hfsm),
            new PlayerRunState(this, playerContext, hfsm)
        };
        _currentSubState = _subStates[0];
        _currentSubState.Enter();
    }

    public override void Enter(object data = null)
    {
        base.Enter();
        _controller = _playerCtx.Controller;

        var footstep = _playerStateMachine.GetComponentInChildren<FootstepController>();
        if (footstep != null && _playerCtx.MoveDirection != Vector2.zero)
        {
            bool inside = _playerCtx.IsInside;

            if (_playerCtx.IsRunning)
                footstep.PlayRun(inside);
            else
                footstep.PlayWalk(inside);
        }
    }

    public override void Update(float dt)
    {
        Vector2 moveDir = _playerCtx.MoveDirection;

        if (moveDir != Vector2.zero)
        {
            if (_playerCtx.IsRunning)
                SwitchSubstate<PlayerRunState>();
            else
                SwitchSubstate<PlayerWalkState>();
        }
        else
        {
            SwitchSubstate<PlayerIdleState>();
        }

        if (!_controller.isGrounded || (_controller.isGrounded && _playerCtx.IsJumping))
        {
            _parentState.SwitchSubstate<PlayerAirborneState>();
        }

        base.Update(dt);
    }
}