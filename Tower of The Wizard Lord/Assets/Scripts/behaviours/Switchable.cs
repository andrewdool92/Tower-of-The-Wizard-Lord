using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * Class contains behaviour for both switches and objects controlled by switches
 * This allows for some objects to be both, in case we want to implement chained interactions
 * 
 * Switches can be assigned multiple controllers. By default, all attached controllers must be activated to toggle a switch.
 * This behaviour can be changed by overriding the evaluateToggle method.
 * 
 * (Unity doesn't like serializing abstract classes or interfaces, so this combined class will have to do for now :/ )
 */
public class Switchable : MonoBehaviour
{
    private bool _ready;
    public switchRules rule = switchRules.ALL;

    [SerializeField] private AudioClip[] _swtichOnAudioFX;
    [SerializeField] private AudioClip[] _switchOffAudioFX;

    public virtual void Start()
    {
        _ready = false;
        toggles = new Dictionary<int, bool>();
        _switchState = false;
    }

    public virtual void switchableSetup()
    {
        foreach (Switchable controller in controllers)
        {
            toggles[controller.GetInstanceID()] = controller._switchState;
            controller.OnFlip += toggleHandler;
        }
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


    // fields and methods for handling behaviours of a switch
    public virtual event Action<bool, int> OnFlip;

    public bool _switchState { get; set; }

    public virtual void flip(bool state)
    {
        OnFlip?.Invoke(state, this.GetInstanceID());
    }


    // fields and methods for handling behaviours of an object controlled by a switch
    [SerializeField] Switchable[] controllers;
    private Dictionary<int, bool> toggles;

    public bool _state;

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

    private void toggleHandler(bool state, int id)
    {
        toggles[id] = state;
        updateToggleState();
    }

    private void updateToggleState()
    {
        bool previous = _state;
        _state = evaluateToggle();
        Debug.Log($"new state: {_state}");

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
        string t = "";
        foreach (bool value in toggles.Values)
        {
            t += $"{value} ";
        }
        Debug.Log($"evaluating: {t}");
        if (rule == switchRules.ALL)
        {
            return toggles.Values.All(x => x);
        }
        else if (rule == switchRules.ANY)
        {
            return toggles.Values.Any(x => x);
        }
        else
        {
            return false;
        }
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
        //        if (audioClips.Length != 0)
        //        {
        //            int rand = UnityEngine.Random.Range(0, audioClips.Length);
        //            AudioManager.Instance.playSoundClip(audioClips[rand], transform, 0.5f);
        //        }
    }
}

public enum switchRules
{
    ALL,
    ANY
}
