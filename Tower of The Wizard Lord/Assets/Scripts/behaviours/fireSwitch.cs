using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireSwitch : Switchable
{

    [SerializeField] Flame flameObject;
    [SerializeField] bool startLit;
    private Flame _flame;


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        _flame = Instantiate(flameObject, this.transform, false);

        _switchState = startLit;
    }

    public override void switchableSetup()
    {
        base.switchableSetup();
        _flame.setFlameState(startLit);
        //_switchState = startLit;
        _flame.stateChange = flip;
    }

}
