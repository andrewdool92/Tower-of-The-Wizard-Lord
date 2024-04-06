using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _Instance;
    public static AudioManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new GameObject().AddComponent<AudioManager>();
                _Instance.name = _Instance.GetType().ToString();
                DontDestroyOnLoad(_Instance.gameObject);
            }
            return _Instance;
        }
    }

    public void playSoundClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        GameObject audioObject = new GameObject("SoundFXObject");

        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }

    public void playRandomClip(AudioClip[] audioClips, Transform spawnTransform, float volume)
    {
        if (audioClips.Length > 0)
        {
            int rand = Random.Range(0, audioClips.Length);
            playSoundClip(audioClips[rand], spawnTransform, volume);
        }
    }

    public AudioSource createPersistentAudioSource(AudioClip audioClip, Transform spawnTransform)
    {
        GameObject audioObject = new GameObject("LoopingFXObject");
        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        
        audioSource.playOnAwake = false;
        audioSource.clip = audioClip;

        return audioSource;
    }

    public static IEnumerator AudioFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = 0;
        while (currentTime < duration && audioSource != null)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime/duration);
            yield return null;
        }
        yield break;
    }

    public void fadeInAudio(AudioSource audioSource, float fadeDuration, float volume, bool randomStart=false)
    {
        audioSource.volume = 0;
        audioSource.time = randomStart ? Random.Range(0, audioSource.clip.length) : 0;

        audioSource.Play();
        StartCoroutine(AudioFade(audioSource, fadeDuration, volume));
    }

    public void fadeAudio(AudioSource audioSource, float duration)
    {
        StartCoroutine(AudioFade(audioSource, duration, 0));
    }
}

