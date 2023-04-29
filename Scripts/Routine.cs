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
    void Update()
    {
        if (timer < length)
        {
            timer++;
            action();
            active = true;
        }
        else if (active)
        {
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
        timer = _length;
    }
    public Routine(int _length, Action _action)
    {
        length = _length;
        action = _action;
        timer = _length;
    }
    public Routine(int _length, Action _action,string _name)
    {
        length = _length;
        action = _action;
        name = _name;
        timer = _length;
    }
    public void Start()
    {
        timer = 0;
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
