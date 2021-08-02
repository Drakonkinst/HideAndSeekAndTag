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

public class ButtonCycle : MonoBehaviour
{
    public string[] Options;
    public string[] Descriptions;
    public Text ButtonText;
    public Text ButtonDescription;
    public int DefaultOption;

    private int currentOption;

    void Start()
    {
        currentOption = DefaultOption;
        UpdateDisplay();
    }

    public void CycleOnClick()
    {
        currentOption = (currentOption + 1) % Options.Length;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        ButtonText.text = Options[currentOption];
        ButtonDescription.text = Descriptions[currentOption];
    }

    public int GetCurrentOption()
    {
        return currentOption;
    }
}
