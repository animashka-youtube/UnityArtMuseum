using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    [Header("Секретный пазл")]
    [SerializeField] private GameObject boxToHide;
    [SerializeField] private GameObject exhibitToShow;

    [Header("UI уведомления")]
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private GameObject notificationPanel;

    private int progress = 0;                     // ← Вот новое поле
    private int[] correctSequence = { 1, 2, 3 };  // Правильный порядок

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterInteraction(int id)
    {
        if (id == 0) return;

        // Нажат правильный объект
        if (id == correctSequence[progress])
        {
            progress++;

            // Последний шаг — паззл решён
            if (progress >= correctSequence.Length)
            {
                OpenSecret();
                progress = 0;
            }
        }
        else
        {
            // Ошибка — сброс
            progress = 0;

            // Если клик соответствует первому элементу —
            // начинаем последовательность заново
            if (id == correctSequence[0])
            {
                progress = 1;
            }
        }
    }

    private void OpenSecret()
    {

        if (boxToHide != null) boxToHide.SetActive(false);
        if (exhibitToShow != null) exhibitToShow.SetActive(true);

        ShowNotification("Секретный объект открыт!");
    }

    private void ShowNotification(string message)
    {
        if (notificationText != null && notificationPanel != null)
        {
            notificationText.text = message;
            notificationPanel.SetActive(true);
            StartCoroutine(HideNotification(5f));
        }
    }

    private IEnumerator HideNotification(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (notificationPanel != null)
            notificationPanel.SetActive(false);
    }
}
