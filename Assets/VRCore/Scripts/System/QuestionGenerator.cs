using System.Collections.Generic;
using UnityEngine;

public class QuestionGenerator
{
    public struct QuestionOption
    {
        public char Character { get; }
        public Sprite Sprite { get; }
        public bool IsCorrect { get; }

        public QuestionOption(char character, Sprite sprite, bool isCorrect)
        {
            Character = character;
            Sprite = sprite;
            IsCorrect = isCorrect;
        }
    }

    public static QuestionOption[] GenerateOptions(
        LevelCharacter correctCharacter,
        List<LevelCharacter> availableCharacters,
        bool isNewLetterPhase,
        int questionNumber,
        Sprite blankSprite,
        int optionCount)
    {
        var options = new QuestionOption[optionCount];

        if (isNewLetterPhase)
        {
            // For new letter phase, distribute the correct answer among more blank options
            for (int i = 0; i < optionCount; i++)
            {
                if (i == questionNumber % optionCount) // Use modulo to ensure valid position
                {
                    options[i] = new QuestionOption(
                        correctCharacter.Character,
                        correctCharacter.CharImage,
                        true
                    );
                }
                else
                {
                    options[i] = new QuestionOption(
                        ' ',
                        blankSprite,
                        false
                    );
                }
            }
        }
        else
        {
            List<LevelCharacter> incorrectOptions = new List<LevelCharacter>(availableCharacters);
            incorrectOptions.Remove(correctCharacter);

            // Shuffle the incorrect options
            for (int i = incorrectOptions.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (incorrectOptions[i], incorrectOptions[randomIndex]) = (incorrectOptions[randomIndex], incorrectOptions[i]);
            }

            int correctPosition = Random.Range(0, optionCount);

            for (int i = 0; i < optionCount; i++)
            {
                if (i == correctPosition)
                {
                    options[i] = new QuestionOption(
                        correctCharacter.Character,
                        correctCharacter.CharImage,
                        true
                    );
                }
                else
                {
                    // Handle cases where we need more incorrect options than available
                    int incorrectIndex = (i > correctPosition ? i - 1 : i) % incorrectOptions.Count;
                    var incorrectChar = incorrectOptions[incorrectIndex];
                    options[i] = new QuestionOption(
                        incorrectChar.Character,
                        incorrectChar.CharImage,
                        false
                    );
                }
            }
        }

        return options;
    }
}