using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance; 
    [SerializeField] private Button btnEnglishNumbers;
    [SerializeField] private Button btnArabicNumbers;
    [SerializeField] private Button btnEnglishAlphabet;
    [SerializeField] private Button btnArabicAlphabet;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (CharacterDataManager.Instance == null)
        {
            GameObject managerObject = new GameObject("CharacterDataManager");
            CharacterDataManager manager = managerObject.AddComponent<CharacterDataManager>();
            DontDestroyOnLoad(managerObject);
        }


        btnEnglishNumbers.onClick.AddListener(() => SetCategory(GameCategory.EnglishNumber));
        btnArabicNumbers.onClick.AddListener(() => SetCategory(GameCategory.ArabicNumber));
        btnEnglishAlphabet.onClick.AddListener(() => SetCategory(GameCategory.EnglishCharacter));
        btnArabicAlphabet.onClick.AddListener(() => SetCategory(GameCategory.ArabicCharacter));
    }

    private void SetCategory(GameCategory category)
    {
        if (CharacterDataManager.Instance == null)
        {
            Debug.LogError("CharacterDataManager is not initialized.");
            return;
        }

        CharacterDataManager.Instance.SetCategory(category);
        Debug.Log($"Category set to: {category}");

        SceneManager.LoadScene(1);
    }
}
