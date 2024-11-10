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
    public Image childImage;
    //SystemCore systemCore;
    private void Awake()
    {
       // systemCore = FindObjectOfType<SystemCore>();
        childImage = gameObject.GetComponentInChildren<Image>();
    }
    private void OnEnable()
    {
        this.GetComponentInChildren<TextMeshProUGUI>().text = answerChar.ToString();
        this.GetComponent<Button>().onClick.AddListener(OnClicked);
    }
    private void Start()
    {
        childImage.sprite = answerImage;
    }
    private void OnClicked()
    {
        if (isCorrect)
        {
            Debug.Log("Correct answer!");
            //systemCore.OnCorrectAnswer();
        }
        else
        {
            Debug.Log("Wrong answer!");
            //systemCore.OnWrongAnswer();
        }
        //TrainAgent.Instance.MoveTheTrain();
    }
}
