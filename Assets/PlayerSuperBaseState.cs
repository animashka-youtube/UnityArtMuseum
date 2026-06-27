using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerSuperBaseState : IPlayerState
{
    protected PlayerSuperBaseState _parentState;
    protected IPlayerState _currentSubState;
    protected List<IPlayerState> _subStates;
    protected PlayerContext _playerCtx;
    protected PlayerStateMachine _playerStateMachine;

    public PlayerSuperBaseState(PlayerContext playerCtx, PlayerStateMachine playerStateMachine)
    {
        _playerCtx = playerCtx;
        _playerStateMachine = playerStateMachine;
    }

    public PlayerSuperBaseState(PlayerSuperBaseState parentState, PlayerContext playerContext, PlayerStateMachine hfsm)
    : this(playerContext, hfsm)
    {
        _parentState = parentState;
    }
    public virtual void Enter(object data = null)
    {
        _currentSubState?.Enter();
    }
    public virtual void Exit()
    {
        _currentSubState?.Exit();
    }

    public virtual void Update(float dt)
    {
        _currentSubState?.Update(dt);
    }
    public void SwitchSubstate<T>() where T : IPlayerState
    {
        _currentSubState?.Exit();
        _currentSubState = _subStates.FirstOrDefault(substate => substate is T);
        _currentSubState.Enter();
    }
}