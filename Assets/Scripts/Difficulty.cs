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

public class Difficulty
{
    public static readonly Difficulty Easy = new Difficulty(1, 60);
    public static readonly Difficulty Medium = new Difficulty(2, 60);
    public static readonly Difficulty Hard = new Difficulty(3, 60);

    public int NumHunters;
    public int TimeNeeded;

    public Difficulty(int numHunters, int timeNeeded)
    {
        NumHunters = numHunters;
        TimeNeeded = timeNeeded;
    }
}
