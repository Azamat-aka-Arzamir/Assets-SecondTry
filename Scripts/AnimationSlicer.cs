using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class AnimationSlicer : MonoBehaviour
{
    public static readonly string[] animNames = {
        "Shoot",
        "Shoot Up",
        "Shoot Down",
        "Idle",
        "Def",
        "Attack 1",
        "Jump",
        "Fly Up",
        "Fly Down",
        "Land",
        "Run",
        "Flip",
        "Attack 2",
        "Attack 3",
        "Idle Gun",
        "Slide",
        "Attach Down",
        "Attached Down",
        "Detach Down",
        "Sliding Face Wall",
        "Detach Wall",
        "Attached Wall"
    };
    public static readonly int[] startFrames =
    {
        0,3,5,7,11,13,17,19,21,23,25,31,36,40,44,48,50,54,55,56,58,58
    };
    public static readonly int[] lengths =
    {
        3,2,2,4,2,4,2,2,2,2,6,5,4,4,4,2,4,1,1,2,2,1
    };
    public static readonly string[] partNames =
    {
        "Shield","Far hand","Legs","Body","Head","Crest","Sword","Close hand","Gun"
    };
    public static readonly int[] partNumberR = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
    public static readonly int[] partNumberL = { 8, 0, 2, 3, 4, 5, 1, 6, 7 };
    [SerializeField]
    Sprite[] L;
    [SerializeField]
    Sprite[] R;

    [field: SerializeField]
    public static Sprite[] spritesL { get; private set; }
    [field: SerializeField]
    public static Sprite[] spritesR { get; private set; }

    public static List<Animation> animations = new List<Animation>();

    Movement movement;
    Rigidbody2D rb;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        spritesL = L;
        spritesR = R;
        InitializeAnims();
        movement = GetComponent<Movement>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.states.Add(new State("Attack 1", 7, GFC("Attack 1"), () => movement.attacking && movement.attackCounter == 0, true));
        animator.states.Add(new State("Idle", 10, GFC("Idle"), () => movement.onGround && Mathf.Abs(rb.velocity.x) <= 0.1&&movement.input.x==0, false));
        animator.states.Add(new State("Jump", 10, GFC("Jump"), () => movement.onGround && movement.jumpTimer != 0, false));
        animator.states.Add(new State("Fly Up", 10, GFC("Fly Up"), () => !movement.onGround &&( movement.touchWall == 0 ||!movement.isHardWall)&& rb.velocity.y > 0, false));
        animator.states.Add(new State("Fly Down", 10, GFC("Fly Down"), () => !movement.onGround && (movement.touchWall == 0 || !movement.isHardWall) && rb.velocity.y <= 0, false));
        animator.states.Add(new State("Land", 7, GFC("Land"), () => movement.onGround, true));
        animator.states.Add(new State("Run", 7, GFC("Run"), () => movement.onGround && Mathf.Abs(rb.velocity.x) > 0.1, false));
        animator.states.Add(new State("Attack 2", 7, GFC("Attack 2"), () => (movement.attacking) && movement.attackCounter == 1, true));
        animator.states.Add(new State("Attack 3", 7, GFC("Attack 3"), () => movement.attacking && movement.attackCounter == 2, true));
        animator.states.Add(new State("Slide", 10, GFC("Slide"), () => movement.sliding&&movement.isHardWall, false));
        animator.states.Add(new State("Attach Down", 10, GFC("Attach Down"), () => movement.attached && movement.attaching && movement.lastDir == Vector2.down, true));
        animator.states.Add(new State("Attached Down", 10, GFC("Attached Down"), () => movement.attached && movement.lastDir == Vector2.down, false));
        animator.states.Add(new State("Detach Down", 10, GFC("Detach Down"), () => !movement.attached&&movement.attaching && movement.lastDir == Vector2.down, true));
        animator.states.Add(new State("Detach Wall", 10, GFC("Detach Wall"), () => !movement.attached && movement.attaching && movement.lastDir != Vector2.down, true));
        animator.states.Add(new State("Attached Wall", 10, GFC("Attached Wall"), () => movement.attached && movement.lastDir != Vector2.down, false));
        animator.states.Add(new State("L", 10, 1, () => movement.lastDirX == -1, false));

    }
    static int GFC(string animationName)
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
    void InitializeAnims()
    {
        foreach (var part in partNames)
        {
            foreach (var name in animNames)
            {
                animations.Add(new Animation(true, part, name, 8));
                animations.Add(new Animation(false, part, name, 8));
            }
        }
        foreach (var a in animations) a.PrintInfo();
    }
    public static Sprite[] GetSpritesForPart(string partName, bool side)
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
            print("afags + " + output.Length);
            if (side) output[i] = spritesR[output.Length * line + i];
            else output[i] = spritesL[output.Length * line + i];
        }
        return output;
    }
    public static Sprite[] GetSpritesForAnimation(Sprite[] spritesForPart, string name)
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
}
public class Animation
{
    public int length { get => sprites.Length; }
    public int speed { get; private set; }
    public string partName { get; private set; }
    public string name { get; private set; }
    public string side { get; private set; }
    public int intSide { get; private set; }
    public bool boolSide { get; private set; }
    public Sprite[] sprites { get; private set; }
    public Animation(bool _side, string _partName, string _name, int _speed)
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
        sprites = AnimationSlicer.GetSpritesForAnimation(AnimationSlicer.GetSpritesForPart(_partName, _side), _name);
    }

    public Sprite GetSprite(int frame)
    {
        if (frame > length) { Debug.LogError("Frame exceed length in " + partName + " " + name); return sprites[0]; }
        return sprites[frame];
    }
    public void PrintInfo()
    {
        Debug.Log(partName+" "+name+" "+side+" " + sprites[0].name + sprites[sprites.Length-1].name);
    }
}
