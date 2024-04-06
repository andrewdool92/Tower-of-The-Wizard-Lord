using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class toggleable : MonoBehaviour
{ 
    [SerializeField] toggle[] controllers;
    [SerializeField] ToggleType toggleType;
    private Dictionary<int, bool> toggles;

    private bool _state;
    private bool _ready;

    [SerializeField] private AudioClip[] _swtichOnAudioFX;
    [SerializeField] private AudioClip[] _switchOffAudioFX;

    public virtual void Start()
    {
        _ready = false;
        toggles = new Dictionary<int, bool>();
    }

    public virtual void switchableSetup()
    {
        foreach (toggle controller in controllers)
        {
            StartCoroutine(hookupToggle(this, toggles, controller));
        }
    }

    public static IEnumerator hookupToggle(toggleable obj, Dictionary<int, bool> toggles, toggle controller)
    {
        if (controller == null)
        {
            yield break;
        }
        toggles[controller.GetInstanceID()] = controller._switchState;
        controller.OnFlip += obj.toggleHandler;
        yield return null;
    }

    public virtual void Update()
    {
        if (!_ready)
        {
            try
            {
                switchableSetup();  // necessary to avoid race condition when relying on other entities
                _ready = true;
            }
            catch (NullReferenceException e)
            {
                _ready = false;
                Debug.Log($"Setup delayed: {this.name}" +
                    $"\n{e.StackTrace}");
            }
        }
    }

    public virtual void OnDestroy()
    {
        if (_ready)
        {
            for (int i = 0; i < controllers.Length; ++i)
            {
                controllers[i].OnFlip -= toggleHandler;
            }
        }
    }

    public void toggleHandler(bool state, int id)
    {
        toggles[id] = state;
        updateToggleState();
    }

    private void updateToggleState()
    {
        bool previous = _state;
        _state = evaluateToggle();

        if (_state != previous)
        {
            onStateChange(_state);
            _playSwitchAudio(_state);
        }
    }

    /**
     * evaluate whether the rules for flipping the switch have been satisfied
     * can be overridden if different rules are preferred
     */
    public virtual bool evaluateToggle()
    {
        return toggles.Values.All(x => x);
    }

    /**
     * method to be called when the switch's state has been changed
     * this has no default implementation and must be overridden by derived classes
     */
    public virtual void onStateChange(bool state)
    {

    }

    private void _playSwitchAudio(bool nextState)
    {
        AudioClip[] audioClips = nextState ? _swtichOnAudioFX : _switchOffAudioFX;
        AudioManager.Instance.playRandomClip(audioClips, transform, 0.5f);
    }
}

public enum ToggleType
{
    all,
    any
}
