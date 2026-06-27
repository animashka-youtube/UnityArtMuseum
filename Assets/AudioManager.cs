using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource bgmSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FadeBGM(float targetVolume, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(targetVolume, duration));
    }

    private IEnumerator FadeRoutine(float target, float time)
    {
        float start = bgmSource.volume;
        float t = 0;

        while (t < time)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(start, target, t / time);
            yield return null;
        }
    }
}
