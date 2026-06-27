using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerExploreState : PlayerSuperBaseState
{
    private Camera _camera;
    private CharacterController _characterController;

    private float _yaw;
    private float _pitch;

    public PlayerExploreState(PlayerContext playerContext, PlayerStateMachine hfsm) : base(playerContext, hfsm)
    {
        _subStates = new List<IPlayerState>
        {
            new PlayerGroundedState(this, playerContext, hfsm),
            new PlayerAirborneState(this, playerContext, hfsm)
        };
        _currentSubState = _subStates[0];
        _currentSubState.Enter();
    }

    public override void Enter(object data = null)
    {
        base.Enter();
        _camera = _playerCtx.Camera;
        _characterController = _playerCtx.Controller;

        Cursor.lockState = CursorLockMode.Locked;

        SwitchSubstate<PlayerGroundedState>();

        var footstep = _playerStateMachine.GetComponentInChildren<FootstepController>();
        footstep?.SetPlayerContext(_playerCtx);

    }

    public override void Update(float dt)
    {
        base.Update(dt);

        Vector2 mouseDelta = _playerCtx.MouseDelta;

        _yaw += mouseDelta.x * 10f * dt;
        _pitch -= mouseDelta.y * 10f * dt;
        _pitch = Mathf.Clamp(_pitch, -80f, 80f);

        _camera.transform.localRotation = Quaternion.Euler(_pitch, 0, 0);
        _characterController.transform.rotation = Quaternion.Euler(0, _yaw, 0);
    }
}
