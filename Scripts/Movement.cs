using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class Movement : Perception
{
    [field: SerializeField]
    public int maxSpeed { get; private set; }
    [field: SerializeField]
    public Vector2 lastDir { get; private set; }
    [field: SerializeField]
    public int acceleration { get; private set; }
    [field: SerializeField]
    public int breakAcceleration { get; private set; }
    [field: SerializeField]
    public float slideSpeed { get; private set; }

    [field: SerializeField]
    public int jumpForce { get; private set; }
    private Weapon[] weapon = { new Weapon(26, Weapon.Type.sword), new Weapon(17, Weapon.Type.gun) };
    public Weapon.Type ActiveWeaponType => weapon[activeWeapon].type;
    private int _activeWeapon;
    private int activeWeapon
    {
        get
        {
            return _activeWeapon;
        }
        set
        {
            _activeWeapon = value % weapon.Length;
        }
    }

    public Vector2 input { get; set; }
    public Vector2 attachedTo { get; private set; }
    [SerializeField]
    private int jumpPrepareFrames;
    public int jumpTimer { get; private set; }
    int _aC;
    [field: SerializeField]
    public int attackCounter
    {
        get
        {
            return _aC;
        }
        set
        {
            _aC = value % 3;
        }
    }
    [field: SerializeField]
    public int attackTimer { get; private set; }
    [field: SerializeField]
    public bool attacking { get; private set; }
    [SerializeField]
    List<string> timerActions = new List<string>();
    Action _timerEvent;
    event Action timerAction
    {
        add
        {
            //if (!timerActions.Contains(value.Method.Name))
            //{
            _timerEvent += value;
            // timerActions.Add(value.Method.Name);
            //}
        }
        remove
        {
            _timerEvent -= value;
            // timerActions.Remove(value.Method.Name);
        }
    }


    public bool sliding { get; private set; }
    public bool attached { get; private set; }

    [field: SerializeField]
    public int rollLength { get; private set; }
    [field: SerializeField]
    public int rollSpeed { get; private set; }

    public Routine attackRoutine;
    public Routine attachRoutine = new Routine(5,()=>print(LogComponent.frameCount));
    public Routine rollRoutine;
    public Routine jumpPrepareRoutine;
    private void Start()
    {
        base.Start();
        _timerEvent = () => { };
        attackRoutine = new Routine(weapon[0].time, AttackAction, "attack");
        rollRoutine = new Routine(rollLength, Roll);
        jumpPrepareRoutine = new Routine(jumpPrepareFrames);
        timerAction += jumpPrepareRoutine;
        timerAction += rollRoutine;
        timerAction += attackRoutine;
        timerAction += attachRoutine;
    }
    public bool idling { get; private set; }
    public bool running { get; private set; }
    protected void FixedUpdate()
    {
        base.FixedUpdate();
        GetLastDir();
        sliding = false;
        if (touchWall != 0 && input.y > -0.5) Slide();
        StayAttached();
        Move();
        attacking = (attackRoutine||attackRoutine.callOnExit) && !attached && !attaching;
        attaching = attachRoutine;
        if (onGround)
        {
            if (input.x == 0)
            {
                idling = true;
                running = false;
            }
            else if (input.x == -touchWall)
            {
                idling = true;
                running = false;
            }
            else
            {
                running = true;
                idling = false;
            }
        }
        else
        {
            running = false;
            idling = false;
        }

        _timerEvent.Invoke();
    }
    void Move()
    {
        float currentSpeed = selfRB.velocity.x;
        int dir = Math.Sign(input.x);
        if (attaching) dir = 0;//if attached to wall while standing it shouldn't move
        if (input.x != 0 && currentSpeed * dir < maxSpeed && !sliding) { selfRB.AddForce(Vector2.right * dir * acceleration); }
        if (input.x == 0 && onGround)
        {
            if (Math.Abs(currentSpeed) > 1)
            {
                selfRB.AddForce(Vector2.right * -Math.Sign(currentSpeed) * breakAcceleration);
            }
            else
            {
                selfRB.AddForce(selfRB.mass * currentSpeed / Time.fixedDeltaTime * Vector2.right * -1);
            }
        }
    }
    public void ChangeWeapon()
    {
        if (!attacking) activeWeapon++;
    }
    public void SetInput(Vector2 _input)
    {
        input = _input;
    }
    Action jumpEvent;
    public void Jump(float additionalJumpForce)
    {

        Vector2 jumpVector = Vector2.up;
        if (!onGround)
        {
            jumpVector = new Vector2(touchWall, 1).normalized;
        }

        if ((!onGround && touchWall == 0)) return;
        jumpEvent = () =>
        {
            if(attached)Detach();
            if ((!onGround && touchWall == 0)) return;
            var currentVericalSpeed = selfRB.velocity.y;
            Vector2 compensatingForce = Vector2.up * -currentVericalSpeed / Time.fixedDeltaTime * selfRB.mass;
            selfRB.AddForce(compensatingForce);
            selfRB.AddForce((jumpForce / 2) * (1 + additionalJumpForce) * jumpVector);

        };
        jumpPrepareRoutine.InvokeOnExit(jumpEvent);


        print("Added jump " + jumpPrepareRoutine.active);
    }

    public void Attack()
    {
        if (!attackRoutine)
        {
            attackRoutine.length = weapon[activeWeapon].time;
            attackRoutine.Start();
            attackCounter++;
        }
        else if (!attackRoutine.callOnExit)
        {
            attackRoutine.InvokeOnExit(Attack);
        }
    }
    public void Roll()
    {
        attachRoutine.Stop();
        attackRoutine.Stop();
        if (!onGround || attached)
        {
            rollRoutine.Stop();
            return;
        }
        if (selfRB.velocity.x * lastDirX < rollSpeed) selfRB.AddForce(Vector2.right * lastDirX * 5 * acceleration);
        if (!rollRoutine.callOnExit) rollRoutine.InvokeOnExit(StopRolling);
    }
    void StopRolling()
    {
        selfRB.AddForce(selfRB.mass * (selfRB.velocity.x - maxSpeed * Math.Sign(selfRB.velocity.x)) / Time.fixedDeltaTime * Vector2.right * -1);
    }
    void AttackAction()
    {
        if (rollRoutine) return;
        if (attached) Detach();
        else
        {
            Attach();
            Debug.DrawRay(transform.localPosition, lastDir, Color.blue);
        }
    }
    [field: SerializeField]
    public int lastDirX { get; private set; } = 1;
    void GetLastDir()
    {
        if (attaching && touchWall == 0 && FloorSurface != null && FloorSurface.Hard)
        {
            return;
        }
        if (rollRoutine) return;
        if (attached || attaching)
        {
            return;
        }
        if (input != Vector2.zero)
        {
            if (!attacking && !attached)
            {
                if (input.y < -0.7) lastDir = Vector2.down;
                else { lastDir = Vector2.right * Math.Sign(input.x); if (Math.Sign(input.x) != 0) lastDirX = Math.Sign(input.x); }

            }
        }
        if (sliding && isHardWall)
        {
            lastDir = Vector2.right * touchWall;
            if (touchWall != 0) lastDirX = touchWall;
        }


        Debug.DrawRay(transform.position, lastDir, Color.red);
    }
    Collider2D selfColl;
    bool CastAttachingRay()
    {
        if (selfColl == null) selfColl = GetComponent<Collider2D>();
        var hit = Physics2D.RaycastAll(transform.position + selfColl.bounds.extents.y * Vector3.up, Vector2.right * -touchWall,selfColl.bounds.extents.x+1f);

        Surface surf = null;
        foreach(var h in hit)
        {
            if (h.collider.TryGetComponent(out surf)) break;
        }

        if (surf != null && WallSurface != null&& surf.id == WallSurface.id)
        {
            if (input.y != -1) return !isHardWall;
            else if (FloorSurface != null) return !FloorSurface.Hard;
            return false;
        }
        return false;
    }
    void Slide()
    {
        if (onGround || attached|| !isHardWall) return;
        var currentVericalSpeed = selfRB.velocity.y;
        sliding = true;
        if (currentVericalSpeed > 0) return;
        Vector2 compensatingForce = Vector2.up * -Physics2D.gravity.y * selfRB.gravityScale * selfRB.mass;
        int i = 1;
        if (!isHardWall) i = 0;
        if (currentVericalSpeed < -slideSpeed) selfRB.AddForce(compensatingForce * i);

    }
    void StayAttached()
    {
        if (!attached)
        {
            selfRB.constraints = RigidbodyConstraints2D.FreezeRotation;
            return;
        }
        selfRB.constraints = RigidbodyConstraints2D.FreezeAll;
    }
    [field: SerializeField]
    public bool attaching { get; private set; } = false;
    void Attach()
    {
        if (ActiveWeaponType != Weapon.Type.sword) return;
        if (!CastAttachingRay()) return;
        if (touchWall != 0)
        {
            lastDirX = -touchWall;
            attachedTo = Vector2.right * lastDirX;
        }
        else attachedTo = Vector2.down;
        attaching = true;
        attachRoutine.Start();
        attached = true;
        attackRoutine.Stop();
        jumpPrepareRoutine.Stop();
    }
    void Detach()
    {
        attaching = true;
        attachRoutine.Start();
        attached = false;
        attackRoutine.Stop();
        StayAttached();
    }
}
public class Weapon
{
    public Weapon(int t, Type _type) { time = t; type = _type; }
    public int time { get; private set; }
    public enum Type { sword, gun }
    public Type type { get; private set; }
}
