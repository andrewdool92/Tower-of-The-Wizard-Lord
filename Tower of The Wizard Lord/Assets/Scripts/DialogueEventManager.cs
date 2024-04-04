using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEventManager : MonoBehaviour
{
    private static DialogueEventManager _Instance;
    public static DialogueEventManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new GameObject().AddComponent<DialogueEventManager>();
                _Instance.name = _Instance.GetType().ToString();
                DontDestroyOnLoad(_Instance.gameObject);
            }
            return _Instance;
        }
    }

    public static event Action<string, string> ShowDialogueEvent;

    public struct dialoguePage
    {
        public dialoguePage(string name, string text)
        {
            this.name = name;
            this.text = text;
        }

        public string name { get; }
        public string text { get; }
    }

    private List<dialoguePage> _dialogueSet;
    private int _dialogueIndex = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        _dialogueIndex = 0;

        GameManager.ContinueDialogueEvent += handleContinueDialogue;
    }

    private void OnDestroy()
    {
        GameManager.ContinueDialogueEvent -= handleContinueDialogue;
    }

    public void startDialogue(List<dialoguePage> pages)
    {
        _dialogueIndex = 0;

        if (_dialogueSet == null)
        {
            _dialogueSet = new List<dialoguePage>();
        }

        foreach (dialoguePage page in pages)
        {
            _dialogueSet.Add(page);
        }

        Debug.Log(pages.Count);
        Debug.Log(_dialogueSet.Count);

        showDialogue(pages[0].name, pages[0].text);
        GameManager.Instance.enterDialogue();
    }


    public void showDialogue(string speakerName, string text)
    {
        ShowDialogueEvent?.Invoke(speakerName, text);
    }

    private void handleContinueDialogue()
    {
        _dialogueIndex++;
        Debug.Log($"Dialogue index: {_dialogueIndex} - count: {_dialogueSet.Count}");
        if (_dialogueIndex < _dialogueSet.Count)
        {
            dialoguePage dialogue = _dialogueSet[_dialogueIndex];
            showDialogue(dialogue.name, dialogue.text);
        }
        else
        {
            Debug.Log($"Dialogue index: {_dialogueIndex}");
            GameManager.Instance.endDialogue();
            _dialogueSet.Clear();
        }
    }

    public void setPages(List<dialoguePage> pages)
    {
        _dialogueSet = pages;
        _dialogueIndex = 0;
    }

}
