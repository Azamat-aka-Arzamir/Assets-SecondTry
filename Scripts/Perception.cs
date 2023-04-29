using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Perception : MonoBehaviour
{
    private const float GROUND_DETECTION_LIMIT = 0.7f;//0-any surface,0.7-45 degrees from up, 1 - strictly up
    public bool onGround { get; private set; }
    public Vector2 groundVector { get; private set; }
    public int touchWall { get; private set; }
    public bool isHardWall { get; private set; }
    private Surface _wallSurface;
    public Surface WallSurface { get => _wallSurface; }
    private Surface _floorSurface;
    public Surface FloorSurface { get => _floorSurface; }
    public Action landedEvent = () => { };
    public Action touchWallEvent = () => { };
    protected Rigidbody2D selfRB;
    private ContactPoint2D[] contactPoints;

    // Start is called before the first frame update
    protected void Start()
    {
        selfRB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    protected void FixedUpdate()
    {
        GetContacts();
        LogComponent.Write("On ground " + onGround);
        LogComponent.Write("Touch wall " + touchWall);
    }
    void GetContacts()
    {
        contactPoints = new ContactPoint2D[100];
        selfRB.GetContacts(contactPoints);
        GetGround();
        GetWalls();
    }
    void GetGround()
    {
        foreach (var contact in contactPoints)
        {
            if (contact.normal.y > GROUND_DETECTION_LIMIT)
            {
                if (!onGround) landedEvent.Invoke();
                onGround = true;
                groundVector = contact.normal;
                contact.collider.TryGetComponent(out _floorSurface);
                return;
            }
        }
        onGround = false;
        _floorSurface = null;
    }
    void GetWalls()
    {
        foreach (var contact in contactPoints)
        {
            if (contact.normal.y < 0.1f)
            {
                if (touchWall==0) touchWallEvent.Invoke();
                touchWall = (int)contact.normal.x;
                try
                {
                    isHardWall = contact.collider.GetComponent<Surface>().Hard;
                    contact.collider.TryGetComponent(out _wallSurface);
                }
                catch(Exception e)
                {
                    isHardWall = true;
                }

                return;
            }
        }
        touchWall = 0;
        _wallSurface = null;
    }
}
