using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterDataProvider
{
    char GetNextCharacter();
    LevelCharacter GetCharacterData(char character);
    void LoadCharacterResources();
}