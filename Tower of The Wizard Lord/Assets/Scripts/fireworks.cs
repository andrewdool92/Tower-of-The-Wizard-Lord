using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireworks : MonoBehaviour
{
    [SerializeField] ParticleSystem _redParticles;
    [SerializeField] ParticleSystem _blueParticles;
    public float startDelay;
    private float _timer;
    private bool _active = false;

    private delegate void OnUpdate(float dt);
    OnUpdate onUpdate;

    void Start()
    {
        _redParticles = Instantiate(_redParticles, transform);
        _blueParticles = Instantiate(_blueParticles, _redParticles.transform);

        _redParticles.gameObject.SetActive(false);

        onUpdate = doNothing;
    }

    private void Update()
    {
        onUpdate(Time.unscaledDeltaTime);
    }

    private void _countdown(float dt)
    {
        _timer -= dt;
        if (_timer < 0 && !_active)
        {
            activateParticles();
            onUpdate = doNothing;
        }
    }

    public void startTimer()
    {
        _timer = startDelay;
        onUpdate = _countdown;
    }

    private void doNothing(float dt) { }

    public void activateParticles()
    {
        Debug.Log("fireworks activated");
        _redParticles.gameObject.SetActive(true);
    }
}
