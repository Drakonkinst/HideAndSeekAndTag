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

public class GameSettings
{
    public enum Mode
    {
        COUNTDOWN, SURVIVAL, COLLECTION
    }

    public static Difficulty GameDifficulty = Difficulty.Medium;
    public static Mode GameMode = Mode.COUNTDOWN;
    private static GameManager manager;
    public static int NumCollectibles = 30;
    public static int PlayerScore = 0;
    public static int PlayerSurvivalTime = 0;
    // Row = difficulty, Col = mode
    public static int[,] HighScores = new int[3, 3] {
        { -1, -1, -1 },
        { -1, -1, -1 },
        { -1, -1, -1 }
    };
    public static bool HighScoreAchievedLastGame = false;

    public static int MainMenuScene = 0;
    public static int HideAndSeekScene = 1;
    public static int VictoryScene = 2;
    public static int DefeatScene = 3;

    public static GameManager GetManager()
    {
        return manager;
    }

    public static void SetManager(GameManager gameManager)
    {
        manager = gameManager;
    }

    private static int GetDifficultyIndex()
    {
        if(GameDifficulty == Difficulty.Easy)
        {
            return 0;
        }

        if(GameDifficulty == Difficulty.Medium)
        {
            return 1;
        }

        return 2;
    }

    public static void SetCountdownHighScore(int score)
    {
        HighScoreAchievedLastGame = false;
        if(score > HighScores[GetDifficultyIndex(), 0])
        {
            HighScoreAchievedLastGame = true;
            HighScores[GetDifficultyIndex(), 0] = score;
        }
    }

    public static void SetSurvivalHighScore(int score)
    {
        HighScoreAchievedLastGame = false;
        if(score > HighScores[GetDifficultyIndex(), 1])
        {
            HighScoreAchievedLastGame = true;
            HighScores[GetDifficultyIndex(), 1] = score;
        }
    }

    public static void SetCollectionHighScore(int score)
    {
        HighScoreAchievedLastGame = false;
        if(score < HighScores[GetDifficultyIndex(), 2] || HighScores[GetDifficultyIndex(), 2] < 0)
        {
            HighScoreAchievedLastGame = true;
            HighScores[GetDifficultyIndex(), 2] = score;
        }
    }

    public static int GetCountdownHighScore()
    {
        return HighScores[GetDifficultyIndex(), 0];
    }

    public static int GetSurvivalHighScore()
    {
        return HighScores[GetDifficultyIndex(), 0];
    }

    public static int GetCollectionHighScore()
    {
        return HighScores[GetDifficultyIndex(), 0];
    }
}
