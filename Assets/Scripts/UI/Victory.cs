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
using UnityEngine.UI;

public class Victory : Defeat
{
    public Image[] Stars;
    public Sprite BlankStar;
    public Sprite FilledStar;

    public override void Start()
    {
        base.Start();
        float scorePercent = GameSettings.PlayerScore * 1.0f / GameSettings.NumCollectibles;

        // Calculate stars
        int numStars = CalculateNumStars(scorePercent);
        SetNumStars(numStars);
    }

    public static int CalculateNumStars(float percent)
    {
        if(percent >= 1.0f)
        {
            return 5;
        }
        
        if(percent > 0.75f)
        {
            return 4;
        }

        if (percent > 0.5f)
        {
            return 3;
        }

        if(percent > 0.25f)
        {
            return 2;
        }

        if(percent > 0.0f)
        {
            return 1;
        }

        return 0;
    }

    private void SetNumStars(int numStars)
    {
        for (int i = 1; i <= Stars.Length; ++i)
        {
            if (i <= numStars)
            {
                Stars[i - 1].sprite = FilledStar;
            }
            else
            {
                Stars[i - 1].sprite = BlankStar;
            }
        }
    }
}
