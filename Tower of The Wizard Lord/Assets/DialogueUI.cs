using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    private TextMeshProUGUI speakerName;
    private TextMeshProUGUI text;
    private GameObject _dialogueBox;

    // Start is called before the first frame update
    void Start()
    {
        _dialogueBox = transform.Find("DialogueBox").gameObject;
        speakerName = _dialogueBox.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        text = _dialogueBox.transform.Find("Text").GetComponent<TextMeshProUGUI>();

        hideDialogue();

        DialogueEventManager.ShowDialogueEvent += updateTextBox;
        GameManager.StartDialogueEvent += showDialogue;
        GameManager.ExitDialogueEvent += hideDialogue;
    }

    private void OnDestroy()
    {
        DialogueEventManager.ShowDialogueEvent -= updateTextBox;
        GameManager.StartDialogueEvent -= showDialogue;
        GameManager.ExitDialogueEvent -= hideDialogue;
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
        _dialogueBox.SetActive(false);
    }
}
