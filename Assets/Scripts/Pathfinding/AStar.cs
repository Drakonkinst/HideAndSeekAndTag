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

public class AStar
{
    private int sizeX;
    private int sizeY;
    private AStarNode[,] nodeMap;

    public AStar(Grid grid)
    {
        sizeX = grid.GetSizeX();
        sizeY = grid.GetSizeY();
        nodeMap = new AStarNode[sizeY, sizeX];

        CreateNodeMap(grid);
    }

    private void CreateNodeMap(Grid grid)
    {
        for (int i = 0; i < sizeX; ++i)
        {
            for (int j = 0; j < sizeY; ++j)
            {
                Tile tile = grid.GetTile(i, j);
                nodeMap[j, i] = new AStarNode(tile.GetPos(), tile.IsWalkable());
            }
        }
    }

    // Returns a list of points for the shortest path between start and end
    public List<Point> FindPath(Point start, Point end)
    {
        AStarNode startNode = nodeMap[start.GetY(), start.GetX()];
        AStarNode endNode = nodeMap[end.GetY(), end.GetX()];

        MinHeap<AStarNode> openSet = new MinHeap<AStarNode>(sizeX * sizeY);
        HashSet<AStarNode> closedSet = new HashSet<AStarNode>();

        startNode.GCost = 0;
        startNode.HCost = GetCost(startNode, endNode);

        openSet.Add(startNode);

        while(openSet.Count > 0)
        {
            AStarNode currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if(currentNode == endNode)
            {
                return RetracePath(startNode, endNode);
            }

            List<AStarNode> neighbors = GetNeighbors(currentNode);
            foreach(AStarNode neighbor in neighbors)
            {
                if(!neighbor.IsWalkable() || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newCostToNeighbor = currentNode.GCost + GetCost(currentNode, neighbor);
                if(newCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor))
                {
                    neighbor.GCost = newCostToNeighbor;
                    neighbor.HCost = GetCost(neighbor, endNode);
                    neighbor.Parent = currentNode;

                    if(!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    private List<Point> RetracePath(AStarNode startNode, AStarNode endNode)
    {
        List<Point> path = new List<Point>();
        AStarNode currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode.GetPos());
            currentNode = currentNode.Parent;
        }

        path.Reverse();

        return path;
    }

    private List<AStarNode> GetNeighbors(AStarNode node)
    {
        List<AStarNode> neighbors = new List<AStarNode>();
        int x = node.GetPos().GetX();
        int y = node.GetPos().GetY();

        int[] xList = { 1, -1, 0, 0 };
        int[] yList = { 0, 0, 1, -1 };

        for(int i = 0; i < xList.Length; ++i)
        {
            int checkX = x + xList[i];
            int checkY = y + yList[i];

            if(IsValid(checkX, checkY))
            {
                AStarNode neighbor = nodeMap[checkY, checkX];
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    private bool IsValid(int x, int y)
    {
        return x >= 0 && x < sizeX && y >= 0 && y < sizeY;
    }

    private int GetCost(AStarNode a, AStarNode b)
    {
        return GetCost(a.GetPos(), b.GetPos());
    }

    private int GetCost(Point a, Point b)
    {
        int deltaX = Mathf.Abs(a.GetX() - b.GetX());
        int deltaY = Mathf.Abs(a.GetY() - b.GetY());

        return deltaX + deltaY;
    }
}
