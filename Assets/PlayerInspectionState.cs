using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
public class PlayerInspectionState : PlayerSuperBaseState
{
    private Interactable currentObj;
    private PlayerInteraction interaction;
    private Camera playerCamera;
    private float currentZoom;
    private float zoomSpeed = 2f;
    private Vector3 originalCamLocalPos;
    private Quaternion originalCamLocalRot;
    private const float focusTime = 0.8f;
    private float originalFOV;

    public PlayerInspectionState(PlayerContext playerContext, PlayerStateMachine playerStateMachine)
        : base(playerContext, playerStateMachine)
    {
        _subStates = new List<IPlayerState>();
    }

    private void SetPlayerModelVisible(bool visible)
{
    if (_playerCtx?.PlayerModel == null)
        return;

    foreach (var renderer in _playerCtx.PlayerModel.GetComponentsInChildren<Renderer>())
        renderer.enabled = visible;
}

public override void Enter(object data = null)
{
    base.Enter();

    interaction = Object.FindObjectOfType<PlayerInteraction>();
    if (interaction != null)
    {
        interaction.SetInspectionMode(true);  // пометить, что мы в режиме инспекции
        interaction.HidePrompt();            // прячем подсказку, но НЕ стираем текст/панель
    }

    currentObj = data as Interactable;
    if (currentObj == null)
    {
        _playerStateMachine.SwitchPlayerState<PlayerExploreState>();
        return;
    }
    if (interaction != null && currentObj.AudioClip != null) // блок аудио
    {
        interaction.AudioSource.clip = currentObj.AudioClip;
        interaction.AudioSource.volume = 0.75f; // вот тут громкость 75%
        interaction.AudioSource.Play();
        AudioManager.Instance?.FadeBGM(0.2f, 0.4f);  // громкость 20% за 0.4 сек // приглушаем музыку
    }

    // Входная логика
    currentZoom = currentObj.OrbitDistance;
    playerCamera = _playerCtx.Camera;
    
    originalFOV = playerCamera.fieldOfView;
    originalCamLocalPos = playerCamera.transform.localPosition;
    originalCamLocalRot = playerCamera.transform.localRotation;

    // отключаем контроллер перед анимацией камеры
    _playerCtx.Controller.enabled = false;

    SetPlayerModelVisible(false);
}



public override void Update(float dt)
{
    base.Update(dt);

    // --- Плавный зум колёсиком мыши (Input.GetAxis сглаживает рывки) ---
    float scroll = Input.GetAxis("Mouse ScrollWheel");
    if (Mathf.Abs(scroll) > 0.01f)
    {
        playerCamera.fieldOfView -= scroll * zoomSpeed * 10f;  // Умножитель для чувствительности
        playerCamera.fieldOfView = Mathf.Clamp(playerCamera.fieldOfView, 15f, 90f); // Настройте пределы
    }

    // --- Камера смотрит на объект под углом 10° сверху ---
    Vector3 lookDirection = (currentObj.transform.position - playerCamera.transform.position).normalized;
    
    // Поднимаем точку обзора чуть выше центра объекта
    Vector3 targetPoint = currentObj.transform.position + Vector3.up * 0.2f;

    // Поворачиваем камеру с небольшим наклоном вниз (10° от горизонтали)
    Quaternion targetRotation = Quaternion.LookRotation(targetPoint - playerCamera.transform.position);
    Quaternion tiltedRotation = targetRotation * Quaternion.Euler(10f, 0f, 0f); // 10° сверху

    playerCamera.transform.rotation = Quaternion.Slerp(
        playerCamera.transform.rotation,
        tiltedRotation,
        dt * 8f  // Плавность поворота
    );

    // --- Выход из режима ---
    if ((_playerCtx != null && _playerCtx.InteractPressed) || Input.GetKeyDown(KeyCode.Escape))
    {
        _playerCtx?.ConsumeInteract();
        _playerStateMachine.SwitchPlayerState<PlayerExploreState>();
    }
}



public override void Exit()
{
    if (playerCamera != null)
    {
        playerCamera.transform.localPosition = originalCamLocalPos;
        playerCamera.transform.localRotation = originalCamLocalRot;
    }

    SetPlayerModelVisible(true);
    _playerCtx.Controller.enabled = true;
    playerCamera.fieldOfView = originalFOV;

    if (currentObj != null && currentObj.AudioSource != null)
    {
        if (currentObj.AmbientSource != null)
        currentObj.AmbientSource.Pause();
    }

    if (interaction != null)
    {
        interaction.SetInspectionMode(false);
        interaction.ExitInspection(); // закрываем панель, очищаем текст и currentInteractable
    }

    interaction?.FadeOutSound();
    AudioManager.Instance?.FadeBGM(0.5f, 0.3f);  // возвращаем к 50% за 0.3 сек

    var footstep = _playerStateMachine.GetComponentInChildren<FootstepController>();
    footstep?.SetPlayerContext(_playerCtx);

    base.Exit();
}


}