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

public class Defeat : MonoBehaviour
{
    public Text Score;
    public Text TimeSurvived;
    public Text LoadingText;
    public GameObject HighScoreNotif;

    // Start is called before the first frame update
    public virtual void Start()
    {
        Score.text = "Coins collected: " + GameSettings.PlayerScore;
        TimeSurvived.text = "You survived for " + GameSettings.PlayerSurvivalTime + " seconds";
        HighScoreNotif.SetActive(GameSettings.HighScoreAchievedLastGame);
    }

    public void PlayAgainOnClick()
    {
        LoadingText.text = "Loading...";
        SceneManager.LoadScene(GameSettings.HideAndSeekScene);
    }

    public void ReturnToMenuOnClick()
    {
        SceneManager.LoadScene(GameSettings.MainMenuScene);
    }
}
