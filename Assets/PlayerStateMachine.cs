using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    private IPlayerState _currentPlayerState;
    private List<IPlayerState> _playerStates = new List<IPlayerState>();
    private PlayerContext _playerContext;
    public PlayerContext PlayerContext => _playerContext;
    public PlayerContext Context => _playerContext;

    [Header("Ссылки — перетащить в Inspector")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Transform ceilingCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private GameObject playerModel;
    [SerializeField] private FootstepController footsteps;
    public FootstepController Footsteps => footsteps;


    private void Awake()
    {
 _playerContext = new PlayerContext(controller, playerCamera, playerInput);

    // ДОБАВИТЬ:
    _playerContext.CeilingCheck = ceilingCheck;   // перетащить в инспекторе
    _playerContext.GroundMask = groundMask;
    _playerContext.PlayerModel = playerModel;

    _playerStates.Clear();
    _playerStates.Add(new PlayerExploreState(_playerContext, this));
    _playerStates.Add(new PlayerInspectionState(_playerContext, this));

    SwitchPlayerState<PlayerExploreState>();
    }
    

    private void Update()
    {
        _currentPlayerState?.Update(Time.deltaTime);
    }

    // Переход без передачи данных
    public void SwitchPlayerState<T>() where T : IPlayerState
    {
        SwitchPlayerState<T>(null);
    }

    // Переход с передачей данных (например, текущий Interactable)
    public void SwitchPlayerState<T>(object data) where T : IPlayerState
    {
        _currentPlayerState?.Exit();

        // ВАЖНО: включаем контроллер перед входом в новое состояние, если он есть
        if (_playerContext.Controller != null && !_playerContext.Controller.enabled)
            _playerContext.Controller.enabled = true;

        _currentPlayerState = _playerStates.FirstOrDefault(s => s is T);

        if (_currentPlayerState == null)
        {
            Debug.LogError($"Состояние {typeof(T).Name} не найдено в списке состояний!");
            return;
        }

        _currentPlayerState.Enter(data);
    }
}