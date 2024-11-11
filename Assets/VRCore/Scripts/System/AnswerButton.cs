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
    Color color = Color.white;
    private System.Action<bool> onAnswerSelected;

    public bool isCorrect { get; private set; }


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

    public void Highlight()
    {
        //color.a = 150f;
        characterImage.color = Color.magenta;
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