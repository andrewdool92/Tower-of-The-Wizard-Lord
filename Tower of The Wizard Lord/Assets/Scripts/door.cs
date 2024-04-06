using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door : Switchable
{
    [SerializeField] bool start_open;
    [SerializeField] Sprite[] topSprites;
    [SerializeField] Sprite[] bottomSprites;

    private bool _isOpen;

    private SpriteRenderer _topSprite;
    private SpriteRenderer _bottomSprite;
    private Collider2D _collider;


    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();
        _collider = GetComponent<Collider2D>();

        _topSprite = transform.Find("Door_top").GetComponent<SpriteRenderer>();
        _bottomSprite = transform.Find("Door_bottom").GetComponent <SpriteRenderer>();

        setDoorState(start_open);
    }


    public void setDoorState(bool open)
    {
        int index = open ? 0 : 1;
        _topSprite.sprite = topSprites[index];
        _bottomSprite.sprite = bottomSprites[index];
        
        _collider.enabled = !open;
        _isOpen = open;
        _state = open;
    }

    public void toggleDoorState()
    {
        setDoorState(!_isOpen);
    }

    public override void onStateChange(bool state)
    {
        setDoorState(state);
    }

}
