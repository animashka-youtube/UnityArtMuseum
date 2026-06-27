using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FootstepController : MonoBehaviour
{
    [Header("Клипы")]
    public AudioClip walkOutside;
    public AudioClip walkInside;
    public AudioClip runOutside;
    public AudioClip runInside;

    [Header("Настройки громкости и плавности")]
    public float fadeDuration = 0.4f;
    public float targetVolume = 1f;

    private AudioSource src;
    private Coroutine fadeRoutine;

    private PlayerContext _playerCtx;

    public void SetPlayerContext(PlayerContext ctx)
    {
        _playerCtx = ctx;
    }

    private void Update()
    {
        if (_playerCtx == null) return;

        if (_playerCtx.MoveDirection != Vector2.zero)
        {
            if (_playerCtx.IsRunning)
                PlayRun(_playerCtx.IsInside);
            else
                PlayWalk(_playerCtx.IsInside);
        }
        else
        {
            StopSmooth();
        }
    }

    private void Awake()
    {
        src = GetComponent<AudioSource>();
    if (src == null)
    {
        Debug.LogError("FootstepController требует AudioSource на этом объекте!");
        enabled = false; // отключаем скрипт, чтобы не падал
        return;
    }
    src.loop = true;
    src.playOnAwake = false;
    }

    // Главный метод — безопасно переключает клип
    private void PlayClip(AudioClip clip)
{
    if (clip == null) return;

    if (src.clip != clip)
    {
        src.clip = clip;
        src.Play();
    }
    // Если клип тот же — просто продолжаем играть (loop = true)

    StartFade(targetVolume);
}

    public void PlayWalk(bool inside)
    {
        PlayClip(inside ? walkInside : walkOutside);
    }

    public void PlayRun(bool inside)
    {
        PlayClip(inside ? runInside : runOutside);
    }

    public void StopSmooth()
    {
        if (src == null) return;
        StartFade(0f);
    }

    // --- ПЛАВНОСТЬ ---
    private void StartFade(float to)
    {
        if (src == null) return; // безопасно
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(Fade(to));
    }

    private IEnumerator Fade(float target)
    {
        float start = src.volume;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            src.volume = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }

        src.volume = target;

        if (target == 0f)
            src.Stop();
    }

    public void Stop()
    {
        if (src.isPlaying)
            StartFade(0f);
    }
    

}