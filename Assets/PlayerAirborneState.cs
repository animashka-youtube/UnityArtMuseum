using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerAirborneState : PlayerLeafBaseState
{
    public PlayerAirborneState(PlayerSuperBaseState parentState, PlayerContext playerContext, PlayerStateMachine hfsm) : base(parentState, playerContext, hfsm)
    {
    }

    public override void Enter(object data = null)
    {
        base.Enter();

        if(_playerCtx.IsJumping)
        {
            _playerCtx.VerticalVelocity = 5;
        }

        // Останавливаем шаги в полёте
        _playerStateMachine.GetComponentInChildren<FootstepController>()?.Stop();
    }

    public override void Update(float dt)
    {
    base.Update(dt);

    Vector2 moveDir = _playerCtx.MoveDirection;

    // Гравитация
    _playerCtx.VerticalVelocity -= 9.81f * dt;

    // === Проверка потолка (только когда летим вверх) ===
    if (_playerCtx.VerticalVelocity > 0f)
    {
        if (Physics.Raycast(_playerCtx.CeilingCheck.position, Vector3.up, 
            out _, 0.15f, _playerCtx.GroundMask))
        {
            _playerCtx.VerticalVelocity = 0f;   // мгновенно останавливаем подъём
        }
    }

    Vector3 horizontal = _playerCtx.Controller.transform.forward * moveDir.y +
                         _playerCtx.Controller.transform.right * moveDir.x;

    _playerCtx.Controller.Move(horizontal.normalized * 2f * dt + 
                              Vector3.up * _playerCtx.VerticalVelocity * dt);

    if (_playerCtx.Controller.isGrounded)
    {
        _parentState.SwitchSubstate<PlayerGroundedState>();
    }

    if (_playerCtx.Controller == null)
{
    Debug.LogError("Controller = NULL !!!");
    return;
}

if (!_playerCtx.Controller.enabled)
{
    Debug.LogWarning("Controller is DISABLED during AirborneState!");
}
    }



    public override void Exit()
    {
        _playerCtx.VerticalVelocity = 0;
    }
}
