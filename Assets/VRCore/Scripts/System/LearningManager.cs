using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static QuestionGenerator;

public class LearningManager : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform questionPanel;
    [SerializeField] private Sprite blankSprite;

    private const int NEW_LETTER_QUESTIONS = 3;    
    private const int REVIEW_QUESTIONS = 3;        
    private const int MAX_PAST_CHARACTERS = 5;     

    private ICharacterDataProvider characterDataProvider;
    private List<LevelCharacter> pastCharacters = new List<LevelCharacter>();
    private LevelCharacter currentCharacter;
    private bool isNewLetterPhase = true;
    private int currentQuestionNumber = 0;
    public int reviewIndex = 0;
    public int myReviewInde = 0;
    public int CharacterIndex = 0;
    private void Start()
    {
        //if (!ValidateReferences()) return;
        characterDataProvider = CharacterDataManager.Instance;
        
        SetupNewCharacter();
        GenerateQuestion();
    }

    private bool ValidateReferences()
    {
        if (buttonPrefab == null)
        {
            Debug.LogError("Button Prefab is not assigned!");
            return false;
        }
        if (questionPanel == null)
        {
            Debug.LogError("Question Panel is not assigned!");
            return false;
        }
        if (blankSprite == null)
        {
            Debug.LogError("Blank Sprite is not assigned!");
            return false;
        }
        return true;
    }

    private void SetupNewCharacter()
    {
        if (currentCharacter != null)
        {
            AddToPastCharacters(currentCharacter);
        }

        char nextChar = characterDataProvider.GetNextCharacter();
        currentCharacter = characterDataProvider.GetCharacterData(nextChar);
        isNewLetterPhase = true;
        currentQuestionNumber = 0;
        reviewIndex = 0;
    }

    private void AddToPastCharacters(LevelCharacter character)
    {
        if (pastCharacters.Count >= MAX_PAST_CHARACTERS)
        {
            pastCharacters.RemoveAt(0);
        }
        pastCharacters.Add(character);
    }

    private void GenerateQuestion()
    {
        ClearQuestionPanel();

        if (!isNewLetterPhase && pastCharacters.Count == 0)
        {
            SetupNewCharacter();
            return;
        }

        if (isNewLetterPhase)
        {
            GenerateNewLetterQuestion();
        }
        else
        {
            GenerateReviewQuestion();
        }
    }

    private void GenerateNewLetterQuestion()
    {
        PlayCharacterSound(currentCharacter);

        var options = QuestionGenerator.GenerateOptions(
            currentCharacter,
            new List<LevelCharacter>(),
            true,
            currentQuestionNumber,
            blankSprite
        );

        CreateAnswerButtons(options);
    }

    private void GenerateReviewQuestion()
    {
        if (pastCharacters.Count == 0)
        {
            SetupNewCharacter();
            return;
        }

        if (reviewIndex >= pastCharacters.Count)
        {
            reviewIndex = 0;
        }

        var reviewCharacter = pastCharacters[reviewIndex];
        PlayCharacterSound(reviewCharacter);

        var availableCharacters = new List<LevelCharacter>(pastCharacters);
        if (!availableCharacters.Contains(currentCharacter))
        {
            availableCharacters.Add(currentCharacter);
        }

        int optionCount = (CharacterDataManager.Instance.currentIndex == 3) ? 4 : 3;
        var options = new QuestionOption[optionCount];

        options = QuestionGenerator.GenerateOptions(
            reviewCharacter,
            availableCharacters,
            false,
            0,
            blankSprite
        );
        CreateAnswerButtons(options);
    }

    private void OnAnswerSelected(bool isCorrect)
    {
        if (isCorrect)
        {
            CharacterDataManager.Instance.PlayCorrectSound();
            StartCoroutine(HandleCorrectAnswerWithDelay());
        }
        else
        {
            CharacterDataManager.Instance.PlayWrongSound();
            if (isNewLetterPhase)
            {
                StartCoroutine(RegenerateQuestionAfterDelay());
            }
        }
    }

    private IEnumerator HandleCorrectAnswerWithDelay()
    {
        yield return new WaitForSeconds(1f);

        if (isNewLetterPhase)
        {
            currentQuestionNumber++;
            if (currentQuestionNumber >= NEW_LETTER_QUESTIONS)
            {
                if (pastCharacters.Count > 0)
                {
                    isNewLetterPhase = false;
                    currentQuestionNumber = 0;
                    reviewIndex = 0;
                }
                else
                {
                    SetupNewCharacter();
                }
            }
        }
        else
        {
            myReviewInde++;
            reviewIndex++;
            if (/*reviewIndex >= pastCharacters.Count ||*/ myReviewInde >= REVIEW_QUESTIONS)
            {
                myReviewInde = 0;
                SetupNewCharacter();
            }
            
        }
        GenerateQuestion();
    }

    private void ClearQuestionPanel()
    {
        foreach (Transform child in questionPanel)
        {
            Destroy(child.gameObject);
        }
    }

    private void PlayCharacterSound(LevelCharacter character)
    {
        if (character.CharacterSound != null)
        {
            AudioManager.Instance.PlaySound(character.CharacterSound);
        }
    }

    private void CreateAnswerButtons(QuestionOption[] options)
    {
        if (options == null || options.Length == 0)
        {
            Debug.LogError("No options provided for creating answer buttons!");
            return;
        }

        foreach (var option in options)
        {
            var buttonObject = Instantiate(buttonPrefab, questionPanel);
            var button = buttonObject.GetComponent<AnswerButton>();

            if (button == null)
            {
                Debug.LogError("AnswerButton component not found on button prefab!");
                continue;
            }

            button.Initialize(option.Character, option.Sprite, option.IsCorrect, OnAnswerSelected);
        }
    }

    private IEnumerator RegenerateQuestionAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        GenerateQuestion();
    }
}