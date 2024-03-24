using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button : Switchable
{

    [SerializeField] Sprite _offSprite;
    [SerializeField] Sprite _onSprite;

    private SpriteRenderer _spriteRenderer;

    // Start is called before the first frame update
    override public void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        base.Start();
    }

    public override void switchableSetup()
    {
        base.switchableSetup();
        _switchState = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        flip(true);
        _spriteRenderer.sprite = _onSprite;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        flip(false);
        _spriteRenderer.sprite = _offSprite;
    }
}
