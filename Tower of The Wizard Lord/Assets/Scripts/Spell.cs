using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    [SerializeField] float offset;
    [SerializeField] float activeTime;
    float timer;

    private Transform _transform;
    private Collider2D _collider;
    private Animator _animator;

    private delegate void update();
    private update onUpdate;

    // Start is called before the first frame update
    void Start()
    {
        _transform = GetComponent<Transform>();
        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();

        _collider.enabled = false;
        timer = 0;

        onUpdate = noActivity;
    }

    private void Update()
    {
        onUpdate();
    }

    void noActivity() { }


    public void activate(Vector2 position, Vector2 direction)
    {
        _transform.up = direction;
        _transform.position = position + direction * offset;

        timer = activeTime;
        _collider.enabled = true;
        _animator.SetTrigger("Activate");

        onUpdate = advance;
    }

    void advance()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            _collider.enabled = false;

            onUpdate = noActivity;
        }
    }
}
