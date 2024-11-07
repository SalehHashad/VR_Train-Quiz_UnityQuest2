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

    public void AddButton(GameObject Btn,char c ,bool IsTrue = false)
    {
        Instantiate(Btn,this.transform);
        Btn.GetComponent<SystemIsCorrectButton>().answerChar = c;
        Btn.GetComponent<SystemIsCorrectButton>().isCorrect = IsTrue;
    }
}
