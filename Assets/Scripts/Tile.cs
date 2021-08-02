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

// Represents a tile on the 2D grid
public class Tile
{
    public enum FootprintDirection
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        NONE,
        CONFUSED
    }

    public TileObject TileObject = null;
    private Point pos;
    private bool walkable = true;

    private FootprintDirection footprintDirection;
    private bool isPlayerFootprint = false;
    private int footprintAge = -1;
    private bool footprintFound = false;

    public Tile(Point pos)
    {
        this.pos = pos;
    }

    public void SetWalkable(bool flag)
    {
        walkable = flag;
    }

    public void RemoveFootprint()
    {
        footprintDirection = FootprintDirection.NONE;
        isPlayerFootprint = false;
        footprintAge = -1;
        footprintFound = false;
        TileObject.SetFootprints(footprintDirection);
    }

    public void IncrementFootprintAge(int maxAge)
    {
        ++footprintAge;
        float agePercent = 1.0f - (footprintAge * 1.0f / maxAge);
        TileObject.SetAgeMultiplier(agePercent);
    }

    public void PlaceFootprint(bool isPlayer, Vector2 velocity)
    {
        FootprintDirection direction = GetDirectionFromVector(velocity);

        if(direction == FootprintDirection.NONE)
        {
            // Do nothing
            return;
        }

        if(FootprintExists())
        {
            if(isPlayerFootprint != isPlayer)
            {
                // Team does not match, do not override footprint
                return;
            }

            if(direction != footprintDirection)
            {
                // Confuse the footprints
                footprintDirection = FootprintDirection.CONFUSED;
                TileObject.SetFootprints(footprintDirection, isPlayer);
            }

            // Refresh footprint age
            footprintAge = 0;
        }
        else
        {
            // Place footprint
            footprintDirection = direction;
            isPlayerFootprint = isPlayer;
            footprintAge = 0;

            GameSettings.GetManager().AddFootprintToList(pos, isPlayer);
            TileObject.SetFootprints(footprintDirection, isPlayer);
        }
    }

    public Point GetPos()
    {
        return pos;
    }

    public bool IsWalkable()
    {
        return walkable;
    }

    public int GetFootprintAge()
    {
        return footprintAge;
    }

    public Vector2 GetFootprintDirection()
    {
        if (footprintDirection == FootprintDirection.UP)
        {
            return Vector2.up;
        }
        else if (footprintDirection == FootprintDirection.DOWN)
        {
            return Vector2.down;
        }
        else if (footprintDirection == FootprintDirection.LEFT)
        {
            return Vector2.left;
        }
        else if (footprintDirection == FootprintDirection.RIGHT)
        {
            return Vector2.right;
        }
        return Vector2.zero;
    }

    public bool FootprintExists()
    {
        return footprintAge > -1;
    }

    public bool IsFootprintFound()
    {
        return footprintFound;
    }

    public void SetFootprintFound(bool flag)
    {
        footprintFound = flag;
    }

    private FootprintDirection GetDirectionFromVector(Vector2 vector)
    {
        if(vector == Vector2.zero)
        {
            // Standing still, no footprint
            return FootprintDirection.NONE;
        }

        if(Mathf.Abs(vector.x) >= Mathf.Abs(vector.y))
        {
            if(vector.x > 0)
            {
                return FootprintDirection.RIGHT;
            }
            return FootprintDirection.LEFT;
        }
        else
        {
            if(vector.y > 0)
            {
                return FootprintDirection.UP;
            }
            return FootprintDirection.DOWN;
        }
    }
}
