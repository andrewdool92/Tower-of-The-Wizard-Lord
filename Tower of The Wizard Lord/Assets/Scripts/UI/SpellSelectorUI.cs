using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellSelectorUI : MonoBehaviour
{
    [SerializeField] Color selectedColor;
    [SerializeField] Color unselectedColor;

    private Dictionary<spellType, Image> spellIcons = new Dictionary<spellType, Image>();

    // Start is called before the first frame update
    void Start()
    {
        selectedColor.a = 255;
        unselectedColor.a = 255;

        spellIcons[spellType.fire] = transform.Find("Fire").GetComponent<Image>();
        spellIcons[spellType.ice] = transform.Find("Ice").GetComponent<Image>();
        spellIcons[spellType.zap] = transform.Find("Zap").GetComponent <Image>();
        spellIcons[spellType.wind] = transform.Find("Wind").GetComponent<Image>();

        GameManager.spellSelectEvent += updateSpellIcons;

        updateSpellIcons(spellType.starting);
    }

    private void OnDestroy()
    {
        GameManager.spellSelectEvent -= updateSpellIcons;
    }

    private void updateSpellIcons(spellType selected)
    {
        foreach (spellType spell in spellIcons.Keys)
        {
            updateIconColor(spellIcons[spell], selected == spell);
        }
    }

    private void updateIconColor(Image icon, bool selected)
    {
        icon.color = selected ? selectedColor : unselectedColor;
    }
}
