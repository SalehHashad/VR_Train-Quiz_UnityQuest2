using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
            audioSource = gameObject.GetComponent<AudioSource>();
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
        {
            audioSource.clip = clip;
            audioSource.loop = false;
            audioSource.Play();
        }
            //audioSource.PlayOneShot(clip);
    }

    private IEnumerator PlaySoundDelayed(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.PlayOneShot(clip);
    }

    public void PlayLetterIntro(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.loop=true;
        audioSource.Play();
    }
    public void StopingAudio()
    {
        audioSource.Stop();
    }
}
