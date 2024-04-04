using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    private TextMeshProUGUI speakerName;
    private TextMeshProUGUI text;
    private GameObject _dialogueBox;

    private Transform _moveTutorial;
    private Transform _spellTutorial;
    private Transform _swapTutorial;
    private Transform _activeTutorial;

    // Start is called before the first frame update
    void Start()
    {
        _dialogueBox = transform.Find("DialogueBox").gameObject;
        speakerName = _dialogueBox.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        text = _dialogueBox.transform.Find("Text").GetComponent<TextMeshProUGUI>();

        _setupTutorials();
        hideDialogue();

        DialogueEventManager.ShowDialogueEvent += updateTextBox;
        DialogueEventManager.showTutorialEvent += showTutorial;
        GameManager.StartDialogueEvent += showDialogue;
        GameManager.ExitDialogueEvent += hideDialogue;
    }

    private void OnDestroy()
    {
        DialogueEventManager.ShowDialogueEvent -= updateTextBox;
        DialogueEventManager.showTutorialEvent -= showTutorial;
        GameManager.StartDialogueEvent -= showDialogue;
        GameManager.ExitDialogueEvent -= hideDialogue;
    }

    private void _setupTutorials()
    {
        Transform tutorialRoot = transform.Find("Tutorials");

        _moveTutorial = tutorialRoot.transform.Find("Move");
        _moveTutorial.gameObject.SetActive(false);
    }

    private void updateSpeaker(string name)
    {
        speakerName.text = name;
    }

    private void updateText(string newText)
    {
        text.text = newText;
    }

    private void updateTextBox(string name, string newText)
    {
        updateSpeaker(name);
        updateText(newText);
    }

    private void showDialogue()
    {
        _dialogueBox.SetActive(true);
    }

    private void hideDialogue()
    {
        if (_activeTutorial != null)
        {
            _activeTutorial.gameObject.SetActive(false);
            _activeTutorial = null;
        }
        _dialogueBox.SetActive(false);
    }

    private void showTutorial(Tutorial tutorial)
    {
        _activeTutorial = tutorial switch
        {
            Tutorial.move => _moveTutorial,
            Tutorial.spell => _spellTutorial,
            Tutorial.swap => _swapTutorial,
            _ => null
        };

        if (_activeTutorial != null)
        {
            _activeTutorial.gameObject.SetActive(true);
            return;
        }
    }
}
