using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Перетащить в инспекторе")]
    [SerializeField] private Camera playerCamera;          
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private TextMeshProUGUI descriptionText;
    public float OrbitDistance = 1f;   // начальная дистанция
    public float MinZoom = 0.8f;       // минимальный зум
    public float MaxZoom = 3f;         // максимальный зум
    [SerializeField] private float interactDistance = 3f;
    private PlayerStateMachine playerStateMachine;
    private AudioSource audioSource;
    private Interactable currentInteractable;
    private PlayerContext _playerCtx;
    private bool isInInspectionMode = false;
    public void SetInspectionMode(bool state) => isInInspectionMode = state;
    public bool IsInInspectionMode() => isInInspectionMode;
    public AudioSource AudioSource => audioSource;


    public void FadeOutSound(float duration = 0.4f)
    {
        if (audioSource == null || !audioSource.isPlaying)
            return;

        StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = audioSource.volume;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }


    // Скрывает только подсказку (не трогает панель описания и сам текст)
    public void HidePrompt()
    {
        if (promptText) promptText.gameObject.SetActive(false);
    }

    // Полностью скрывает UI описания (используется при окончательном выходе)
    public void ExitInspection()
    {
        isInInspectionMode = false;
        if (descriptionPanel) descriptionPanel.SetActive(false);
        if (descriptionText) descriptionText.text = "";
        if (promptText) promptText.gameObject.SetActive(false);
        currentInteractable = null;
    }

    public void ClearCurrent()
    {
        currentInteractable = null;
        // не трогаем панель здесь — ExitInspection отвечает за это
    }

    private void Awake()
    {

        playerStateMachine = GetComponent<PlayerStateMachine>();
        
        if (playerStateMachine != null)
        _playerCtx = playerStateMachine.PlayerContext;

        // Если не перетащили — пытаемся найти сами
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (playerCamera == null)
            playerCamera = FindObjectOfType<Camera>();

        if (playerCamera == null)
            Debug.LogError("PlayerInteraction: Камера не найдена! Перетащи её в поле Player Camera");

         audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D звук
    }

    private void Start()
    {
        playerStateMachine = GetComponent<PlayerStateMachine>();
    _playerCtx = playerStateMachine.PlayerContext;

        if (promptText) promptText.gameObject.SetActive(false);
        if (descriptionPanel) descriptionPanel.SetActive(false);
    }

    private void Update()
    {
        // Если уже в режиме осмотра — не обрабатываем наведение/интеракт
    if (isInInspectionMode || playerCamera == null || _playerCtx == null) return;

    DetectInteractable();

    // Используем новый InputSystem через контекст
    if (_playerCtx.InteractPressed && currentInteractable != null)
    {
        // Сразу потребляем нажатие, чтобы оно не "просочилось" в другие обработчики
        _playerCtx.ConsumeInteract();
        EnterInspection();
    }
    }

private void DetectInteractable()
{
    if (isInInspectionMode)
    {
        promptText.gameObject.SetActive(false);
        return;
    }

    Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

    if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
    {
        currentInteractable = hit.collider.GetComponent<Interactable>();
    }
    else
    {
        currentInteractable = null;
    }

    promptText.gameObject.SetActive(currentInteractable != null);
}


private void EnterInspection()
{
    isInInspectionMode = true;

    descriptionPanel.SetActive(true);
    descriptionText.text = currentInteractable.Description;

    // ← ИСПРАВЛЕНО: было obj → теперь currentInteractable!
    PuzzleManager.Instance?.RegisterInteraction(currentInteractable.PuzzleId);

    playerStateMachine.SwitchPlayerState<PlayerInspectionState>(currentInteractable);
}


}