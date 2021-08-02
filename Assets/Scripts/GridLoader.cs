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
using System.IO;
using System;
using UnityEngine;

public class GridLoader
{
    public static Grid LoadFromFile(string mapFile)
    {
        // https://stackoverflow.com/questions/41979055/unity-read-image-pixels/41979364
        Texture2D image = (Texture2D)Resources.Load(mapFile);
        Grid grid = new Grid(image.width, image.height);

        Debug.Log("Loading a " + image.width + " by " + image.height + " tilemap");

        for(int i = 0; i < image.width; ++i)
        {
            for(int j = 0; j < image.height; ++j)
            {
                Color pixel = image.GetPixel(i, j);

                // Non-white tiles are not walkable
                if(pixel != Color.white)
                {
                    grid.GetTile(i, j).SetWalkable(false);
                }
            }
        }

        return grid;
    }
 }
