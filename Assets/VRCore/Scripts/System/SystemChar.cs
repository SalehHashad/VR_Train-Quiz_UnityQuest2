using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelCharacter
{
    public char Character;
    public AudioClip CharacterSound;
    public Sprite charImage;
    public GameObject charFBX;
}

public class SystemChar : MonoBehaviour
{
    public static SystemChar instance;

    public AudioClip correctAnswer;
    public AudioClip wrongAnswer;

    public List<LevelCharacter> englishCharactersData;
    public List<LevelCharacter> arabicCharactersData;
    public List<LevelCharacter> numbersCharacterData;

    public int index = 0;

    [SerializeField] int countWrongAnswer = 0;

    private readonly string englishChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private readonly string arabicChars = "ابتثجحخدذرزسشصضطظعغفقكلمنهوي";
    private readonly string numberChars = "0123456789";

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
                Sprite sprite = Resources.Load<Sprite>($"Sprites/{c}");
                GameObject fbx = Resources.Load<GameObject>($"Models/{c}");

                LevelCharacter newCharacter = new LevelCharacter
                {
                    Character = c,
                    CharacterSound = clip,
                    charImage = sprite,
                    charFBX = fbx
                };

                englishCharactersData.Add(newCharacter);
                CheckAssetsAdded(newCharacter);
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
                Sprite sprite = Resources.Load<Sprite>($"Sprites/{c}");
                GameObject fbx = Resources.Load<GameObject>($"Models/{c}");

                LevelCharacter newCharacter = new LevelCharacter
                {
                    Character = c,
                    CharacterSound = clip,
                    charImage = sprite,
                    charFBX = fbx
                };

                arabicCharactersData.Add(newCharacter);
                CheckAssetsAdded(newCharacter);
            }
            else
            {
                Debug.Log($"{c} already exists in the list.");
            }
        }
    }

    [ContextMenu("Add Number Char to character data")]
    public void AddNumberCharacterData()
    {
        foreach (char c in numberChars)
        {
            if (!numbersCharacterData.Exists(character => character.Character == c))
            {
                AudioClip clip = Resources.Load<AudioClip>($"Audio/{c}");
                Sprite sprite = Resources.Load<Sprite>($"Sprites/{c}");
                GameObject fbx = Resources.Load<GameObject>($"Models/{c}");

                LevelCharacter newCharacter = new LevelCharacter
                {
                    Character = c,
                    CharacterSound = clip,
                    charImage = sprite,
                    charFBX = fbx
                };

                numbersCharacterData.Add(newCharacter);
                CheckAssetsAdded(newCharacter);
            }
            else
            {
                Debug.Log($"{c} already exists in the list.");
            }
        }
    }

    private void CheckAssetsAdded(LevelCharacter character)
    {
        if (character.CharacterSound != null)
            Debug.Log($"{character.Character} has an audio clip: {character.CharacterSound.name}");
        else
            Debug.LogWarning($"{character.Character} is missing an audio clip.");

        if (character.charImage != null)
            Debug.Log($"{character.Character} has a sprite: {character.charImage.name}");
        else
            Debug.LogWarning($"{character.Character} is missing a sprite.");

        if (character.charFBX != null)
            Debug.Log($"{character.Character} has an FBX model: {character.charFBX.name}");
        else
            Debug.LogWarning($"{character.Character} is missing an FBX model.");
    }
}
