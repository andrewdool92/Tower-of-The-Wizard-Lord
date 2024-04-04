using System.Collections;
using System.Collections.Generic;
using Unity.Services.Mediation;
using UnityEngine;

public class dialogueEvent : MonoBehaviour
{
    public string[] names;
    public string[] texts;
    public List<DialogueEventManager.dialoguePage> pages;

    // Start is called before the first frame update
    void Start()
    {
        pages = new List<DialogueEventManager.dialoguePage>();

        for (int i = 0; i < names.Length; i++)
        {
            pages.Add(new DialogueEventManager.dialoguePage(names[i], texts[i]));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (pages != null)
        {
            DialogueEventManager.Instance.startDialogue(pages);
        }

        transform.GetComponent<Collider2D>().enabled = false;
    }
}
