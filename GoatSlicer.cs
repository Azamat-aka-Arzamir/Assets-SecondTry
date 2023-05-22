using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoatSlicer : AnimationSlicer
{
    public override string[] animNames => null;

    public override int[] startFrames => null;

    public override int[] lengths => null;

    public override string[] partNames => null;

    public override int[] partNumberR => null;

    public override int[] partNumberL => null;

    protected override void InitializeStates()
    {
        Debug.Log("sfgdg");
        var goatM = GetComponent<GoatBehavior>();
        AddState("Walk", 7, 4, () => movement.running, false);
        AddState("Start Eat", 7, 1, () => goatM.eating, true);
        AddState("Stop Eat", 7, 1, () => !goatM.eating, true);
        AddState("Eat", 7, 1, () => goatM.eating , false);
        AddState("Idle", 7, 1, () => !movement.running, false);
        AddState("Die", 7, 4, () => goatM.dying, false);
    }
}


