using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLeafBaseState : IPlayerState
{
    protected PlayerSuperBaseState _parentState;

    protected PlayerContext _playerCtx;
    protected PlayerStateMachine _playerStateMachine;

    public PlayerLeafBaseState(PlayerSuperBaseState parentState, PlayerContext playerCtx, PlayerStateMachine hfsm)
    {
        _parentState = parentState;
        _playerCtx = playerCtx;
        _playerStateMachine = hfsm;
    }

    public virtual void Enter(object data = null)
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void FixedUpdate(float dt)
    {
    }

    public virtual void Update(float dt)
    {
    }
}
