using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemQustionPanel : MonoBehaviour
{
    public static SystemQustionPanel instance;

    private void Awake()
    {
        instance = this;
    }

    public void AddButton(GameObject btnPrefab, char c, Sprite sprite, bool isTrue = false)
    {
        GameObject newBtn = Instantiate(btnPrefab, this.transform);
        var buttonComponent = newBtn.GetComponent<SystemIsCorrectButton>();
        buttonComponent.answerChar = c;
        buttonComponent.answerImage = sprite;
        buttonComponent.isCorrect = isTrue;
    }
}
