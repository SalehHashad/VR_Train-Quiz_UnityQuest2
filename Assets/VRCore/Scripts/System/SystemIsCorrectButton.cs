using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SystemIsCorrectButton : MonoBehaviour
{
    public bool isCorrect = false;
    public char answerChar;
    public Sprite answerImage;
    private void OnEnable()
    {
        this.GetComponentInChildren<TextMeshProUGUI>().text = answerChar.ToString();
        this.GetComponentInChildren<Image>().sprite = answerImage;
        this.GetComponent<Button>().onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        print($"is {isCorrect} Answer");
        TrainAgent.Instance.MoveTheTrain();
    }
}
