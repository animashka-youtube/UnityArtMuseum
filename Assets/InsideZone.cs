using System.Collections;
using UnityEngine;

public class InsideZone : MonoBehaviour
{
    public AudioSource audioSource;
    public float fadeDuration = 1f; // длительность затухания
    private Coroutine fadeRoutine;

    private void OnTriggerEnter(Collider other)
    {
        var sm = other.GetComponent<PlayerStateMachine>();
        if (sm != null)
        {
            sm.PlayerContext.IsInside = true;
            // плавное затухание
            StartFade(0f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var sm = other.GetComponent<PlayerStateMachine>();
        if (sm != null)
        {
            sm.PlayerContext.IsInside = false;
            // плавное увеличение громкости
            StartFade(1f);
        }
    }

    private void StartFade(float targetVolume)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeVolume(targetVolume));
    }

    private IEnumerator FadeVolume(float targetVolume)
    {
        float startVolume = audioSource.volume;
        float t = 0f;

        // если звук ещё не играет и нужно увеличить громкость — запускаем
        if (!audioSource.isPlaying && targetVolume > 0f)
            audioSource.Play();

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;

        // если цель — 0, останавливаем звук
        if (targetVolume == 0f)
            audioSource.Stop();
    }
}
