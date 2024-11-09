using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemCharDisplay : MonoBehaviour
{
    public static SystemCharDisplay instance;
    private void Awake()
    {
        instance = this;
    }
}
