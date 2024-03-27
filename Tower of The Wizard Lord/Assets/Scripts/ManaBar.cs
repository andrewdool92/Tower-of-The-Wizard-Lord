using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBar : MonoBehaviour
{
    [SerializeField] GameObject manaPip;
    [SerializeField] int maxMana;
    [SerializeField] float offset;

    private bool[] mana;
    private Animator[] manaPipAnimators;

    // Start is called before the first frame update
    void Start()
    {
        mana = new bool[maxMana];
        manaPipAnimators = new Animator[maxMana];

        for (int i = maxMana - 1; i >= 0; i--)
        {
            Debug.Log($"i = {i}");
            GameObject pip = Instantiate(manaPip, transform);
            pip.name = $"pip ({i})";

            pip.GetComponent<RectTransform>().anchoredPosition = new Vector3(-offset * i, 0, 0);
            manaPipAnimators[i] = pip.GetComponent<Animator>();

            manaPipAnimators[i].Play("Base Layer.charged", 0, Random.value);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
