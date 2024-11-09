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
public enum GameCategories { englishCharacter, arabicCharacter, englishNumber, arabicNumber }

public class SystemChar : MonoBehaviour
{
    public static SystemChar instance;
    public GameCategories gameCategory = GameCategories.englishCharacter;


    public AudioClip correctAnswer;
    public AudioClip wrongAnswer;

    public static List<LevelCharacter> currentCharactersData = new List<LevelCharacter>();
    public List<LevelCharacter> englishCharactersData;
    public List<LevelCharacter> arabicCharactersData;
    public List<LevelCharacter> englishNumberCharacterData;
    public List<LevelCharacter> arabicNumberCharacterData;

    public int index = 0;

    [SerializeField] int countWrongAnswer = 0;

    private readonly string englishChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private readonly string arabicChars = "ابتثجحخدذرزسشصضطظعغفقكلمنهوي";
    private readonly string englishNumberChars = "0123456789";
    private readonly string arabicNumberChars = "٠١٢٣٤٥٦٧٨٩";

    private void Awake()
    {
        instance = this;
        UpdateCurrentCharData(gameCategory);
    }
    
    public void UpdateCurrentCharData(GameCategories _gameCategory)
    {
        if(currentCharactersData.Count > 0)
            currentCharactersData.Clear();

        gameCategory = _gameCategory;
        
        switch (_gameCategory)
        {
            case GameCategories.englishCharacter:
                currentCharactersData = englishCharactersData;
                break;
            case GameCategories.englishNumber:
                currentCharactersData = englishNumberCharacterData;
                break;
            case GameCategories.arabicCharacter:
                currentCharactersData = arabicCharactersData;
                break;
            case GameCategories.arabicNumber:
                currentCharactersData = arabicNumberCharacterData;
                break;

        }
    }
    public LevelCharacter UpdateNextQuestion()
    {
        if (index >= currentCharactersData.Count)
        {
            throw new IndexOutOfRangeException("Index is out of range for currentCharactersData.");
        }

        LevelCharacter m_levelCharacter = currentCharactersData[index];
        index++;
        return m_levelCharacter;
    }


    #region ContextMenu
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

    [ContextMenu("Add English Number Char to character data")]
    public void AddEnglishNumberCharacterData()
    {
        foreach (char c in englishNumberChars)
        {
            if (!englishNumberCharacterData.Exists(character => character.Character == c))
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

                englishNumberCharacterData.Add(newCharacter);
                CheckAssetsAdded(newCharacter);
            }
            else
            {
                Debug.Log($"{c} already exists in the list.");
            }
        }
    }

    [ContextMenu("Add Arabic Number Char to character data")]
    public void AddArabicNumberCharacterData()
    {
        foreach (char c in arabicNumberChars)
        {
            if (!arabicNumberCharacterData.Exists(character => character.Character == c))
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

                arabicNumberCharacterData.Add(newCharacter);
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
    #endregion
 }
