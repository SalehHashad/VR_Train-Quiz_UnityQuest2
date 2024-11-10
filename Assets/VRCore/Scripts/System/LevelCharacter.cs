using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelCharacter
{
    public char Character { get; private set; }
    public AudioClip CharacterSound { get; private set; }
    public Sprite CharImage { get; private set; }
    public GameObject CharFBX { get; private set; }

    public LevelCharacter(char character, AudioClip sound, Sprite image, GameObject fbx)
    {
        Character = character;
        CharacterSound = sound;
        CharImage = image;
        CharFBX = fbx;
    }
}
