using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq.Expressions;
using UnityEngine;

public abstract class AnimationSlicer : MonoBehaviour
{
    public abstract string[] animNames { get;  }
    public abstract int[] startFrames { get; }
    public abstract int[] lengths { get;  }
    public abstract string[] partNames { get;}
    public abstract int[] partNumberR { get;}
    public abstract int[] partNumberL { get;}
    [SerializeField]
    protected Sprite[] L;
    [SerializeField]
    protected Sprite[] R;




    protected Movement movement;
    protected Rigidbody2D rb;
    protected Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            movement = GetComponent<Movement>();
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            animator.states.Add(new State("L", 10, 1, () => movement.lastDirX == -1, false));
        }
        catch
        {

        }
        InitializeAnims();
        InitializeStates();


    }
    abstract protected void InitializeStates();

    protected void AddState(string name, int frameLength, Condition condition, bool _onlyOnce)
    {
        animator.states.Add(new State(name, frameLength, GFC(name), condition, _onlyOnce));
    }
    protected void AddState(string name, int frameLength, Func<bool> condition, bool _onlyOnce)
    {
        animator.states.Add(new State(name, frameLength, GFC(name), condition, _onlyOnce));
    }
    protected void AddState(string name, int frameLength, int frameCount, Condition condition, bool _onlyOnce)
    {
        animator.states.Add(new State(name, frameLength, frameCount, condition, _onlyOnce));
    }
    protected void AddState(string name, int frameLength, int frameCount, Func<bool> condition, bool _onlyOnce)
    {
        animator.states.Add(new State(name, frameLength, frameCount, condition, _onlyOnce));
    }
    protected int GFC(string animationName)
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
    // Update is called once per frame
    void Update()
    {

    }
    virtual protected void InitializeAnims()
    {
        if (partNames == null || animNames == null) return;
        foreach (var part in partNames)
        {
            foreach (var name in animNames)
            {
                animator.animations.Add(new Animation(true, part, name, 8, GetSpritesForAnimation(GetSpritesForPart(part, true), name)));
                animator.animations.Add(new Animation(false, part, name, 8, GetSpritesForAnimation(GetSpritesForPart(part, false), name)));
            }
        }
        //foreach (var a in animations) a.PrintInfo();
    }
    protected Sprite[] GetSpritesForPart(string partName, bool side)
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
        var output = new Sprite[L.Length / partNames.Length];
        int line = -1;
        if (side)
        {
            line = partNumberR[num];
        }
        else line = partNumberL[num];
        if (line == -1) throw new System.Exception("Cum");
        for (int i = 0; i < output.Length; i++)
        {
            if (side) output[i] = R[output.Length * line + i];
            else output[i] = L[output.Length * line + i];
        }
        return output;
    }
    protected Sprite[] GetSpritesForAnimation(Sprite[] spritesForPart, string name)
    {
        if (spritesForPart.Length < L.Length / partNames.Length)
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
}

