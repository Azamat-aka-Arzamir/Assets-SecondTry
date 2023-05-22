using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using JetBrains.Annotations;

[Serializable]
public class Animation
{
    [SerializeField]
    private int _speed;
    [SerializeField]
    private string _partName;
    [SerializeField]
    private string _name;
    [SerializeField]
    private string _side;
    [SerializeField]
    private Sprite[] _sprites;


    public int length { get => sprites.Length; }
    public int speed { get => _speed; private set => _speed = value; }
    public string partName { get => _partName; private set => _partName = value; }
    public string name { get => _name; private set => _name = value; }
    public string side { get => _side; private set => _side = value; }
    public int intSide { get; private set; }
    public bool boolSide { get; private set; }
    public Sprite[] sprites { get=>_sprites; private set=>_sprites=value; }
    
    public Sprite this[int i]
    {
        get=>GetSprite(i);
    }
#if UNITY_EDITOR
    public void ValidateData()
    {
        if (side.StartsWith("r") || side.StartsWith("R"))
        {
            side = "Right";
            intSide = 1;
            boolSide = true;
        }
        else
        {
            side = "Left";
            intSide = -1;
            boolSide = false;
        }
    }
#endif
    public Animation(bool _side, string _partName, string _name, int _speed, Sprite[] _sprites)
    {
        if (_side)
        {
            side = "Right";
            intSide = 1;
            boolSide = _side;
        }
        else
        {
            side = "Left";
            intSide = -1;
            boolSide = _side;
        }
        partName = _partName;
        name = _name;
        speed = _speed;
        sprites = _sprites;
    }

    public Sprite GetSprite(int frame)
    {
        if (frame > length) { Debug.LogError("Frame exceed length in " + partName + " " + name); return sprites[0]; }
        return sprites[frame];
    }
    public void PrintInfo()
    {
        Debug.Log(partName + " " + name + " " + side + " " + sprites[0].name + sprites[sprites.Length - 1].name);
    }
}
/*
[CustomPropertyDrawer(typeof(Animation))]
public class AnimationDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        List<Rect> rects = new List<Rect>();
        int offset = 0;
        foreach(var prop in typeof(Animation).GetProperties())
        {
            var rect = new Rect(position.x + offset, position.y, 100, position.height);
            rects.Add(rect);
            offset += 30;
            switch (prop.PropertyType.Name)
            {
                case "String":
                    {
                        EditorGUI.TextField(rect, prop.GetValue(property));
                    }
                default:
                    {
                        Debug.Log(prop.PropertyType.Name);
                        break;
                    }
            }

        }

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}*/
