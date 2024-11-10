using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LearningManager : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform questionPanel;

    private const int TOTAL_QUESTIONS = 3;
    private const int MAX_PAST_CHARACTERS = 5;

    private ICharacterDataProvider characterDataProvider;
    private List<LevelCharacter> pastCharacters = new List<LevelCharacter>();
    private LevelCharacter currentCharacter;
    private int correctAnswersCount;
    private int previousCharacterIndex;
    private bool isReviewMode;

    private void Start()
    {
        characterDataProvider = CharacterDataManager.Instance;
        SetupNewCharacter();
        GenerateQuestion();
    }

    private void SetupNewCharacter()
    {
        if (currentCharacter != null)
            AddToPastCharacters(currentCharacter);

        char nextChar = characterDataProvider.GetNextCharacter();
        currentCharacter = characterDataProvider.GetCharacterData(nextChar);
    }

    private void AddToPastCharacters(LevelCharacter character)
    {
        if (pastCharacters.Count >= MAX_PAST_CHARACTERS)
            pastCharacters.RemoveAt(0);

        pastCharacters.Add(character);
    }

    private void GenerateQuestion()
    {
        ClearQuestionPanel();

        var targetCharacter = isReviewMode ?
            pastCharacters[previousCharacterIndex] :
            currentCharacter;

        PlayCharacterSound(targetCharacter);

        var options = QuestionGenerator.GenerateOptions(
            targetCharacter,
            isReviewMode ? pastCharacters : pastCharacters.Concat(new[] { currentCharacter }).ToList()
        );

        CreateAnswerButtons(options);
    }

    private void ClearQuestionPanel()
    {
        foreach (Transform child in questionPanel)
            Destroy(child.gameObject);
    }

    private void PlayCharacterSound(LevelCharacter character)
    {
        if (character.CharacterSound != null)
            AudioManager.Instance.PlaySound(character.CharacterSound);
    }

    private void CreateAnswerButtons(QuestionGenerator.QuestionOption[] options)
    {
        foreach (var option in options)
        {
            var button = Instantiate(buttonPrefab, questionPanel).GetComponent<AnswerButton>();
            button.Initialize(option.Character, option.Sprite, option.IsCorrect, OnAnswerSelected);
        }
    }

    private void OnAnswerSelected(bool isCorrect)
    {
        if (isCorrect)
        {
            CharacterDataManager.Instance.PlayCorrectSound();
            HandleCorrectAnswer();
        }
        else
        {
            CharacterDataManager.Instance.PlayWrongSound();
        }
    }

    private void HandleCorrectAnswer()
    {
        if (isReviewMode)
            HandleReviewModeCorrectAnswer();
        else
            HandleLearningModeCorrectAnswer();
    }

    private void HandleReviewModeCorrectAnswer()
    {
        previousCharacterIndex++;

        if (previousCharacterIndex >= pastCharacters.Count)
        {
            isReviewMode = false;
            previousCharacterIndex = 0;
            SetupNewCharacter();
        }

        StartCoroutine(GenerateNextQuestionDelayed());
    }

    private void HandleLearningModeCorrectAnswer()
    {
        correctAnswersCount++;

        if (correctAnswersCount >= TOTAL_QUESTIONS)
        {
            correctAnswersCount = 0;

            if (pastCharacters.Count > 0)
            {
                isReviewMode = true;
                previousCharacterIndex = 0;
            }
            else
            {
                SetupNewCharacter();
            }
        }

        StartCoroutine(GenerateNextQuestionDelayed());
    }

    private IEnumerator GenerateNextQuestionDelayed()
    {
        yield return new WaitForSeconds(1f);
        GenerateQuestion();
    }
}
