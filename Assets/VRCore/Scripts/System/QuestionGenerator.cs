using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestionGenerator
{
    private const int NUM_OPTIONS = 3;

    public struct QuestionOption
    {
        public char Character;
        public Sprite Sprite;
        public bool IsCorrect;
    }

    public static QuestionOption[] GenerateOptions(LevelCharacter correctCharacter, List<LevelCharacter> otherOptions)
    {
        var options = new List<QuestionOption>
        {
            new QuestionOption
            {
                Character = correctCharacter.Character,
                Sprite = correctCharacter.CharImage,
                IsCorrect = true
            }
        };

        var wrongOptions = otherOptions.Where(x => x.Character != correctCharacter.Character).ToList();
        var selectedWrong = SelectRandomElements(wrongOptions, NUM_OPTIONS - 1);

        options.AddRange(selectedWrong.Select(x => new QuestionOption
        {
            Character = x.Character,
            Sprite = x.CharImage,
            IsCorrect = false
        }));

        return ShuffleOptions(options.ToArray());
    }

    private static List<LevelCharacter> SelectRandomElements(List<LevelCharacter> list, int count)
    {
        var result = new List<LevelCharacter>();
        var tempList = new List<LevelCharacter>(list);

        while (result.Count < count && tempList.Count > 0)
        {
            int index = Random.Range(0, tempList.Count);
            result.Add(tempList[index]);
            tempList.RemoveAt(index);
        }

        return result;
    }

    private static QuestionOption[] ShuffleOptions(QuestionOption[] options)
    {
        for (int i = options.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            var temp = options[randomIndex];
            options[randomIndex] = options[i];
            options[i] = temp;
        }
        return options;
    }
}
