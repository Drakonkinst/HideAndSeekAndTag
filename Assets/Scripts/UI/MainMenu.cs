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
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject MainMenuObject;
    public GameObject StatsObject;
    public GameObject CreditsObject;

    public ButtonCycle DifficultySelection;
    public ButtonCycle ModeSelection;
    public Text LoadingText;
    public Text CountdownHighScores;
    public Text SurvivalHighScores;
    public Text CollectionHighScores;

    public void Start()
    {
        ReturnToMainMenuOnClick();
        LoadHighScores();
    }

    public void PlayOnClick()
    {
        LoadingText.text = "Loading...";
        LoadSettings();
        SceneManager.LoadScene(GameSettings.HideAndSeekScene);
    }

    public void ExitOnClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ViewStatsOnClick()
    {
        StatsObject.SetActive(true);
        CreditsObject.SetActive(false);
        MainMenuObject.SetActive(false);
    }

    public void ViewCreditsOnClick()
    {
        StatsObject.SetActive(false);
        CreditsObject.SetActive(true);
        MainMenuObject.SetActive(false);
    }

    public void ReturnToMainMenuOnClick()
    {
        StatsObject.SetActive(false);
        CreditsObject.SetActive(false);
        MainMenuObject.SetActive(true);
    }

    private void LoadSettings()
    {
        int difficultyChoice = DifficultySelection.GetCurrentOption();

        if(difficultyChoice == 0)
        {
            GameSettings.GameDifficulty = Difficulty.Easy;
        }
        else if(difficultyChoice == 1)
        {
            GameSettings.GameDifficulty = Difficulty.Medium;
        }
        else if(difficultyChoice == 2)
        {
            GameSettings.GameDifficulty = Difficulty.Hard;
        }
        else
        {
            Debug.LogWarning("Invalid game difficulty choice!");
        }

        int modeChoice = ModeSelection.GetCurrentOption();
        
        if(modeChoice == 0)
        {
            GameSettings.GameMode = GameSettings.Mode.COUNTDOWN;
        }
        else if(modeChoice == 1)
        {
            GameSettings.GameMode = GameSettings.Mode.SURVIVAL;
        }
        else if(modeChoice == 2)
        {
            GameSettings.GameMode = GameSettings.Mode.COLLECTION;
        }
        else
        {
            Debug.LogWarning("Invalid game mode choice!");
        }
    }

    private void LoadHighScores()
    {
        int[,] data = GameSettings.HighScores;
        UpdateHighScores(CountdownHighScores, 0);
        UpdateHighScores(SurvivalHighScores, 1);
        UpdateHighScores(CollectionHighScores, 2);
    }

    private void UpdateHighScores(Text text, int modeIndex)
    {
        text.text = GetScoreDisplay(0, modeIndex) + "\n\n"
            + GetScoreDisplay(1, modeIndex) + "\n\n"
            + GetScoreDisplay(2, modeIndex);
    }

    private string GetScoreDisplay(int diffIndex, int modeIndex)
    {
        int score = GameSettings.HighScores[diffIndex, modeIndex];
        if(score < 0)
        {
            return "None";
        }
        return "" + score;
    }
}
