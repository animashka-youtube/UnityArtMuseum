using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IPlayerState
{
    void Enter(object data = null); // Добавьте это (data для obj)
    void Exit();
    void Update(float dt);
}