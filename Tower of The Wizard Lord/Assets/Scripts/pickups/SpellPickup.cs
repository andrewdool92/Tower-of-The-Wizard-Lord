using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellPickup : BasePickup
{
    [SerializeField] spellType spell;
    [SerializeField] ParticleSystem spellParticles;
    [SerializeField] ParticleSystem spellAura;

    private Animator _baseAnimator;
    private CircleCollider2D _bookCollider;

    private ParticleSystem _mainParticles;
    private ParticleSystem _aboveAura;
    private ParticleSystem _auraParticles;

    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();
        _baseAnimator = transform.Find("base").GetComponent<Animator>();

        Transform book = transform.Find("book");
        _bookCollider = book.GetComponent<CircleCollider2D>();
        Debug.Log(_bookCollider.isTrigger);

        _animator = book.GetComponent<Animator>();
        setupParticleSystems(book);

        playParticles();
    }

    private void setupParticleSystems(Transform bookTransform)
    {
        _mainParticles = Instantiate(spellParticles, bookTransform);
        _auraParticles = Instantiate(spellAura, bookTransform);

        _aboveAura = Instantiate(spellAura, bookTransform);
        _aboveAura.transform.position += Vector3.up * 100;

        var aboveMain = _aboveAura.main;
        aboveMain.prewarm = true;
    }

    private void playParticles()
    {
        _mainParticles.Play();
        _aboveAura.Play();
        _auraParticles.Play();
    }

    private void stopParticles()
    {
        _mainParticles.Stop();
        _aboveAura.Stop();
        _auraParticles.Stop();
    }
    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        basePickup();

        if (collision.gameObject.TryGetComponent(out spellSelector player))
        {
            _bookCollider.enabled = false;
            player.enableSpell(spell);

            stopParticles();
            _baseAnimator.SetTrigger("pickup");
            _animator.SetTrigger("pickup");
        }
    }
}
