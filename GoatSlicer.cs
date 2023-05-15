using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoatSlicer : AnimationSlicer
{
    public new static readonly string[] animNames = {
        "Walk",
        "Idle",
        "Eat",
        "Die"
    };
    public new static readonly int[] startFrames =
    {
        0,1,4,7
    };
    public new static readonly int[] lengths =
    {
        4,1,3,4
    };
    public new static readonly string[] partNames =
    {
        "Body"
    };
    public new static readonly int[] partNumberR = { 0 };
    public new static readonly int[] partNumberL = { 0 };

    void Start()
    {
        spritesL = L;
        spritesR = R;
        InitializeAnims();
        movement = GetComponent<Movement>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.states.Add(new State("Walk", 7, GFC("Walk"), () => movement.running, false));
        animator.states.Add(new State("Idle", 7, GFC("Idle"), () => movement.idling, false));
        animator.states.Add(new State("L", 10, 1, () => movement.lastDirX == -1, false));
    }
    override protected void InitializeAnims()
    {
        foreach (var part in partNames)
        {
            foreach (var name in animNames)
            {
                animations.Add(new Animation(true, part, name, 8, this));
                animations.Add(new Animation(false, part, name, 8, this));
            }
        }
        foreach (var a in animations) a.PrintInfo();
    }
     public override Sprite[] GetSpritesForPart(string partName, bool side)
    {
        int num = -1;
        for (int i = 0; i < partNames.Length; i++)
        {
            if (partNames[i] == partName) { num = i; break; }
        }
        if (num == -1)
        {
            Debug.LogError("Part named " + partName + " not found");
            return null;
        }
        var output = new Sprite[spritesL.Length / partNames.Length];
        int line = -1;
        if (side)
        {
            line = partNumberR[num];
        }
        else line = partNumberL[num];
        if (line == -1) throw new System.Exception("Cum");
        for (int i = 0; i < output.Length; i++)
        {
            if (side) output[i] = spritesR[output.Length * line + i];
            else output[i] = spritesL[output.Length * line + i];
        }
        return output;
    }
    public override Sprite[] GetSpritesForAnimation(Sprite[] spritesForPart, string name)
    {
        if (spritesForPart.Length < spritesL.Length / partNames.Length)
        {
            Debug.LogError("Invalid array of sprites in " + name);
            return null;
        }
        int num = -1;
        for (int i = 0; i < animNames.Length; i++)
        {
            if (animNames[i] == name) { num = i; break; }
        }
        if (num == -1)
        {
            Debug.LogError("Animation named " + name + " not found");
            return null;
        }
        var output = new Sprite[lengths[num]];
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = spritesForPart[startFrames[num] + i];
        }
        return output;
    }
    override protected int GFC(string animationName)
    {
        int num = -1;
        for (int i = 0; i < animNames.Length; i++)
        {
            if (animNames[i] == animationName) { num = i; break; }
        }
        if (num == -1)
        {
            Debug.LogError("Animation named " + animationName + " not found");
            return 0;
        }
        return lengths[num];
    }
}
