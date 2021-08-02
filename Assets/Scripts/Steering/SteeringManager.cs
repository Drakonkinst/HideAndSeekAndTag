/*
 * Name: Wesley Ho
 * ID: 2382489
 * Email: weho@chapman.edu
 * CPSC 236-02
 * Assignment: Final Project - Hide and Seek (and Tag)
 * This is my own work, and I did not cheat on this assignment.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based on multiple of my adaptations on Steering Behaviors
// https://gamedevelopment.tutsplus.com/series/understanding-steering-behaviors--gamedev-12732
public class SteeringManager : MonoBehaviour
{
    // Constants
    private const float maxAngle = 2.0f * Mathf.PI;
    private const float maxForce = 0.5f;
    private const float defaultSlowingRadius = 1.0f;
    private const float brakingMultiplier = 0.3f;

    // Wandering constants
    private const float wanderCircleDistance = 1.0f;
    private const float wanderCircleRadius = 1.0f;
    private const float maxAngleChange = 5.0f * Mathf.Deg2Rad;
    private const float sightDistanceMultiplier = 0.5f;
    private const float sightAngle = 30.0f * Mathf.Deg2Rad;

    // Truncates the vector's magnitude to the given value.
    private static Vector2 Truncate(Vector2 vector, float max)
    {
        if(vector.magnitude > max)
        {
            return vector.normalized * max;
        }
        return vector;
    }

    public float MaxVelocity = 2.0f;

    private GameObject host;
    private Transform hostTransform;
    private Rigidbody2D hostRb;
    private Vector2 steering;
    private float facing = 0.0f;
    private float wanderAngle;

    public void Awake()
    {
        host = gameObject;
        hostTransform = host.transform;
        hostRb = host.GetComponent<Rigidbody2D>();
        wanderAngle = Random.Range(0.0f, maxAngle);

        if (hostTransform == null || hostRb == null)
        {
            Debug.LogWarning("Host does not have Transform or Rigidbody properties!");
        }
    }

    public void Start()
    {
        Reset();
    }

    // Called once all other steering behaviors are performed
    // Applies steering force to velocity
    public void UpdateSteering()
    {
        steering = Truncate(steering, maxForce);
        hostRb.velocity += steering;
        hostRb.velocity = Truncate(hostRb.velocity, MaxVelocity);

        UpdateFacing();
        Reset();
    }

    /* Steering Behaviors */

    // Seeks out a target position. Smooth slowing radius when they get near, but can be set to 0 to disable.
    public void Seek(Vector2 targetPos, float slowingRadius = defaultSlowingRadius)
    {
        if(targetPos == null)
        {
            Debug.LogWarning("Null seek command!");
            return;
        }

        Vector2 hostPos = GetHostPosition();
        float distance = Vector2.Distance(hostPos, targetPos);
        Vector2 seekForce = (targetPos - hostPos).normalized * MaxVelocity;

        if(distance < slowingRadius)
        {
            // Truncate
            seekForce *= (distance / slowingRadius);
        }

        steering += seekForce - GetHostVelocity();
    }

    // Flees a target position with the given percentage of the maximum velocity.
    public void Flee(Vector2 targetPos, float percentage = 1.0f)
    {
        Vector2 fleeForce = (GetHostPosition() - targetPos).normalized * MaxVelocity * percentage;
        steering += fleeForce - GetHostVelocity();
    }

    // Wanders at a pseudo-random angle, modifying the current angle to make 
    // movement more natural and less random.
    public void Wander()
    {
        Vector2 circleCenter = GetHostVelocity().normalized * wanderCircleDistance;
        SetWanderAngle(wanderAngle + Random.Range(-maxAngleChange, maxAngleChange));
        Vector2 displacement = new Vector2(Mathf.Cos(wanderAngle), Mathf.Sin(wanderAngle)) * wanderCircleRadius;
        steering += circleCenter + displacement;
    }

    // Provides a cardinal direction force to avoid collisions with upcoming
    // objects if necessary. Returns whether a possible collision was found.
    public bool CheckCollisions(GameManager manager)
    {
        Vector2 hostPos = GetHostPosition();

        // Scale vector length based on current velocity
        Vector2 ahead = GetHostVelocity() * sightDistanceMultiplier;
        Vector2 aheadHalf = ahead / 2.0f;

        float leftAngle = facing - sightAngle;
        float rightAngle = facing + sightAngle;
        Vector2 aheadLeft = new Vector2(Mathf.Cos(leftAngle), Mathf.Sin(leftAngle)) * ahead.magnitude / 2.0f;
        Vector2 aheadRight = new Vector2(Mathf.Cos(rightAngle), Mathf.Sin(rightAngle)) * ahead.magnitude / 2.0f;

        Vector2[] toCheck = { ahead, aheadHalf, aheadLeft, aheadRight };
        for(int i = 0; i < toCheck.Length; ++i)
        {
            Vector2 pos = toCheck[i];
            pos += hostPos;
            Tile tile = manager.GetTileAtPos(pos);

            if (manager.IsOutOfBounds(pos) || !tile.IsWalkable())
            {
                Vector2 dir = GetCardinalDirection(hostPos, pos);
                Vector2 toFlee = hostPos + dir;
                Flee(toFlee, 0.5f);
                SetWanderAngle(hostPos - dir);
                return false;
            }
        }

        return true;
    }

    // Applies a global braking force to all steering forces
    // to make the unit come to a halt.
    // Only effective if this is the last method called.
    public void Brake()
    {
        Vector2 brakeForce = -steering * brakingMultiplier;
        brakeForce -= GetHostVelocity();
        steering += brakeForce;
    }

    /* Helper Methods */

    // Returns a point the given distance ahead of the unit.
    public Vector2 GetPositionAhead(float distance)
    {
        return GetHostPosition() + GetHostVelocity().normalized * distance;
    }

    // Simplifies the angle between two vectors to a cardinal direction.
    private Vector2 GetCardinalDirection(Vector2 center, Vector2 pos)
    {
        float deltaX = pos.x - center.x;
        float deltaY = pos.y - center.y;

        if(Mathf.Abs(deltaX) >= Mathf.Abs(deltaY))
        {
            // Left or right
            if(deltaX > 0)
            {
                return Vector2.right;
            }
            return Vector2.left;
        }
        else
        {
            // Up or down
            if(deltaY > 0)
            {
                return Vector2.up;
            }
            return Vector2.down;
        }
    }

    // Resets steering force
    private void Reset()
    {
        steering = Vector2.zero;
    }

    // Updates facing angle of unit
    private void UpdateFacing()
    {
        Vector2 velocity = GetHostVelocity();
        if(velocity.magnitude > 0)
        {
            facing = Mathf.Atan2(velocity.y, velocity.x);
        }
    }

    // Updates wander angle to face the current orientation
    public void SetWanderAngleToFacing()
    {
        Vector2 velocity = GetHostVelocity();
        if(velocity.magnitude > 0)
        {
            wanderAngle = Mathf.Atan2(velocity.y, velocity.x);
        }
    }

    // Updates wander angle to point towards the given vector
    public void SetWanderAngle(Vector2 towards)
    {
        Vector2 hostPos = GetHostPosition();
        wanderAngle = Mathf.Atan2(towards.y - hostPos.y, towards.x - hostPos.x);
    }

    // Sets the wander angle to the given value
    public void SetWanderAngle(float angle)
    {
        wanderAngle = angle % maxAngle;
        if(wanderAngle < 0)
        {
            wanderAngle += maxAngle;
        }
    }

    public Vector2 GetHostPosition()
    {
        return hostTransform.position;
    }

    public Vector2 GetHostVelocity()
    {
        return hostRb.velocity;
    }

    public float GetFacing()
    {
        return facing;
    }
}