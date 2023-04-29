using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animator : MonoBehaviour
{
    public List<State> states;
    public delegate void AnimatorStateDelegate();
    public event AnimatorStateDelegate animatorStateTimer = () => { };
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public int activeStates;
    // Update is called once per frame
    void FixedUpdate()
    {
        activeStates = 0;
        foreach (var st in states)
        {
            st.Check();
            if (st.active)
            {
                activeStates++;
                LogComponent.Write(st.name + " " + st.frameNumber);
            }
        }
        animatorStateTimer.Invoke();
    }
}
[Serializable]
public delegate bool Condition();
[Serializable]
public class State
{
    public bool onlyOnce { get; private set; }
    private int _frameNumber;
    public int frameNumber
    {
        get
        {
            return _frameNumber;
        }
        private set
        {
            _frameNumber = value % frameCount;
            if(value>=frameCount&&onlyOnce)active = false;
        }
    }
    public bool active { get; private set; }
    public readonly string name;
    public readonly int frameLength;
    public readonly int frameCount;
    readonly Condition condition;
    int lastFrame;
    public State(string _name, int _frameLength, int _frameCount, Condition _condition, bool _onlyOnce)
    {
        lastFrame = 0;
        _frameNumber = 0;
        active = false;
        name = _name;
        frameLength = _frameLength;
        if (frameLength == 0) frameLength = 1;
        frameCount = _frameCount;
        if (frameCount == 0) frameCount = 1;
        condition = _condition;
        frameNumber = 0;
        onlyOnce = _onlyOnce;
    }
    bool lastCondition = true;
    public void Check()
    {
        if (!onlyOnce)
        {
            active = condition();
        }
        else
        {
            if (!active)
            {
                if (!lastCondition)
                {
                    active = condition();
                }
                lastCondition = condition();
            }
        }
        var dif = LogComponent.frameCount - lastFrame;
        if (dif / frameLength > 0)
        {
            lastFrame = LogComponent.frameCount;
            if (active) frameNumber ++;
            else frameNumber = 0;
        }
    }
}
