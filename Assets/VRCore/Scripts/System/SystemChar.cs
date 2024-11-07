using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelCharacter
{
    public char Character;
    public AudioClip CharacterSound;
}

public class SystemChar : MonoBehaviour
{
    public static SystemChar instance;

    public List<LevelCharacter> englishCharactersData;
    public List<LevelCharacter> arabicCharactersData;
    public int index = 0;

    private readonly string englishChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private readonly string arabicChars = "ابتثجحخدذرزسشصضطظعغفقكلمنهوي";
    private void Awake()
    {
        instance = this;
    }
    public char GetEnglishChar()
    {
        if (index >= 0 && index < englishChars.Length)
        {
            char selectedChar = englishChars[index];
            print(selectedChar + " Is Selected");
            index++;
            return selectedChar;
        }
        throw new IndexOutOfRangeException("Index is out of range for English characters.");
    }

    public char GetArabicChar()
    {
        if (index >= 0 && index < arabicChars.Length)
        {
            char selectedChar = arabicChars[index];
            print(selectedChar + " Is Selected");
            index++;
            return selectedChar;
        }
        throw new IndexOutOfRangeException("Index is out of range for Arabic characters.");
    }

    [ContextMenu("Add English Char to character data")]
    public void AddEnglishCharacterData()
    {
        foreach (char c in englishChars)
        {
            if (!englishCharactersData.Exists(character => character.Character == c))
            {
                AudioClip clip = Resources.Load<AudioClip>($"Audio/{c}");
                LevelCharacter newCharacter = new LevelCharacter
                {
                    Character = c,
                    CharacterSound = clip
                };
                englishCharactersData.Add(newCharacter);
                CheckAudioClipAdded(newCharacter);
            }
            else
            {
                Debug.Log($"{c} already exists in the list.");
            }
        }
    }

    [ContextMenu("Add Arabic Char to character data")]
    public void AddArabicCharacterData()
    {
        foreach (char c in arabicChars)
        {
            if (!arabicCharactersData.Exists(character => character.Character == c))
            {
                AudioClip clip = Resources.Load<AudioClip>($"Audio/{c}");
                LevelCharacter newCharacter = new LevelCharacter
                {
                    Character = c,
                    CharacterSound = clip
                };
                arabicCharactersData.Add(newCharacter);
                CheckAudioClipAdded(newCharacter);
            }
            else
            {
                Debug.Log($"{c} already exists in the list.");
            }
        }
    }

    private void CheckAudioClipAdded(LevelCharacter character)
    {
        if (character.CharacterSound != null)
        {
            Debug.Log($"{character.Character} has an audio clip: {character.CharacterSound.name}");
        }
        else
        {
            Debug.LogWarning($"{character.Character} is missing an audio clip.");
        }
    }
}
