using UnityEngine;

[System.Serializable]
public class LevelCharacter
{
    public char Character;
    public AudioClip CharacterSound;
    public AudioClip LetterIntro;
    public Sprite CharImage;
    public GameObject CharFBX;

    public LevelCharacter(char character, AudioClip sound, AudioClip soundIntro, Sprite image, GameObject fbx)
    {
        Character = character;
        CharacterSound = sound;
        LetterIntro = soundIntro;
        CharImage = image;
        CharFBX = fbx;
    }
}