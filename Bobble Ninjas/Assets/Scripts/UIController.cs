using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public UIDocument mainMenuDoc;
    public UIDocument levelSelectDoc;

    public GameObject mainMenuUIObject;
    public GameObject levelSelectUIObject;

    public Button startButton;
    public Button settingsButton;
    public Button exitButton;

    public Button backButton;

    void Start()
    {
        mainMenuDoc = mainMenuUIObject.GetComponent<UIDocument>();
        levelSelectDoc = levelSelectUIObject.GetComponent<UIDocument>();

        mainMenuDoc.rootVisualElement.style.display = DisplayStyle.Flex;
        levelSelectDoc.rootVisualElement.style.display = DisplayStyle.None;

        startButton = mainMenuDoc.rootVisualElement.Q<Button>("StartGame");
        settingsButton = mainMenuDoc.rootVisualElement.Q<Button>("Settings");
        exitButton = mainMenuDoc.rootVisualElement.Q<Button>("ExitGame");

        backButton = levelSelectDoc.rootVisualElement.Q<Button>("BackButton");

        startButton.clicked += StartButtonPressed;
        backButton.clicked += BackButtonPressed;
    }
    
    void StartButtonPressed()
    {
        mainMenuDoc.rootVisualElement.style.display = DisplayStyle.None;
        levelSelectDoc.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    void SettingsButtonPressed()
    {

    }

    void ExitButtonPressed()
    {

    }

    void BackButtonPressed()
    {
        levelSelectDoc.rootVisualElement.style.display = DisplayStyle.None;
        mainMenuDoc.rootVisualElement.style.display = DisplayStyle.Flex;
    }
}
