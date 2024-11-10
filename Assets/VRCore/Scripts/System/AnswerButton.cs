using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerButton : MonoBehaviour
{
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI characterText;
    private Button button;
    private System.Action<bool> onAnswerSelected;
    private bool isCorrect;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Initialize(char character, Sprite sprite, bool correct, System.Action<bool> callback)
    {
        characterText.text = character.ToString();
        characterImage.sprite = sprite;
        isCorrect = correct;
        onAnswerSelected = callback;
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        onAnswerSelected?.Invoke(isCorrect);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClicked);
    }
}