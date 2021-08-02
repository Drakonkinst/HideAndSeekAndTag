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

public class Grid {
    private int sizeX;
    private int sizeY;
    private Tile[,] grid;
    private AStar pathfinding;

    public Grid(int sizeX, int sizeY)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;

        grid = new Tile[sizeX, sizeY];


        for(int i = 0; i < sizeX; ++i)
        {
            for(int j = 0; j < sizeY; ++j)
            {
                grid[i, j] = new Tile(new Point(i, j));
            }
        }

        RefreshGrid();
    }

    public void RefreshGrid()
    {
        pathfinding = new AStar(this);
    }

    // Resets all tile colors
    public void ResetTileColors()
    {
        for (int i = 0; i < sizeX; ++i)
        {
            for (int j = 0; j < sizeY; ++j)
            {
                GetTile(i, j).TileObject.ResetColor();
            }
        }
    }

    // Reveals tiles based on player sight
    public void ColorPlayerSight(int[,] playerSight)
    {
        for (int i = 0; i < sizeX; ++i)
        {
            for (int j = 0; j < sizeY; ++j)
            {
                bool isRevealed = playerSight[i, j] >= 0;
                if(isRevealed)
                {
                    GetTile(i, j).TileObject.Reveal();
                }
            }
        }
    }

    // Returns sight matrix centered at the given point with the given distance
    public int[,] GetSight(Point center, int maxDistance)
    {
        int[,] sight = CreateBlankSight();
        RecursiveSight(sight, maxDistance, center.GetX(), center.GetY(), 0, center);
        return sight;
    }

    // Merges two sight matrices together
    // https://stackoverflow.com/questions/4260207/how-do-you-get-the-width-and-height-of-a-multi-dimensional-array/4260228
    public int[,] MergeSight(int[,] sight1, int[,] sight2)
    {
        int[,] finalSight = new int[sight1.GetLength(0), sight1.GetLength(1)];
        for(int i = 0; i < sight1.GetLength(0); ++i)
        {
            for(int j = 0; j < sight2.GetLength(1); ++j)
            {
                if(sight1[i, j] >= 0 && sight2[i, j] >= 0)
                {
                    // Both visible, choose lowest value
                    if(sight1[i, j] <= sight2[i, j])
                    {
                        finalSight[i, j] = sight1[i, j];
                    }
                    else
                    {
                        finalSight[i, j] = sight2[i, j];
                    }
                }
                else if(sight1[i, j] >= 0)
                {
                    finalSight[i, j] = sight1[i, j];
                }
                else
                {
                    finalSight[i, j] = sight2[i, j];
                }
            }
        }
        return finalSight;
    }

    // Returns a blank sight matrix
    public int[,] CreateBlankSight()
    {
        int[,] sight = new int[sizeX, sizeY];
        for (int i = 0; i < sizeX; ++i)
        {
            for (int j = 0; j < sizeY; ++j)
            {
                sight[i, j] = -1;
            }
        }
        return sight;
    }

    public List<Point> FindPath(Point start, Point end)
    {
        return pathfinding.FindPath(start, end);
    }

    private void RecursiveSight(int[,] sight, int maxDistance, int x, int y, int distance, Point startPos)
    {
        // Already found and path is not better
        if(!IsValidPoint(x, y)                              // Out of bounds
            || sight[x, y] >= 0 && sight[x, y] < distance   // Already found tile and the path is not better
            || distance > maxDistance                       // Too far
            || Raycast(startPos, new Point(x, y)) != null) // Collision with obstacle, cannot see
        {
            return;
        }

        sight[x, y] = distance;
        int[] xOffset = { 1, -1, 0, 0 };
        int[] yOffset = { 0, 0, 1, -1 };

        for(int i = 0; i < xOffset.Length; ++i)
        {
            RecursiveSight(sight, maxDistance, x + xOffset[i], y + yOffset[i], distance + 1, startPos);
        }
    }

    // Raycasting on a 2D Grid using Brensham's Line Algorithm:
    // https://www.codeproject.com/Articles/15604/Ray-casting-in-a-2D-tile-based-environment
    private List<Point> BrenshamLine(int startX, int startY, int endX, int endY)
    {
        List<Point> result = new List<Point>();

        bool isSteep = Mathf.Abs(endY - startY) > Mathf.Abs(endX - startX);
        bool inverted = false;

        if(isSteep)
        {
            // Reverse dimensions
            int temp = startX;
            startX = startY;
            startY = temp;

            temp = endX;
            endX = endY;
            endY = temp;
        }

        if(startX > endX)
        {
            // Swap points
            int temp = startX;
            startX = endX;
            endX = temp;

            temp = startY;
            startY = endY;
            endY = temp;
            inverted = true;
        }

        int deltaX = endX - startX; // Guaranteed to be non-negative
        int deltaY = Mathf.Abs(endY - startY);
        int error = 0;
        int yStep;
        int y = startY;

        if(startY < endY)
        {
            yStep = 1;
        } else
        {
            yStep = -1;
        }
    
        for(int x = startX; x <= endX; ++x)
        {
            if(isSteep)
            {
                // Reverse dimensions back
                result.Add(new Point(y, x));
            } else
            {
                result.Add(new Point(x, y));
            }

            error += deltaY;
            if(2 * error >= deltaX)
            {
                y += yStep;
                error -= deltaX;
            }
        }

        if(inverted)
        {
            result.Reverse();
        }

        return result;
    }

    // Raycasts from start to end. Returns the point of collision if it collides into something
    // not walkable, otherwise returns null if there were no collisions.
    private Point Raycast(Point start, Point end)
    {
        List<Point> rayLine = BrenshamLine(start.GetX(), start.GetY(), end.GetX(), end.GetY());
        if(rayLine.Count <= 0)
        {
            return null;
        }

        // Loop through all points starting from the start
        for(int i = 0; i < rayLine.Count; ++i)
        {
            Point rayPoint = rayLine[i];
            
            // Ignore the last point
            if(rayPoint.Equals(end))
            {
                return null;
            }

            if(IsValidPoint(rayPoint))
            {
                Tile tile = GetTile(rayPoint);
                if(!tile.IsWalkable())
                {
                    return rayPoint;
                }
            } else
            {
                return rayPoint;
            }
        }

        return null;
    }

    /* Helpers */

    public bool IsValidPoint(int x, int y)
    {
        return x >= 0 && x < sizeX && y >= 0 && y < sizeY;
    }

    public bool IsValidPoint(Point p)
    {
        return IsValidPoint(p.GetX(), p.GetY());
    }

    public Tile GetTile(int x, int y)
    {
        return grid[x, y];
    }

    public Tile GetTile(Point p)
    {
        return GetTile(p.GetX(), p.GetY());
    }

    public int GetSizeX()
    {
        return sizeX;
    }

    public int GetSizeY()
    {
        return sizeY;
    }

    // Debug method
    public void PrintGrid()
    {
        string gridStr = "";
        for (int y = 0; y < sizeY; ++y)
        {
            for (int x = 0; x < sizeX; ++x)
            {
                Tile tile = GetTile(x, y);
                if (tile.IsWalkable())
                {
                    gridStr += "-";
                }
                else
                {
                    gridStr += "X";
                }
                gridStr += "   ";
            }
            gridStr += "\n\n";
        }
        Debug.Log(gridStr);
    }
}
