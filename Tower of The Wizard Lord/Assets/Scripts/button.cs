using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button : Switchable
{

    [SerializeField] Sprite _offSprite;
    [SerializeField] Sprite _onSprite;

    private SpriteRenderer _spriteRenderer;
    private int _weights;

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
        if (_weights == 0)
        {
            flip(true);
            _spriteRenderer.sprite = _onSprite;
        }
        _weights += 1;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _weights -= 1;
        if (_weights <= 0)
        {
            flip(false);
            _weights = 0;
            _spriteRenderer.sprite = _offSprite;
        }
    }
}
