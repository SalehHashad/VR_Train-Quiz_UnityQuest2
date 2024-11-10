using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance => instance;

    private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
            Destroy(gameObject);
    }

    public void PlaySound(AudioClip clip, float delay = 0f)
    {
        if (clip == null) return;

        if (delay > 0)
            StartCoroutine(PlaySoundDelayed(clip, delay));
        else
            audioSource.PlayOneShot(clip);
    }

    private IEnumerator PlaySoundDelayed(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.PlayOneShot(clip);
    }
}
