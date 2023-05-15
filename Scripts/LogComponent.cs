using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogComponent : MonoBehaviour
{
    static LogComponent theOnly;
    [SerializeField]
    string[] log;

    private void Awake()
    {
        if (theOnly == null)
        {
            theOnly = this;
        }
        else if (theOnly != this)
        {
            Debug.LogError("Log Component duplicates on " + gameObject.name + " object. Delete it");
            Destroy(this);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {


        var a = GetLog();
        log = a.Split('\n');


        InvokeTimer();
    }


    public delegate void VoidEvent();
    private static string LogContent = "";
    public static event Action timerEvent = () => { };
    public static int frameCount { get; private set; }
    static void InvokeTimer()
    {
        timerEvent.Invoke();
        frameCount++;
        ClearLog();
    }
    public static void Write(string msg)
    {
        LogContent += "\n" + msg;
    }
    private static void ClearLog()
    {
        LogContent = "";
    }
    public static string GetLog()
    {
        return LogContent;
    }
}

