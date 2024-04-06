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
    public static event Action<Tutorial> showTutorialEvent;

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

    public HashSet<Tutorial> viewedTutorials = new HashSet<Tutorial>();

    public bool moveTutorialEnabled = true;
    public bool spellTutorialEnabled = true;
    public bool swapTutorialEnabled = true;
    public bool spendTutorialEnabled = true;

    private Tutorial _activeTutorial = Tutorial.none;
    
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

        showDialogue(pages[0].name, pages[0].text);
        GameManager.Instance.enterDialogue();
    }


    public void showDialogue(string speakerName, string text)
    {
        ShowDialogueEvent?.Invoke(speakerName, text);
    }

    private void handleContinueDialogue()
    {
        if (_activeTutorial == Tutorial.none && _dialogueIndex++ < _dialogueSet.Count - 1)
        {
            dialoguePage dialogue = _dialogueSet[_dialogueIndex];
            showDialogue(dialogue.name, dialogue.text);
        }
        else
        {
            GameManager.Instance.endDialogue();
            _dialogueSet?.Clear();
            _activeTutorial = Tutorial.none;
        }
    }

    public void setPages(List<dialoguePage> pages)
    {
        _dialogueSet = pages;
        _dialogueIndex = 0;
    }

    public void displayTutorial(Tutorial tutorial)
    {
        //if ((tutorial == Tutorial.move && moveTutorialEnabled) ||
        //    (tutorial == Tutorial.spell && spellTutorialEnabled) ||
        //    (tutorial == Tutorial.manaSpend && spendTutorialEnabled))
        //{
        //    _activeTutorial = tutorial;
        //    GameManager.Instance.enterDialogue();
        //    showTutorialEvent?.Invoke(tutorial);
        //    moveTutorialEnabled = false;
        //}

        if (!(viewedTutorials.Contains(tutorial))) {
            _activeTutorial = tutorial;
            GameManager.Instance.enterDialogue();
            showTutorialEvent?.Invoke(tutorial);
            viewedTutorials.Add(tutorial);
        } 
    }
}

public enum Tutorial
{
    move,
    spell,
    manaSpend,
    swap,
    none
}
