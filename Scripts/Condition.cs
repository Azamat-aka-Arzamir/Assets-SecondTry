using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine.Events;
using UnityEngine;
using System.Linq.Expressions;

[System.Serializable]
public class Condition
{
    /// <summary>
    /// Reference to field
    /// </summary>
    public FieldInfo property;
    /// <summary>
    /// Type info
    /// </summary>
    public System.Reflection.TypeInfo typeRef;

    public string objectID;
    /// <summary>
    /// Value to compare with
    /// </summary>
    public object value;
    public bool eventinvoked;
    [System.NonSerialized]
    public Component objectRef;
    //You can create simplified condition using script. It will contain bool delegate and thats' all.
    public bool simplified { get; private set; } = false;
    Func<bool> func;
    public static implicit operator Func<bool>(Condition c){ if (!c.simplified) return c.IsTrue; else return c.func; }
    public static implicit operator Condition(Func<bool> _func)=>new Condition(_func);
    public static implicit operator bool(Condition c) => ((Func<bool>)c)();
    public enum CondType { E, G, L, LOE, GOE, NE, CONTAINS, NOTCONTAIN }
    public CondType type;
    /// <summary>
    /// Checks condition and needs invoker in case if component is local
    /// </summary>
    /// <param name="invoker">object that called this function. It's supposed to be an animator</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public bool IsTrue()
    {
        var a = property.GetValue(objectRef);
        if (property.FieldType == typeof(string) && type == CondType.CONTAINS)
        {
            return (a as String).Contains(value as string);

        }
        if (property.FieldType == typeof(string) && type == CondType.NOTCONTAIN)
        {
            return !(a as String).Contains(value as string);
        }
        if (property.FieldType == typeof(string) || property.FieldType == typeof(bool))
        {
            if (value == null && property.FieldType == typeof(string))
            {
                value = string.Empty;
            }
            else if (value == null && (property.FieldType == typeof(bool)))
            {
                value = false;
            }
            switch (type)
            {
                case CondType.E:
                    if (a.Equals(value)) return true;
                    else
                    {
                        //DELETE
                        //invoker.GetComponent<CustomAnimator>().ALog("           Failed condition: " + property.Name + " equals to " + value + ". Actual value is" + a);
                        return false;
                    }

                case CondType.NE:
                    if (!a.Equals(value)) return true;
                    else
                    {
                        //DELETE
                        //invoker.GetComponent<CustomAnimator>().ALog("           Failed condition: " + property.Name + "not equals to " + value + ". Actual value is" + a);
                        return false;
                    }
                default:
                    throw new Exception("Wrong operation in some condition (FIND IT BY YOURSELF, BITCH!)\n" + "ok, property holder on " + objectRef + " and its name is  " + property.Name);
            }
        }
        else if (property.FieldType == typeof(UnityEvent))
        {
            if (eventinvoked)
            {
                //DELETE
                //invoker.GetComponent<CustomAnimator>().ALog(property.Name + " were invoked and cond passed");
                return true;
            }
            else
            {
                //DELETE
                //invoker.GetComponent<CustomAnimator>().ALog("           Failed condition: " + property.Name + " wasn't invoked");
                return false;
            }

        }
        else
        {
            if (value == null) value = 0;
            float b = 0;
            float c = 0;
            if (a.GetType() == typeof(int))
            {
                b = (int)a;
                c = Convert.ToInt32(value);
            }
            else if (a.GetType() == typeof(float))
            {
                b = (float)a;
                c = Convert.ToSingle(value);
            }
            else if (a.GetType() == typeof(double))
            {
                b = (float)(double)a;
                c = Convert.ToSingle(value);
            }

            switch (type)
            {
                case CondType.E:
                    if (b == c) return true;
                    break;
                case CondType.G:
                    if (b > c) return true;
                    break;
                case CondType.L:
                    if (b < c) return true;
                    break;
                case CondType.GOE:
                    if (b >= c) return true;
                    break;
                case CondType.LOE:
                    if (b <= c) return true;
                    break;
                case CondType.NE:
                    if (b != c) return true;
                    break;
            }
            //DELETE
            //invoker.GetComponent<CustomAnimator>().ALog("           Failed condition: " + property.Name + " is not " + type.ToString() + " to " + value + ". Actual value is" + a);
            return false;
        }
    }
    public string GetInfo()
    {
        string output = objectID + ", ";
        try
        {
            output += "object name: " + objectRef.name + ", ";
        }
        catch (Exception e) { };
        try
        {
            output += "type: " + typeRef.Name + ", ";
        }
        catch (Exception e) { };
        try
        {
            output += "prop: " + property.Name + ", ";
        }
        catch (Exception e) { };
        return output + "loaded: " + (objectRef != null);
    }

    //Call at load
    public void FindObject()
    {
        if (objectRef != null) return;//already have been loaded correctly
        if (typeRef == null || objectID == null || objectID == "") return;
        List<UnityEngine.Object> list = new List<UnityEngine.Object>();
        list.AddRange(UnityEngine.Object.FindObjectsOfType(typeof(IDCard)));
        var obj = list.Find((x) => (x as IDCard).ID == objectID);
        if (obj != null) objectRef = (obj as IDCard).gameObject.GetComponent(typeRef);
        if (property != null && property.FieldType == typeof(UnityEvent))
        {
            (property.GetValue(objectRef) as UnityEvent).AddListener(() => { ResetInvokeBool(); });
        }
    }
    void ResetInvokeBool()
    {
        eventinvoked = true;
    }
    public Condition(Func<bool> _func)
    {
        func = _func;
        simplified = true;
    }
}
