using UnityEngine;

[System.Serializable]
public class LevelCharacter
{
    public char Character;
    public AudioClip CharacterSound;
    public Sprite CharImage;
    public GameObject CharFBX;

    public LevelCharacter(char character, AudioClip sound, Sprite image, GameObject fbx)
    {
        Character = character;
        CharacterSound = sound;
        CharImage = image;
        CharFBX = fbx;
    }
}