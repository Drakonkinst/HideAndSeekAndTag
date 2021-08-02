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

public class Hunter : Steerable
{
    private const float brakingDistance = 0.5f;
    private const float stuckDistance = 0.5f;
    private const int maxStuckCounter = 3;

    public enum TargetType
    {
        PLAYER, FOOTPRINT, LAST_PLAYER_POS, NONE
    }

    public int SightDistance = 4;
    public Vector2 Target = Vector2.zero;
    public TargetType Type = TargetType.NONE;
    public Sprite HunterSprite1;
    public Sprite HunterSprite2;

    private int stuckCounter = 0;
    private Vector2 lastPos;
    private bool frame1 = false;

    public override void OnStart()
    {
        lastPos = myTransform.position;
        InvokeRepeating("DoStuckCheck", 1.0f, 1.0f);
        InvokeRepeating("UpdateSprite", 0.0f, 0.05f);
        sprite.sprite = HunterSprite1;
    }

    public override void DoBehavior()
    {
        Point point = GetPoint();
        Grid grid = manager.GetGrid();
        if (!grid.IsValidPoint(point))
        {
            Debug.LogWarning("Hunter is out of bounds!");
            return;
        }

        // Debug: Test wandering only
        /*
        steering.Wander();
        steering.CheckCollisions(manager);
        return;
        //*/

        if (Type != TargetType.NONE)
        {
            Point targetPoint = manager.WorldPosToPoint(Target);
            List<Point> pathToTarget = grid.FindPath(point, targetPoint);
            if (pathToTarget.Count <= 1)
            {
                // Same tile as target
                if (Type == TargetType.PLAYER)
                {
                    // Try to collide with player
                    steering.Seek(Target, 0.0f);
                    steering.SetWanderAngle(Target);
                }
                else if (Type == TargetType.LAST_PLAYER_POS)
                {
                    // Is the last known location of the player
                    ResetTarget();
                }
                else
                {
                    // Is a footprint, align with footprint direction if possible
                    Tile tile = grid.GetTile(targetPoint);
                    if (tile == null)
                    {
                        Debug.LogWarning("Tile does not exist at " + targetPoint.ToString() + "!");
                        return;
                    }

                    Vector2 footprintDirection = tile.GetFootprintDirection();

                    if (footprintDirection != Vector2.zero)
                    {
                        // Set wander angle towards the footprint direction
                        steering.SetWanderAngle(GetPosition() + footprintDirection);
                    }

                    ResetTarget();
                }
            }
            else
            {
                // Pathfind to target
                Point nextTile = pathToTarget[1];
                Vector2 nextTilePos = manager.PointToWorldPos(nextTile, true);
                steering.Seek(nextTilePos, 0.0f);
                steering.SetWanderAngle(nextTilePos);
            }
        }
        else
        {
            steering.Wander();
            steering.CheckCollisions(manager);
        }

        // If there is another Hunter in front of this hunter, brake
        Vector2 ahead = steering.GetPositionAhead(1.0f);
        List<Hunter> hunters = manager.Hunters;

        foreach (Hunter hunter in hunters)
        {
            if (hunter == this)
            {
                continue;
            }
            if (Vector2.Distance(hunter.GetPosition(), ahead) < brakingDistance)
            {
                steering.Brake();
                break; // Ha, ha
            }
        }

        SetRotation(steering.GetFacing(), 90.0f);
    }

    private void UpdateSprite()
    {
        // I don't know how to do animations lmao
        if (frame1)
        {
            sprite.sprite = HunterSprite2;
        }
        else
        {
            sprite.sprite = HunterSprite1;
        }

        frame1 = !frame1;
    }

    private void DoStuckCheck()
    {
        Vector2 currPos = myTransform.position;
        float distance = Vector2.Distance(currPos, lastPos);
        if (distance <= stuckDistance)
        {
            stuckCounter++;
            if (stuckCounter >= maxStuckCounter)
            {
                ResetTarget();
            }
        }
        else
        {
            stuckCounter = 0;
        }
        lastPos = currPos;
    }

    public void AssignTarget(Vector2 target, TargetType type)
    {
        if ((Type == TargetType.PLAYER || Type == TargetType.LAST_PLAYER_POS) && type == TargetType.FOOTPRINT)
        {
            // Lower priority, ignore
            return;
        }
        Target = target;
        Type = type;
    }

    public void ResetTarget()
    {
        Target = Vector2.zero;
        Type = TargetType.NONE;
    }
}
