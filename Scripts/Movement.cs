using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class Movement : Perception
{
    [field: SerializeField]
    public int maxSpeed { get; private set; }
    public Vector2 lastDir { get; private set; }
    [field: SerializeField]
    public int acceleration { get; private set; }
    [field: SerializeField]
    public int breakAcceleration { get; private set; }
    [field: SerializeField]
    public float slideSpeed { get; private set; }

    [field: SerializeField]
    public int jumpForce { get; private set; }
    private Weapon activeWeapon = new Weapon(28, Weapon.Type.sword);

    public Vector2 input { get; private set; }
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
    delegate void TimerEventHandler();
    TimerEventHandler _timerEvent;
    event TimerEventHandler timerAction
    {
        add
        {
            if (!timerActions.Contains(value.Method.Name))
            {
                _timerEvent += value;
                timerActions.Add(value.Method.Name);
            }
        }
        remove
        {
            _timerEvent -= value;
            timerActions.Remove(value.Method.Name);
        }
    }


    public bool sliding { get; private set; }
    public bool attached { get; private set; }
    private void Start()
    {
        base.Start();
        _timerEvent = () => { };
        attackAction += Attach;
    }

    protected void FixedUpdate()
    {
        base.FixedUpdate();
        GetLastDir();
        sliding = false;
        if (touchWall != 0 && input.y > -0.5) Slide();
        StayAttached();
        Move();

        LogComponent.Write(attackQueue + "  " + attackTimer);
        _timerEvent.Invoke();
    }
    void Move()
    {
        float currentSpeed = selfRB.velocity.x;
        int dir = Math.Sign(input.x);
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

        if (jumpTimer != 0 || (!onGround && touchWall == 0)) return;
        Action addCF = () =>
        {
            var currentVericalSpeed = selfRB.velocity.y;
            Vector2 compensatingForce = Vector2.up * -currentVericalSpeed / Time.fixedDeltaTime * selfRB.mass;
            selfRB.AddForce(compensatingForce);
        };

        jumpEvent = () => selfRB.AddForce((jumpForce / 2) * (1 + additionalJumpForce) * jumpVector);
        jumpEvent += addCF;
        timerAction += JumpWait;
    }

    void JumpWait()
    {
        jumpTimer++;
        if (jumpTimer >= jumpPrepareFrames)
        {
            jumpTimer = 0;
            timerAction -= JumpWait;
            jumpEvent.Invoke();
        }
    }
    Action attackAction = () => { };
    [field: SerializeField]
    public bool attackQueue { get; private set; }
    Action onAttackFinish = () => { };

    public void Attack()
    {
        if (attacking) return;
        if (attackTimer != 0 && attackQueue == true) return;
        else if (attackTimer != 0)
        {
            onAttackFinish = Attack;
            onAttackFinish += () => attacking = true;
            attackQueue = true;
            return;
        }
        attackCounter++;
        timerAction += AttackWait;
    }
    void AttackWait()
    {
        if (attackTimer == 0) attacking = true;
        attackTimer++;
        attackAction();
        if (attackTimer >= activeWeapon.time)
        {
            attackTimer = 0;
            timerAction -= AttackWait;
            attackQueue = false;
            attacking = false;
            onAttackFinish.Invoke();
            onAttackFinish = () => { };
        }
        LogComponent.Write("" + attackTimer + " " + attacking);
    }
    public int lastDirX { get; private set; }
    void GetLastDir()
    {
        if (input != Vector2.zero)
        {
            if (attackTimer == 0 && !attached)
            {
                if (input.y < -0.7) lastDir = Vector2.down;
                else { lastDir = Vector2.right * Math.Sign(input.x); lastDirX = Math.Sign(input.x); }

                }
            if (sliding && isHardWall)
            {
                lastDir = Vector2.right * touchWall;
                lastDirX=touchWall; ;
            }
        }
        Debug.DrawRay(transform.position, lastDir, Color.red);
    }
    bool CastAttachingRay()
    {
        Vector2 direction = lastDir;
        int rayLength = 2;//Weapon's length
        /*RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, rayLength);
        print("cock");
        Debug.DrawRay(transform.position, direction * rayLength, Color.green, 5f);
        Surface surface;
        foreach (var hit in hits)
        {
            if (hit.transform.TryGetComponent(out surface))
            {
                return !surface.Hard;
            }
        }
        return false;*/
        if (lastDir.y == 0) return !isHardWall;
        else if(FloorSurface!=null)return !FloorSurface.Hard;
        return false;
    }
    void Slide()
    {
        if (onGround || attached) return;
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
    [field:SerializeField]
    public bool attaching { get; private set; }=false;
    void AttachEvent() { timerAction -= AttachEvent; attaching = false; }
    void Attach()
    {
        if (isHardWall&&!CastAttachingRay()) return;
        attaching = true;
        timerAction+=AttachEvent;
        attached = true;
        attackQueue = false;
        timerAction -= AttackWait;
        attacking = false;
        onAttackFinish = () => { };
        attackAction += Detach;
        attackAction -= Attach;
        attackTimer = 0;
    }
    void Detach()
    {
        attaching = true;
        timerAction += AttachEvent;
        attached = false;
        timerAction -= AttackWait;
        attacking = false;
        attackQueue = false;
        attackTimer = 0;
        attackAction -= Detach;
        attackAction += Attach;
    }
}
public class Weapon
{
    public Weapon(int t, Type _type) { time = t;type = _type;}
    public int time { get; private set; }
    public enum Type { sword, gun}
    public Type type { get; private set; }
}
