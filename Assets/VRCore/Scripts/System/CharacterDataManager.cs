using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public enum GameCategory {none, EnglishCharacter, ArabicCharacter, EnglishNumber, ArabicNumber }

public class CharacterDataManager : MonoBehaviour, ICharacterDataProvider
{
    private static CharacterDataManager instance;
    public static CharacterDataManager Instance => instance;

    [SerializeField] private AudioClip correctAnswerSound;
    [SerializeField] private AudioClip wrongAnswerSound;
    public GameCategory currentCategory = GameCategory.EnglishCharacter;

    [SerializeField] private List<LevelCharacter> englishCharactersData = new List<LevelCharacter>();
    [SerializeField] private List<LevelCharacter> arabicCharactersData = new List<LevelCharacter>();
    [SerializeField] private List<LevelCharacter> englishNumberData = new List<LevelCharacter>();
    [SerializeField] private List<LevelCharacter> arabicNumberData = new List<LevelCharacter>();

    private readonly string englishChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private readonly string arabicChars = "ابتثجحخدذرزسشصضطظعغفقكلمنهوي";
    private readonly string englishNumbers = "0123456789";
    private readonly string arabicNumbers = "٠١٢٣٤٥٦٧٨٩";

    private List<LevelCharacter> CurrentData => GetCurrentCategoryData();
    public int currentIndex = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        ValidateCurrentResources();
    }

    public void LoadCharacterResources()
    {
        Debug.Log("Loading all character resources...");

        englishCharactersData.Clear();
        arabicCharactersData.Clear();
        englishNumberData.Clear();
        arabicNumberData.Clear();

        foreach (char c in englishChars)
        {
            AddCharacterToList(c, englishCharactersData);
        }

        foreach (char c in arabicChars)
        {
            AddCharacterToList(c, arabicCharactersData);
        }

        foreach (char c in englishNumbers)
        {
            AddCharacterToList(c, englishNumberData);
        }

        foreach (char c in arabicNumbers)
        {
            AddCharacterToList(c, arabicNumberData);
        }

        Debug.Log("Finished loading all character resources");
        ValidateAllResources();
    }

    public void SetCategory(GameCategory category)
    {
        currentCategory = category;
        currentIndex = 0;
        ValidateCurrentResources();
    }

    private List<LevelCharacter> GetCurrentCategoryData()
    {
        return currentCategory switch
        {
            GameCategory.EnglishCharacter => englishCharactersData,
            GameCategory.ArabicCharacter => arabicCharactersData,
            GameCategory.EnglishNumber => englishNumberData,
            GameCategory.ArabicNumber => arabicNumberData,
            _ => englishCharactersData
        };
    }

    public char GetNextCharacter()
    {
        if (currentIndex >= CurrentData.Count)
            throw new System.IndexOutOfRangeException("No more characters available");

        return CurrentData[currentIndex++].Character;
    }

    public LevelCharacter GetCharacterData(char character)
    {
        return CurrentData.Find(x => x.Character == character);
    }

    #region Context Menu Functions
    [ContextMenu("Add English Characters")]
    private void AddEnglishCharacters()
    {
        foreach (char c in englishChars)
        {
            if (!englishCharactersData.Exists(x => x.Character == c))
            {
                AddCharacterToList(c, englishCharactersData);
            }
        }
    }

    [ContextMenu("Add Arabic Characters")]
    private void AddArabicCharacters()
    {
        foreach (char c in arabicChars)
        {
            if (!arabicCharactersData.Exists(x => x.Character == c))
            {
                AddCharacterToList(c, arabicCharactersData);
            }
        }
    }

    [ContextMenu("Add English Numbers")]
    private void AddEnglishNumbers()
    {
        foreach (char c in englishNumbers)
        {
            if (!englishNumberData.Exists(x => x.Character == c))
            {
                AddCharacterToList(c, englishNumberData);
            }
        }
    }

    [ContextMenu("Add Arabic Numbers")]
    private void AddArabicNumbers()
    {
        foreach (char c in arabicNumbers)
        {
            if (!arabicNumberData.Exists(x => x.Character == c))
            {
                AddCharacterToList(c, arabicNumberData);
            }
        }
    }

    [ContextMenu("Validate All Resources")]
    private void ValidateAllResources()
    {
        Debug.Log("Validating all character resources...");
        ValidateCharacterList("English Characters", englishCharactersData);
        ValidateCharacterList("Arabic Characters", arabicCharactersData);
        ValidateCharacterList("English Numbers", englishNumberData);
        ValidateCharacterList("Arabic Numbers", arabicNumberData);
    }

    private void ValidateCurrentResources()
    {
        var currentData = GetCurrentCategoryData();
        ValidateCharacterList(currentCategory.ToString(), currentData);
    }
    #endregion

    #region Helper Functions
    private void AddCharacterToList(char character, List<LevelCharacter> list)
    {
        var clip = Resources.Load<AudioClip>($"Audio/{character}");
        var clipIntro = Resources.Load<AudioClip>($"Audio/Exploring the Letter {character}");
        var sprite = Resources.Load<Sprite>($"Sprites/{character}");
        var fbx = Resources.Load<GameObject>($"Models/{character}");

        var newChar = new LevelCharacter(character, clip, clipIntro, sprite, fbx);
        list.Add(newChar);

        Debug.Log($"Added character '{character}' to list");
        ValidateCharacterResources(newChar);
    }

    private void ValidateCharacterList(string listName, List<LevelCharacter> characters)
    {
        Debug.Log($"Validating {listName}...");

        foreach (var character in characters)
        {
            ValidateCharacterResources(character);
        }
    }

    private void ValidateCharacterResources(LevelCharacter character)
    {
        if (character.LetterIntro == null)
            Debug.Log($"Missing audio for character '{character.Character}'");
        if (character.CharacterSound == null)
            Debug.LogWarning($"Missing audio for character '{character.Character}'");
        if (character.CharImage == null)
            Debug.LogWarning($"Missing sprite for character '{character.Character}'");
        if (character.CharFBX == null)
            Debug.LogWarning($"Missing model for character '{character.Character}'");
    }

    public void PlayCorrectSound() => AudioManager.Instance.PlaySound(correctAnswerSound);
    public void PlayWrongSound() => AudioManager.Instance.PlaySound(wrongAnswerSound);
    #endregion
}