using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SystemCore : MonoBehaviour
{
    [SerializeField] GameObject btnPrefab;
    [ContextMenu("UpdateDisplay")]
    public void UpdateDisplays()
    {
        LevelCharacter currentCharacterData = SystemChar.instance.UpdateNextQuestion();
        foreach(Transform t in SystemCharDisplay.instance.transform) { Destroy(t.gameObject); }
        GameObject g = Instantiate(currentCharacterData.charFBX, SystemCharDisplay.instance.transform.position, Quaternion.identity);
        g.transform.parent = SystemCharDisplay.instance.transform;
        g.transform.localScale = Vector3.one;
    }


    //SystemCharDisplay.instance
    //SystemQustionPanel.instance.AddButton(btnPrefab, c, true);


}