using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLoadingAsset : MonoBehaviour
{
    public AudioSource _audioSource;

    private void Awake()
    {
        // Get the AudioSource component
        //_audioSource = GetComponent<AudioSource>();
    }
    [ContextMenu("LoadAudio")]
    public void LoadAudio()
    {
        AudioClip clip = Resources.Load<AudioClip>("Audio/Intro_ا");

        if (clip != null)
        {
            _audioSource.clip = clip;
            Debug.Log($"Audio clip '{clip.name}' loaded successfully.");
        }
        else
        {
            Debug.LogError("Audio clip not found! Ensure the file is in the Resources folder and the path is correct.");
        }
    }

    //private void SpwanModel(GameObject charModel)
    //{
        
    //        Animator animator = forwardCurrentCharacterInstance.GetComponent<Animator>();
    //        if (animator == null)
    //        {
    //            animator = forwardCurrentCharacterInstance.AddComponent<Animator>();
    //        }

    //        RuntimeAnimatorController animatorController = Resources.Load<RuntimeAnimatorController>("CharacterAnimation/CharactersAnim_test");
    //        AnimationClip animationClip = Resources.Load<AnimationClip>("CharacterAnimation/Alaaf");

    //        if (animatorController != null && animationClip != null)
    //        {
    //            // Create an Animator Override Controller
    //            AnimatorOverrideController overrideController = new AnimatorOverrideController(animatorController);

    //            // Get the list of animation clips in the original controller
    //            var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
    //            overrideController.GetOverrides(anims);

    //            // Replace the default animation with your new clip
    //            // You might need to adjust the key based on your actual Animator Controller setup
    //            for (int i = 0; i < anims.Count; i++)
    //            {
    //                if (anims[i].Key != null)
    //                {
    //                    overrideController[anims[i].Key] = animationClip;
    //                }
    //            }

    //            // Assign the override controller to the animator
    //            animator.runtimeAnimatorController = overrideController;

    //            Debug.Log("Animation clip added to Animator successfully!");
    //        }
    //        else
    //        {
    //            Debug.LogWarning("Failed to load Animator Controller or Animation Clip!");
    //        }

    //        Debug.Log("Animator and Animation Clip setup completed!");
    //        if (animator != null)
    //        {
    //            animator.runtimeAnimatorController = animatorController;
    //        }
    //        else
    //        {
    //            Debug.LogWarning("No Animator Controller provided for the character.");
    //        }
    //    }
    //}
}
