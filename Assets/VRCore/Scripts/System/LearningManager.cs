using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
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
    public Transform CentercharacterSpawnPoint;
    private GameObject forwardCurrentCharacterInstance;
    private GameObject centerCurrentCharacterInstance;
    private Animator animator;

    private bool isSpwaning = false;
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

    //Events
    [SerializeField] private VoidEventChannelSO TrainStoppedEventSO;
    [SerializeField] private VoidEventChannelSO PlayLetterIntroEventSO;
    private void Awake()
    {
        questionPanels = new Transform[]
        {
            questionPanel,
            questionPanel_1,
            questionPanel_2
        };
    }

    private void OnEnable()
    {
        TrainStoppedEventSO.onEventRaised += TrainStopped;
        PlayLetterIntroEventSO.onEventRaised += PlayLetterIntroClip;
    }

    private void OnDisable()
    {
        TrainStoppedEventSO.onEventRaised -= TrainStopped;
        PlayLetterIntroEventSO.onEventRaised -= PlayLetterIntroClip;
    }
    public void TrainStopped()
    {
        Debug.LogError("Train IS Stopped");
        
    }

    public void PlayLetterIntroClip()
    {
        Debug.LogError("Audio Is playing " + currentCharacter.LetterIntro);
        AudioManager.Instance.PlayLetterIntro(currentCharacter.LetterIntro);
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
        SetupNewCharacter();
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
        isSpwaning = true;
        if (currentCharacter != null)
        {
            AddToPastCharacters(currentCharacter);
        }
        if (centerCurrentCharacterInstance != null)
        {
            Destroy(centerCurrentCharacterInstance);
        }

        char nextChar = characterDataProvider.GetNextCharacter();
        currentCharacter = characterDataProvider.GetCharacterData(nextChar);

        if (currentCharacter != null && currentCharacter.CharFBX != null)
        {
            SpwanModel(currentCharacter.CharFBX);
        }

        StartCoroutine(StartNewCharacterSequence());
        
    }

    private IEnumerator StartNewCharacterSequence()
    {
        yield return null;
        TrainAgent.Instance.ResumeTrainMovement();
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

    private void AddClipToEntryState(Animator animator, AnimationClip clip)
    {
        var animatorController = animator.runtimeAnimatorController as AnimatorController;

        if (animatorController == null)
        {
            Debug.LogError("Animator Controller is null or not of the correct type!");
            return;
        }
        var layer = animatorController.layers[0];

        var stateMachine = layer.stateMachine;
        var entryState = stateMachine.entryPosition;

        var state = stateMachine.AddState("EntryAnimationState");
        state.motion = clip;

        var transition = stateMachine.AddEntryTransition(state);

        Debug.Log("Clip added to Entry State successfully!");
    }

    private void ClearAnimator(Animator animator)
    {
        var animatorController = animator.runtimeAnimatorController as AnimatorController;

        if (animatorController == null)
        {
            Debug.LogError("Animator Controller is null or not of the correct type!");
            return;
        }

        
        foreach (var layer in animatorController.layers)
        {
            AnimatorStateMachine stateMachine = layer.stateMachine;
            stateMachine.states = new ChildAnimatorState[0]; 
        }

        Debug.Log("Cleared all animation clips from the Animator!");
    }

    private void SpwanModel(GameObject charModel)
    {
        if (isSpwaning == true)
        {
            //ClearAnimator(animator);
        }

        if (forwardCurrentCharacterInstance != null)
        {
            Destroy(forwardCurrentCharacterInstance);
            forwardCurrentCharacterInstance = null;
        }
        if (centerCurrentCharacterInstance != null)
        {
            Destroy(centerCurrentCharacterInstance);
            centerCurrentCharacterInstance = null;
        }

        if (charModel != null && characterSpawnPoint != null)
        {
            forwardCurrentCharacterInstance = Instantiate(charModel, characterSpawnPoint.position, characterSpawnPoint.rotation, characterSpawnPoint);
            centerCurrentCharacterInstance = Instantiate(charModel, CentercharacterSpawnPoint.position, CentercharacterSpawnPoint.rotation, CentercharacterSpawnPoint);

            HandleCategoryBasedScale(forwardCurrentCharacterInstance);
            HandleCategoryBasedScale(centerCurrentCharacterInstance);
            Animator animator = forwardCurrentCharacterInstance.GetComponent<Animator>();
            if (animator == null)
            {
                animator = forwardCurrentCharacterInstance.AddComponent<Animator>();
            }

            RuntimeAnimatorController animatorController = Resources.Load<RuntimeAnimatorController>("CharacterAnimation/CharactersAnim_test");
            AnimationClip animationClip = Resources.Load<AnimationClip>("CharacterAnimation/Alaaf");

            if (animatorController != null && animationClip != null)
            {
                animator.runtimeAnimatorController = animatorController;

                // Add the clip to the Entry State
                AddClipToEntryState(animator, animationClip);
            }
            else
            {
                Debug.LogWarning("Failed to load Animator Controller or Animation Clip!");
            }

            Debug.Log("Animator and Animation Clip setup completed!");
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
        PlayCharacterSound(currentCharacter);
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

    private void HandleCategoryBasedScale(GameObject characterObject)
    {
        Vector3 EnglishScale = new Vector3(3f, 3f, 3f);
        Vector3 DefaultScale = new Vector3(1f, 1f, 1f);

        if (characterObject != null)
        {
            if (CharacterDataManager.Instance.currentCategory == GameCategory.EnglishCharacter)
            {
                characterObject.transform.localScale = EnglishScale;
            }

            else
            {
                characterObject.transform.localScale = DefaultScale;
            }
        }
        else
        {
            Debug.Log(characterObject.name + "Not Found");
        }
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
        //PlayCharacterSound(currentCharacter);
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
        //PlayCharacterSound(currentCharacter);
    }
}