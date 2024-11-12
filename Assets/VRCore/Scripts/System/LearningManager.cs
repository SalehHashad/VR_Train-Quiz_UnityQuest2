    using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static QuestionGenerator;

public class LearningManager : MonoBehaviour
{
    [SerializeField] private Transform questionPanel;
    [SerializeField] private Transform questionPanel_1;
    [SerializeField] private Transform questionPanel_2;
    private Transform[] questionPanels;
    private int currentQuestionPanelIndex = 0;

    [SerializeField] private GameObject buttonPrefab;
    
    [SerializeField] private Sprite blankSprite;
    private bool canPlayCharacterSound = false;
    private bool isNewLetterPhase = true;
    public Transform characterSpawnPoint;
    private GameObject currentCharacterInstance;
    private Animator animator;
    //const Param
    private const int NEW_LETTER_QUESTIONS = 3;
    private const int INITIAL_REVIEW_QUESTIONS = 3;    
    private const int MEDIUM_REVIEW_QUESTIONS = 4;     
    private const int MAX_REVIEW_QUESTIONS = 5;
    private const int MAX_PAST_CHARACTERS = 5;
    private const int DEFAULT_OPTIONS = 3;
    private const int MEDIUM_OPTIONS = 4;
    private const int MAX_OPTIONS = 5;
    //Other Scripts
    private ICharacterDataProvider characterDataProvider;
    private List<LevelCharacter> pastCharacters = new List<LevelCharacter>();
    private LevelCharacter currentCharacter;
    //Integers 
    private int currentQuestionNumber = 0;
    private int reviewIndex = 0;
    private int myReviewInde = 0;
    private int CharacterIndex = 0;
    private int maxReachedIndex = 0;
    private int WrongAnswerCount = 0;


    // Coroutines
    private Coroutine characterSoundCoroutine;
    private Coroutine letterIntroCoroutine;
    private Coroutine audioLoopCoroutine;

    private void Awake()
    {
        questionPanels = new Transform[]
    {
        questionPanel,
        questionPanel_1,
        questionPanel_2
    };
    }

    private void Start()
    {
        if (CharacterDataManager.Instance.currentCategory == GameCategory.none)
        {
            Debug.LogWarning("No category selected. Please select a category from the main menu.");
            return;
        }

        characterDataProvider = CharacterDataManager.Instance;
        if (questionPanel != null)
        {
            questionPanel.gameObject.SetActive(false);
        }
        TrainAgent.Instance.OnTrainStopped += HandleTrainStopped;
        SetupNewCharacter();
    }

    private void OnDestroy()
    {
        if (TrainAgent.Instance != null)
        {
            TrainAgent.Instance.OnTrainStopped -= HandleTrainStopped;
        }
    }

    private IEnumerator PlayCharacterSoundLoop()
    {
        while (true)
        {
            AudioManager.Instance.PlaySound(currentCharacter.CharacterSound);
            yield return new WaitForSeconds(currentCharacter.CharacterSound.length);
        }
    }

    private IEnumerator PlayLetterIntroLoop()
    {
        while (!TrainAgent.Instance.isFirstArrival || TrainAgent.Instance.trainAgent.velocity.magnitude > 0.1f)
        {
            AudioManager.Instance.PlaySound(currentCharacter.LetterIntro);
            yield return new WaitForSeconds(currentCharacter.LetterIntro.length);
        }
    }

    private void StartCharacterSoundLoop()
    {
        if (characterSoundCoroutine != null)
        {
            StopCoroutine(characterSoundCoroutine);
        }
        characterSoundCoroutine = StartCoroutine(PlayCharacterSoundLoop());
    }

    private void StartLetterIntroLoop()
    {
        if (letterIntroCoroutine != null)
        {
            StopCoroutine(letterIntroCoroutine);
        }
        letterIntroCoroutine = StartCoroutine(PlayLetterIntroLoop());
    }

    private void StopAllAudioLoops()
    {
        if (characterSoundCoroutine != null)
        {
            StopCoroutine(characterSoundCoroutine);
            characterSoundCoroutine = null;
        }
        if (letterIntroCoroutine != null)
        {
            StopCoroutine(letterIntroCoroutine);
            letterIntroCoroutine = null;
        }
    }

    private void HandleTrainStopped()
    {
        if (audioLoopCoroutine != null)
        {
            StopCoroutine(audioLoopCoroutine);
            audioLoopCoroutine = null;
        }

        if (questionPanel != null)
        {
            questionPanel.gameObject.SetActive(true);
        }
    }
    //private IEnumerator LoopAudio(AudioClip clip)
    //{
    //    while (true)
    //    {
    //        AudioManager.Instance.PlaySound(clip);
    //        yield return new WaitForSeconds(clip.length);
    //    }
    //}

    private void OnTrainStopped()
    {
        print("Train is Stopped in Learning Manager");
        canPlayCharacterSound = true;
    }

    private void OnTrainStarted()
    {
        print("Train moves");
        canPlayCharacterSound = false;
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
        if (currentCharacterInstance != null)
        {
            Destroy(currentCharacterInstance);
        }

        char nextChar = characterDataProvider.GetNextCharacter();
        currentCharacter = characterDataProvider.GetCharacterData(nextChar);

        if (currentCharacter != null && currentCharacter.CharFBX != null)
        {
            Debug.Log(currentCharacter.CharFBX.name);
            SpwanModel(currentCharacter.CharFBX);
        }

        StartCoroutine(StartNewCharacterSequence());

    }
    private IEnumerator LoopAudio(AudioClip clip)
    {
        
        //AudioManager.Instance.PlaySound(currentCharacter.CharacterSound);
        //yield return new WaitForSeconds(currentCharacter.CharacterSound.length);

        while (!TrainAgent.Instance.isFirstArrival || TrainAgent.Instance.trainAgent.velocity.magnitude > 0.1f)
        {
            AudioManager.Instance.PlaySound(currentCharacter.LetterIntro);
            yield return new WaitForSeconds(clip.length);
        }
    }
    private IEnumerator StartNewCharacterSequence()
    {
        StopAllAudioLoops(); 

        if (currentCharacter != null)
        {
            StartCharacterSoundLoop();
            StartLetterIntroLoop();
        }

        TrainAgent.Instance.MoveTheTrain();

        while (TrainAgent.Instance.isFirstArrival)
        {
            yield return null;
        }

        StopAllAudioLoops(); 

        if (questionPanel != null)
        {
            questionPanel.gameObject.SetActive(true);
        }

        isNewLetterPhase = true;
        currentQuestionNumber = 0;
        reviewIndex = 0;
        myReviewInde = 0;

        GenerateQuestion();
    }

    private void SpwanModel(GameObject charModel)
    {
        if (charModel != null && characterSpawnPoint != null)
        {
            currentCharacterInstance = Instantiate(charModel, characterSpawnPoint.position, characterSpawnPoint.rotation, characterSpawnPoint);
            Animator animator = currentCharacterInstance.GetComponent<Animator>();
            if (animator == null)
            {
                animator = currentCharacterInstance.AddComponent<Animator>();
            }
            var animatorController = Resources.Load<RuntimeAnimatorController>("CharacterAnimation/CharactersAnim");

            if (animator != null)
            {
                animator.runtimeAnimatorController = animatorController;
            }
            else
            {
                Debug.LogWarning("No Animator Controller provided for the character.");
            }
        }
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
    private int GetCurrentReviewQuestionCount()
    {
        int currentIndex = CharacterDataManager.Instance.currentIndex;

        if (currentIndex > maxReachedIndex)
        {
            maxReachedIndex = currentIndex;
        }

        if (maxReachedIndex >= 5)
        {
            return MAX_REVIEW_QUESTIONS;
        }
        else if (maxReachedIndex >= 4)
        {
            return MEDIUM_REVIEW_QUESTIONS;
        }
        return INITIAL_REVIEW_QUESTIONS;
    }

    private int GetCurrentOptionCount()
    {
        if (maxReachedIndex >= 5)
        {
            currentQuestionPanelIndex = 2;
            return MAX_OPTIONS;
        }
        else if (maxReachedIndex >= 3)
        {
            currentQuestionPanelIndex = 1;
            return MEDIUM_OPTIONS;
        }
        else
        {
            currentQuestionPanelIndex = 0;
            return DEFAULT_OPTIONS;
        }
    }
    private void GenerateNewLetterQuestion()
    {
        var options = QuestionGenerator.GenerateOptions(
            currentCharacter,
            new List<LevelCharacter>(),
            true,
            currentQuestionNumber,
            blankSprite,
            3
        );
        CreateAnswerButtons(options);

        //PlayCharacterSound(currentCharacter);
    }

    private void GenerateReviewQuestion()
    {
        ClearQuestionPanel();
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

        int optionCount = GetCurrentOptionCount();
        var options = QuestionGenerator.GenerateOptions(
            reviewCharacter,
            availableCharacters,
            false,
            0,
            blankSprite,
            optionCount
        );

        CreateAnswerButtons(options);
    }

    private void OnAnswerSelected(bool isCorrect)
    {
        if (isCorrect)
        {
            WrongAnswerCount = 0;
            CharacterDataManager.Instance.PlayCorrectSound();
            StartCoroutine(HandleCorrectAnswerWithDelay());
        }
        else
        {
            CharacterDataManager.Instance.PlayWrongSound();
            WrongAnswerCount++;
            if (WrongAnswerCount > 3)
            {
                HighlightCorrectAnswer();
            }
            if (isNewLetterPhase)
            {
                StartCoroutine(RegenerateQuestionAfterDelay());
            }
        }
    }

    private void HighlightCorrectAnswer()
    {
        foreach (Transform child in questionPanel)
        {
            var button = child.GetComponent<AnswerButton>();
            if (button != null && button.isCorrect)
            {
                button.Highlight(); 
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
            int currentReviewQuestions = GetCurrentReviewQuestionCount();

            if (myReviewInde >= currentReviewQuestions)
            {
                myReviewInde = 0;
                SetupNewCharacter();
            }
            else if (reviewIndex >= pastCharacters.Count)
            {
                reviewIndex = 0;
            }
        }
        GenerateQuestion();
    }

    private void ClearQuestionPanel()
    {
        foreach (Transform child in questionPanels[currentQuestionPanelIndex])
        {
            Destroy(child.gameObject);
        }
    }

    public void PlayCharacterSound(LevelCharacter character)
    {
        if (character.CharacterSound != null)
        {
            AudioManager.Instance.PlaySound(character.CharacterSound);
        }
    }

    private void CreateAnswerButtons(QuestionOption[] options)
    {
        int optionCount = options.Length;
        int panelIndex = 0;

        if (optionCount == 4)
        {
            panelIndex = 1;
        }
        else if (optionCount == 5)
        {
            panelIndex = 2;
        }

        Transform targetPanel = questionPanels[panelIndex];

        foreach (var option in options)
        {
            var buttonObject = Instantiate(buttonPrefab, targetPanel);
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