using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SystemCore : MonoBehaviour
{
    [SerializeField] GameObject btnPrefab;
    [ContextMenu("UpdateDisplay")]
    public void UpdateDisplay()
    {
        char c = SystemChar.instance.GetEnglishChar();
        FindObjectOfType<SystemCharDisplay>().GetComponent<TextMeshProUGUI>().text
            = c.ToString();
        print(c);
        for (int i = 0; i < SystemChar.instance.index; i++)
        {   
            SystemQustionPanel.instance.AddButton(btnPrefab, c, true);
        }
    }
}