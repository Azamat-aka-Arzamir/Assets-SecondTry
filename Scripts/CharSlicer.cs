using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharSlicer : AnimationSlicer
{
    public override string[] animNames
    {
        get => new string[]{
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
        "Attached Wall",
        "Aim Up",
        "Aim Down"
    };
    }
    public override int[] startFrames
    {
        get => new int[]{
        0,3,5,7,11,13,17,19,21,23,25,31,36,40,44,48,50,54,55,56,58,58,3,5
    };
    }
    public override int[] lengths
    {
        get => new int[]
    {
        3,2,2,4,2,4,2,2,2,2,6,5,4,4,4,2,4,1,1,2,2,1,1,1
    };
    }
    public override string[] partNames
    {
        get => new string[]{
        "Shield","Far hand","Legs","Body","Head","Crest","Sword","Close hand","Gun"
        };
    }
    public override int[] partNumberR
    {
        get => new int[]
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8
        };
    }
    public override int[] partNumberL
    {
        get => new int[]
        {
            8, 0, 2, 3, 4, 5, 1, 6, 7
        };
    }
    protected override void InitializeStates()
    {
        Debug.Log("startInit");
        animator.states.Add(new State("Attack 1", 7, GFC("Attack 1"), () => movement.attacking && movement.attackCounter == 0 && movement.ActiveWeaponType == Weapon.Type.sword, false));
        animator.states.Add(new State("Idle", 10, GFC("Idle"), () => movement.idling, false));
        animator.states.Add(new State("Jump", 10, GFC("Jump"), () => movement.onGround && movement.jumpTimer != 0, false));
        animator.states.Add(new State("Fly Up", 10, GFC("Fly Up"), () => !movement.onGround && !(movement.sliding) && rb.velocity.y > 0, false));
        animator.states.Add(new State("Fly Down", 10, GFC("Fly Down"), () => !movement.onGround && !(movement.sliding) && rb.velocity.y <= 0, false));
        animator.states.Add(new State("Land", 7, GFC("Land"), () => movement.onGround, true));
        animator.states.Add(new State("Run", 7, GFC("Run"), () => movement.running, false));
        animator.states.Add(new State("Attack 2", 7, GFC("Attack 2"), () => (movement.attacking) && movement.attackCounter == 1 && movement.ActiveWeaponType == Weapon.Type.sword, false));
        animator.states.Add(new State("Attack 3", 7, GFC("Attack 3"), () => movement.attacking && movement.attackCounter == 2 && movement.ActiveWeaponType == Weapon.Type.sword, false));
        animator.states.Add(new State("Slide", 10, GFC("Slide"), () => movement.sliding && movement.isHardWall, false));
        animator.states.Add(new State("Attach Down", 5, GFC("Attach Down"), () => movement.attached && movement.attaching && movement.attachedTo == Vector2.down, true));
        animator.states.Add(new State("Attached Down", 10, GFC("Attached Down"), () => movement.attached && movement.attachedTo == Vector2.down, false));
        animator.states.Add(new State("Detach Down", 10, GFC("Detach Down"), () => !movement.attached && movement.attaching && movement.attachedTo == Vector2.down, false));
        animator.states.Add(new State("Detach Wall", 5, GFC("Detach Wall"), () => !movement.attached && movement.attaching && movement.attachedTo != Vector2.down, true));
        animator.states.Add(new State("Attached Wall", 10, GFC("Attached Wall"), () => movement.attached && movement.attachedTo != Vector2.down, false));
        animator.states.Add(new State("Flip", 7, GFC("Flip"), () => movement.rollRoutine, false));

        animator.states.Add(new State("Idle Gun", 7, GFC("Idle Gun"), () => !movement.attacking && movement.ActiveWeaponType == Weapon.Type.gun && movement.input.y == 0, false));
        animator.states.Add(new State("Shoot", 8, GFC("Shoot"), () => movement.attacking && movement.ActiveWeaponType == Weapon.Type.gun && movement.input.y == 0, false));
        animator.states.Add(new State("Shoot Up", 11, GFC("Shoot Up"), () => movement.attacking && movement.ActiveWeaponType == Weapon.Type.gun && movement.input == Vector2.up, false));
        animator.states.Add(new State("Shoot Down", 11, GFC("Shoot Down"), () => movement.attacking && movement.ActiveWeaponType == Weapon.Type.gun && movement.input == Vector2.down, false));
        animator.states.Add(new State("Aim Up", 7, GFC("Aim Up"), () => movement.ActiveWeaponType == Weapon.Type.gun && movement.input == Vector2.up, false));
        animator.states.Add(new State("Aim Down", 7, GFC("Aim Down"), () => movement.ActiveWeaponType == Weapon.Type.gun && movement.input == Vector2.down, false));
    }
}
