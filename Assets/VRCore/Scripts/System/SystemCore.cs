//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class SystemCore : MonoBehaviour
//{
//    public GameObject panel;
//    public GameObject buttonPrefab;
//    private SystemChar systemChar;
//    [SerializeField] private int correctAnswersCount = 0;
//    [SerializeField] private int correctAnswersForPrevious = 0;
//    private const int TOTAL_QUESTIONS = 3;
//    private const int MAX_PAST_CHARACTERS = 5;
//    [SerializeField] private List<LevelCharacter> pastCharacters = new List<LevelCharacter>();
//    // Store the current character and sprite
//    private char currentChar;
//    private Sprite currentSprite;
//    TrainAgent trainAgent;
//    private char previousChar;
//    private Sprite previousSprite;
//    private bool isPreviousCharacterMode = false;

//    private void Start()
//    {
//        trainAgent = TrainAgent.Instance;
//        systemChar = SystemChar.instance;
//        if (systemChar != null)
//        {
//            SetupNewCharacter();
//            GenerateQuestion();
//        }
//    }

//    private void SetupNewCharacter()
//    {
//        // Save the current character data to pastCharacters list if it's valid
//        if (currentChar != '\0')
//        {
//            var charData = systemChar.englishCharactersData.Find(x => x.Character == currentChar);
//            if (charData != null)
//            {
//                if (pastCharacters.Count >= MAX_PAST_CHARACTERS)
//                {
//                    pastCharacters.RemoveAt(0); // Keep only the last 5 characters
//                }
//                pastCharacters.Add(charData); // Add current character to past characters list
//            }
//        }
//        // Load the next character
//        currentChar = systemChar.GetEnglishChar();
//        var newCharData = systemChar.englishCharactersData.Find(x => x.Character == currentChar);
//        if (newCharData != null)
//        {
//            currentSprite = newCharData.charImage;
//        }
//    }

//    public void GenerateQuestion()
//    {
//        foreach (Transform child in SystemQustionPanel.instance.transform)
//        {
//            Destroy(child.gameObject);
//        }
//        var answers = new[]
//        {
//            (character: currentChar, sprite: currentSprite, isCorrect: true),
//            (character: ' ', sprite: null, isCorrect: false),
//            (character: ' ', sprite: null, isCorrect: false)
//        };

//        for (int i = answers.Length - 1; i > 0; i--)
//        {
//            int randomIndex = Random.Range(0, i + 1);
//            var temp = answers[randomIndex];
//            answers[randomIndex] = answers[i];
//            answers[i] = temp;
//        }

//        // Create the buttons
//        foreach (var answer in answers)
//        {
//            SystemQustionPanel.instance.AddButton(
//                buttonPrefab,
//                answer.character,
//                answer.sprite,
//                answer.isCorrect
//            );
//        }
//    }

//    private void GeneratePreviousCharacterQuestions()
//    {
//        // Clear previous questions on the UI
//        foreach (Transform child in SystemQustionPanel.instance.transform)
//        {
//            Destroy(child.gameObject);
//        }

//        isPreviousCharacterMode = true;

//        // Get the current past character
//        LevelCharacter correctCharacter = pastCharacters[correctAnswersForPrevious];

//        // Play the character's sound
//        if (correctCharacter.CharacterSound != null)
//        {
//            AudioSource.PlayClipAtPoint(correctCharacter.CharacterSound, Camera.main.transform.position);
//        }

//        // Create a list of wrong options from other past characters
//        List<LevelCharacter> wrongOptions = new List<LevelCharacter>();
//        for (int i = 0; i < pastCharacters.Count; i++)
//        {
//            if (i != correctAnswersForPrevious) // Don't include the correct character
//            {
//                wrongOptions.Add(pastCharacters[i]);
//            }
//        }

//        // Randomly select two wrong options (or use what's available if less than 2)
//        List<LevelCharacter> selectedWrongOptions = new List<LevelCharacter>();
//        while (selectedWrongOptions.Count < 2 && wrongOptions.Count > 0)
//        {
//            int index = Random.Range(0, wrongOptions.Count);
//            selectedWrongOptions.Add(wrongOptions[index]);
//            wrongOptions.RemoveAt(index);
//        }

//        // If we don't have enough wrong options, fill with repeated characters
//        while (selectedWrongOptions.Count < 2)
//        {
//            selectedWrongOptions.Add(pastCharacters[Random.Range(0, pastCharacters.Count)]);
//        }

//        // Create the answers array with the correct and wrong options
//        var answers = new[]
//        {
//            (character: correctCharacter.Character, sprite: correctCharacter.charImage, isCorrect: true),
//            (character: selectedWrongOptions[0].Character, sprite: selectedWrongOptions[0].charImage, isCorrect: false),
//            (character: selectedWrongOptions[1].Character, sprite: selectedWrongOptions[1].charImage, isCorrect: false)
//        };

//        // Shuffle answers
//        for (int i = answers.Length - 1; i > 0; i--)
//        {
//            int randomIndex = Random.Range(0, i + 1);
//            var temp = answers[randomIndex];
//            answers[randomIndex] = answers[i];
//            answers[i] = temp;
//        }

//        // Create buttons for each question
//        foreach (var answer in answers)
//        {
//            SystemQustionPanel.instance.AddButton(
//                buttonPrefab,
//                answer.character,
//                answer.sprite,
//                answer.isCorrect
//            );
//        }
//    }

//    public void OnCorrectAnswer()
//    {
//        if (isPreviousCharacterMode)
//        {
//            if (systemChar.correctAnswer != null)
//            {
//                AudioSource.PlayClipAtPoint(systemChar.correctAnswer, Camera.main.transform.position);
//            }

//            correctAnswersForPrevious++;

//            if (correctAnswersForPrevious >= pastCharacters.Count)
//            {
//                // Reset for new character
//                isPreviousCharacterMode = false;
//                correctAnswersForPrevious = 0;
//                SetupNewCharacter();
//                GenerateQuestion();
//            }
//            else
//            {
//                // Generate another previous character question
//                StartCoroutine(GenerateNextPreviousQuestion());
//            }
//        }
//        else
//        {
//            correctAnswersCount++;

//            if (systemChar.correctAnswer != null)
//            {
//                AudioSource.PlayClipAtPoint(systemChar.correctAnswer, Camera.main.transform.position);
//            }

//            if (correctAnswersCount < TOTAL_QUESTIONS)
//            {
//                StartCoroutine(GenerateNextQuestion());
//            }
//            else
//            {
//                Debug.Log("All questions for the current character completed!");
//                correctAnswersCount = 0; // Reset main question count

//                if (pastCharacters.Count > 0)
//                {
//                    correctAnswersForPrevious = 0; // Reset the counter for previous characters
//                    GeneratePreviousCharacterQuestions(); // Generate questions for past characters
//                }
//                else
//                {
//                    SetupNewCharacter(); // Move to the next new character
//                    GenerateQuestion(); // Start new questions for the next character
//                }
//            }
//        }
//    }

//    private IEnumerator GenerateNextPreviousQuestion()
//    {
//        yield return new WaitForSeconds(1f);
//        GeneratePreviousCharacterQuestions();
//    }

//    private IEnumerator GenerateNextQuestion()
//    {
//        yield return new WaitForSeconds(1f);
//        GenerateQuestion();
//    }
//    public void OnWrongAnswer()
//    {
//        if (systemChar.wrongAnswer != null)
//        {
//            AudioSource.PlayClipAtPoint(systemChar.wrongAnswer, Camera.main.transform.position);
//        }
//    }
//}