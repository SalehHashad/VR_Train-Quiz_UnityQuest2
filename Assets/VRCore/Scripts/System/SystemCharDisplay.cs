using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemCharDisplay : MonoBehaviour
{
    public List<CharactersDataClass> characters = new List<CharactersDataClass>();
    public Transform characterSpawnPoint;
    private GameObject currentCharacterInstance;

    public static SystemCharDisplay instance;

    private void Awake()
    {
        instance = this;
    }

    public void SpawnCharacter(GameObject characterFBX)
    {
        if (currentCharacterInstance != null)
        {
            Debug.Log("Destroyyyyyyyy");
            Destroy(currentCharacterInstance);
        }

        if (characterFBX != null && characterSpawnPoint != null)
        {
            //Debug.Log(Equals(characterFBX.transform, characterSpawnPoint.transform));
            currentCharacterInstance = Instantiate(characterFBX, characterSpawnPoint.position, characterSpawnPoint.rotation, characterSpawnPoint);
        }
    }

    public void ClearCurrentCharacter()
    {
        if (currentCharacterInstance != null)
        {
            Destroy(currentCharacterInstance);
            currentCharacterInstance = null;
        }
    }
}