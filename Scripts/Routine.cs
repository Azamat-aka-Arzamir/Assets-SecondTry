using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine.PlayerLoop;

public class Routine
{
    public readonly string name;
    private int timer = 0;
    public int length;
    public Action action;
    private Action exit;
    public bool callOnExit { get; private set; }
    public delegate void A();
    public bool active { get; set; }

    public static implicit operator Action(Routine r) => r.Update;
    public static implicit operator bool(Routine r) => r.active;
    private int lastUpdate=0;
    void Update()
    {
        if (lastUpdate == LogComponent.frameCount) return;
        lastUpdate=LogComponent.frameCount;
        if (timer < length)
        {
            timer++;
            action();
            active = true;
        }
        else if (active)
        {
            lastUpdate = 0;
            active = false;
            exit();
            exit = () => { };
            callOnExit = false;
        }
    }
    public Routine(int _length)
    {
        length = _length;
        action = () => { };
        exit = () => { };
        timer = _length;
    }
    public Routine(int _length, Action _action)
    {
        length = _length;
        action = _action;
        exit = () => { };
        timer = _length;
    }
    public Routine(int _length, Action _action,string _name)
    {
        length = _length;
        action = _action;
        exit = () => { };
        name = _name;
        timer = _length;
    }
    public void Start()
    {
        timer = 0;
        Update();
    }
    public void Stop()
    {
        timer = length;
        exit = () => { };
        active = false;
        callOnExit = false;
    }
    public void InvokeOnExit(Action _action)
    {
        exit = _action;
        callOnExit = true;
    }
}
