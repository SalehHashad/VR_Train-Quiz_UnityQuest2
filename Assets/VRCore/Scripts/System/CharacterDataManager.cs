using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CharacterDataManager : MonoBehaviour, ICharacterDataProvider
{
    private static CharacterDataManager instance;
    public static CharacterDataManager Instance => instance;

    [SerializeField] private AudioClip correctAnswerSound;
    [SerializeField] private AudioClip wrongAnswerSound;

    [SerializeField] private List<LevelCharacter> englishCharactersData = new List<LevelCharacter>();
    private readonly string englishChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private int currentIndex = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        LoadCharacterResources();
    }

    public char GetNextCharacter()
    {
        if (currentIndex >= englishChars.Length)
            throw new System.IndexOutOfRangeException("No more characters available");

        return englishChars[currentIndex++];
    }

    public LevelCharacter GetCharacterData(char character)
    {
        return englishCharactersData.Find(x => x.Character == character);
    }

    public void LoadCharacterResources()
    {
        foreach (char c in englishChars)
        {
            var clip = Resources.Load<AudioClip>($"Audio/{c}");
            var sprite = Resources.Load<Sprite>($"Sprites/{c}");
            var fbx = Resources.Load<GameObject>($"Models/{c}");

            var characterData = new LevelCharacter(c, clip, sprite, fbx);
            englishCharactersData.Add(characterData);
            ValidateCharacterResources(characterData);
        }
    }

    private void ValidateCharacterResources(LevelCharacter character)
    {
        if (character.CharacterSound == null)
            Debug.LogWarning($"Missing audio for character {character.Character}");
        if (character.CharImage == null)
            Debug.LogWarning($"Missing sprite for character {character.Character}");
        if (character.CharFBX == null)
            Debug.LogWarning($"Missing model for character {character.Character}");
    }

    public void PlayCorrectSound() => AudioManager.Instance.PlaySound(correctAnswerSound);
    public void PlayWrongSound() => AudioManager.Instance.PlaySound(wrongAnswerSound);
}