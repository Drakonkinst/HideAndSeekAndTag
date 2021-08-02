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

public class Collectible : MonoBehaviour
{
    private SpriteRenderer sprite;
    private Point point;
    
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetOpacity(float opacity)
    {
        Color color = sprite.color;
        color.a = opacity;
        sprite.color = color;
    }

    public void SetPoint(Point p)
    {
        point = p;
    }

    public Point GetPoint()
    {
        return point;
    }
}
