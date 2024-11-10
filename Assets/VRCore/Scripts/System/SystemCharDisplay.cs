using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemCharDisplay : MonoBehaviour
{
    public List<CharactersDataClass> characters = new List<CharactersDataClass>();

    public static SystemCharDisplay instance;
    private void Awake()
    {
        instance = this;
    }
}
